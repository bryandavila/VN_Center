// VN_Center/Controllers/BeneficiariosController.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VN_Center.Data; // Asumiendo que VNCenterDbContext está aquí o en un namespace similar
using VN_Center.Models.Entities;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using VN_Center.Documents;
using Microsoft.AspNetCore.Hosting; // Necesario para IWebHostEnvironment

namespace VN_Center.Controllers
{
  [Authorize]
  public class BeneficiariosController : Controller
  {
    private readonly VNCenterDbContext _context; // Usando VNCenterDbContext
    private readonly IWebHostEnvironment _webHostEnvironment; // Para obtener la ruta wwwroot

    public BeneficiariosController(VNCenterDbContext context, IWebHostEnvironment webHostEnvironment)
    {
      _context = context;
      _webHostEnvironment = webHostEnvironment; // Inyectar IWebHostEnvironment
    }

    // GET: Beneficiarios
    public async Task<IActionResult> Index()
    {
      var vNCenterDbContext = _context.Beneficiarios
                                      .Include(b => b.Comunidad) // Asumiendo que la entidad Beneficiarios tiene una propiedad de navegación Comunidad
                                      .OrderBy(b => b.Apellidos)
                                      .ThenBy(b => b.Nombres);
      return View(await vNCenterDbContext.ToListAsync());
    }

    // GET: Beneficiarios/ExportToExcel
    public async Task<IActionResult> ExportToExcel()
    {
      var beneficiarios = await _context.Beneficiarios
                                      .Include(b => b.Comunidad)
                                      .OrderBy(b => b.Apellidos)
                                      .ThenBy(b => b.Nombres)
                                      .ToListAsync();

      using (var workbook = new XLWorkbook())
      {
        var worksheet = workbook.Worksheets.Add("Beneficiarios");
        var currentRow = 1;
        worksheet.Cell(currentRow, 1).Value = "ID";
        worksheet.Cell(currentRow, 2).Value = "Nombres";
        worksheet.Cell(currentRow, 3).Value = "Apellidos";
        worksheet.Cell(currentRow, 4).Value = "Comunidad"; // Asumiendo Comunidad tiene NombreComunidad
        worksheet.Cell(currentRow, 5).Value = "Rango Edad";
        worksheet.Cell(currentRow, 6).Value = "Género";
        worksheet.Cell(currentRow, 7).Value = "Fecha de Registro";

        var headerRange = worksheet.Range(currentRow, 1, currentRow, 7);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        foreach (var ben in beneficiarios)
        {
          currentRow++;
          worksheet.Cell(currentRow, 1).Value = ben.BeneficiarioID;
          worksheet.Cell(currentRow, 2).Value = ben.Nombres;
          worksheet.Cell(currentRow, 3).Value = ben.Apellidos;
          worksheet.Cell(currentRow, 4).Value = ben.Comunidad?.NombreComunidad; // Protección contra nulos
          worksheet.Cell(currentRow, 5).Value = ben.RangoEdad;
          worksheet.Cell(currentRow, 6).Value = ben.Genero;
          worksheet.Cell(currentRow, 7).Value = ben.FechaRegistroBeneficiario; // Usando FechaRegistroBeneficiario
          worksheet.Cell(currentRow, 7).Style.DateFormat.Format = "dd/MM/yyyy";
        }
        worksheet.Columns().AdjustToContents();
        using (var stream = new MemoryStream())
        {
          workbook.SaveAs(stream);
          var content = stream.ToArray();
          return File(
              content,
              "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
              $"Beneficiarios_Lista_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }
      }
    }

    // GET: Beneficiarios/ExportToPdf (Lista - Usando QuestPDF)
    public async Task<IActionResult> ExportToPdf()
    {
      var beneficiarios = await _context.Beneficiarios
                                      .Include(b => b.Comunidad)
                                      .OrderBy(b => b.Apellidos)
                                      .ThenBy(b => b.Nombres)
                                      .ToListAsync();
      var document = new BeneficiariosPdfDocument(beneficiarios); // Asume que BeneficiariosPdfDocument espera una lista de 'Beneficiarios'
      byte[] pdfBytes = document.GeneratePdf();
      return File(pdfBytes, "application/pdf", $"Beneficiarios_Lista_{DateTime.Now:yyyyMMddHHmmss}.pdf");
    }

    // GET: Beneficiarios/ExportDetailToPdf/5
    public async Task<IActionResult> ExportDetailToPdf(int id)
    {
      var beneficiario = await _context.Beneficiarios
          // Incluir todas las colecciones y entidades relacionadas necesarias para el PDF
          .Include(b => b.FuenteConocimiento) // Asumiendo que existe esta propiedad de navegación
          .Include(b => b.BeneficiarioProgramaProyectos)
              .ThenInclude(bpp => bpp.ProgramaProyecto)
          .Include(b => b.BeneficiarioGrupos)
              .ThenInclude(bg => bg.GrupoComunitario)
          .Include(b => b.AsistenciasRecibidas)
          .Include(b => b.ParticipacionesActivas)
          .Include(b => b.EvaluacionesPrograma)
              .ThenInclude(ep => ep.ProgramaProyecto)
          // Considera si necesitas cargar el usuario que registró o actualizó, si vas a mostrar sus nombres
          // .Include(b => b.UsuarioRegistro) 
          .FirstOrDefaultAsync(m => m.BeneficiarioID == id); // Corregido a BeneficiarioID

      if (beneficiario == null)
      {
        return NotFound();
      }

      // Obtener la ruta del logo
      string wwwRootPath = _webHostEnvironment.WebRootPath;
      string logoPath = Path.Combine(wwwRootPath, "img", "logo_vncenter_mini.png"); // Ajusta la ruta a tu logo si es diferente

      // Pasar el objeto 'beneficiario' de tipo 'Beneficiarios'
      var document = new BeneficiarioDetailPdfDocument(beneficiario, logoPath);
      var pdfBytes = document.GeneratePdf();

      // Usar Cedula si existe y no es nula, sino BeneficiarioID para el nombre del archivo
      string fileIdentifier = !string.IsNullOrEmpty(beneficiario.Cedula) ? beneficiario.Cedula : beneficiario.BeneficiarioID.ToString();
      return File(pdfBytes, "application/pdf", $"Beneficiario_{fileIdentifier}_Detalle.pdf");
    }

    // GET: Beneficiarios/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null) { return NotFound(); }
      var beneficiarios = await _context.Beneficiarios
          .Include(b => b.Comunidad)
          .FirstOrDefaultAsync(m => m.BeneficiarioID == id); // Usa BeneficiarioID
      if (beneficiarios == null) { return NotFound(); }
      return View(beneficiarios);
    }

    private void PopulateComunidadesDropDownList(object? selectedComunidad = null)
    {
      // Asumiendo que la entidad Comunidad tiene ComunidadID y NombreComunidad
      var comunidadesQuery = from c in _context.Comunidades orderby c.NombreComunidad select c;
      ViewData["ComunidadID"] = new SelectList(comunidadesQuery.AsNoTracking(), "ComunidadID", "NombreComunidad", selectedComunidad);
    }

    public IActionResult Create()
    {
      PopulateComunidadesDropDownList();
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("BeneficiarioID,FechaRegistroBeneficiario,ComunidadID,Nombres,Apellidos,RangoEdad,Genero,PaisOrigen,OtroPaisOrigen,EstadoMigratorio,OtroEstadoMigratorio,NumeroPersonasHogar,ViviendaAlquiladaOPropia,MiembrosHogarEmpleados,EstaEmpleadoPersonalmente,TipoSituacionLaboral,OtroTipoSituacionLaboral,TipoTrabajoRealizadoSiEmpleado,OtroTipoTrabajoRealizado,EstadoCivil,TiempoEnCostaRicaSiMigrante,TiempoViviendoEnComunidadActual,IngresosSuficientesNecesidades,NivelEducacionCompletado,OtroNivelEducacion,InscritoProgramaEducacionCapacitacionActual,NinosHogarAsistenEscuela,BarrerasAsistenciaEscolarNinos,OtroBarrerasAsistenciaEscolar,PercepcionAccesoIgualOportunidadesLaboralesMujeres,DisponibilidadServiciosMujeresVictimasViolencia,DisponibilidadServiciosSaludMujer,DisponibilidadServiciosApoyoAdultosMayores,AccesibilidadServiciosTransporteComunidad,AccesoComputadora,AccesoInternet,Cedula")] Beneficiarios beneficiarios) // Añadí Cedula al Bind si es una propiedad que se debe crear/editar
    {
      ModelState.Remove("Comunidad"); // Si Comunidad es una propiedad de navegación
                                      // Quita estas líneas si estas propiedades de navegación no existen o si quieres validar su estado.
      ModelState.Remove("BeneficiarioAsistenciaRecibida");
      ModelState.Remove("BeneficiarioGrupos");
      ModelState.Remove("BeneficiariosProgramasProyectos");

      if (ModelState.IsValid)
      {
        _context.Add(beneficiarios);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Beneficiario creado exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      PopulateComunidadesDropDownList(beneficiarios.ComunidadID);
      return View(beneficiarios);
    }

    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null) { return NotFound(); }
      var beneficiarios = await _context.Beneficiarios.FindAsync(id); // FindAsync usa la PK
      if (beneficiarios == null) { return NotFound(); }
      PopulateComunidadesDropDownList(beneficiarios.ComunidadID);
      return View(beneficiarios);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("BeneficiarioID,FechaRegistroBeneficiario,ComunidadID,Nombres,Apellidos,RangoEdad,Genero,PaisOrigen,OtroPaisOrigen,EstadoMigratorio,OtroEstadoMigratorio,NumeroPersonasHogar,ViviendaAlquiladaOPropia,MiembrosHogarEmpleados,EstaEmpleadoPersonalmente,TipoSituacionLaboral,OtroTipoSituacionLaboral,TipoTrabajoRealizadoSiEmpleado,OtroTipoTrabajoRealizado,EstadoCivil,TiempoEnCostaRicaSiMigrante,TiempoViviendoEnComunidadActual,IngresosSuficientesNecesidades,NivelEducacionCompletado,OtroNivelEducacion,InscritoProgramaEducacionCapacitacionActual,NinosHogarAsistenEscuela,BarrerasAsistenciaEscolarNinos,OtroBarrerasAsistenciaEscolar,PercepcionAccesoIgualOportunidadesLaboralesMujeres,DisponibilidadServiciosMujeresVictimasViolencia,DisponibilidadServiciosSaludMujer,DisponibilidadServiciosApoyoAdultosMayores,AccesibilidadServiciosTransporteComunidad,AccesoComputadora,AccesoInternet,Cedula")] Beneficiarios beneficiarios) // Añadí Cedula al Bind
    {
      if (id != beneficiarios.BeneficiarioID) { return NotFound(); }

      ModelState.Remove("Comunidad");
      ModelState.Remove("BeneficiarioAsistenciaRecibida");
      ModelState.Remove("BeneficiarioGrupos");
      ModelState.Remove("BeneficiariosProgramasProyectos");

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(beneficiarios);
          await _context.SaveChangesAsync();
          TempData["SuccessMessage"] = "Beneficiario actualizado exitosamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!BeneficiariosExists(beneficiarios.BeneficiarioID)) { return NotFound(); }
          else { throw; }
        }
        return RedirectToAction(nameof(Index));
      }
      PopulateComunidadesDropDownList(beneficiarios.ComunidadID);
      return View(beneficiarios);
    }

    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null) { return NotFound(); }
      var beneficiarios = await _context.Beneficiarios
          .Include(b => b.Comunidad)
          .FirstOrDefaultAsync(m => m.BeneficiarioID == id); // Usa BeneficiarioID
      if (beneficiarios == null) { return NotFound(); }
      return View(beneficiarios);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var beneficiarios = await _context.Beneficiarios.FindAsync(id);
      if (beneficiarios != null)
      {
        _context.Beneficiarios.Remove(beneficiarios);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Beneficiario eliminado exitosamente.";
      }
      return RedirectToAction(nameof(Index));
    }

    private bool BeneficiariosExists(int id)
    {
      return _context.Beneficiarios.Any(e => e.BeneficiarioID == id); // Usa BeneficiarioID
    }
  }
}
