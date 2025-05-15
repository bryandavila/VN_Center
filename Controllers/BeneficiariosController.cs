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
using VN_Center.Data;
using VN_Center.Models.Entities;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using VN_Center.Documents; // Asegúrate que este using esté presente
using Microsoft.AspNetCore.Hosting;

namespace VN_Center.Controllers
{
  [Authorize]
  public class BeneficiariosController : Controller
  {
    private readonly VNCenterDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public BeneficiariosController(VNCenterDbContext context, IWebHostEnvironment webHostEnvironment)
    {
      _context = context;
      _webHostEnvironment = webHostEnvironment;
    }

    // GET: Beneficiarios
    public async Task<IActionResult> Index()
    {
      var vNCenterDbContext = _context.Beneficiarios
                                      .Include(b => b.Comunidad)
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
        worksheet.Cell(currentRow, 4).Value = "Comunidad";
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
          worksheet.Cell(currentRow, 4).Value = ben.Comunidad?.NombreComunidad;
          worksheet.Cell(currentRow, 5).Value = ben.RangoEdad;
          worksheet.Cell(currentRow, 6).Value = ben.Genero;
          worksheet.Cell(currentRow, 7).Value = ben.FechaRegistroBeneficiario;
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
      // Asegúrate de que BeneficiariosPdfDocument.cs exista en la carpeta Documents
      // y que el namespace sea VN_Center.Documents
      var document = new BeneficiariosPdfDocument(beneficiarios);
      byte[] pdfBytes = document.GeneratePdf();
      return File(pdfBytes, "application/pdf", $"Beneficiarios_Lista_{DateTime.Now:yyyyMMddHHmmss}.pdf");
    }

    // GET: Beneficiarios/ExportDetailToPdf/5
    public async Task<IActionResult> ExportDetailToPdf(int id)
    {
      var beneficiario = await _context.Beneficiarios
          // IMPORTANTE: Revisa las inclusiones (Include/ThenInclude)
          // Deben coincidir con las propiedades que USAS en BeneficiarioDetailPdfDocument.cs
          // y que EXISTEN en tus entidades.

          // La entidad Beneficiarios NO tiene una FK directa o propiedad de navegación 'FuenteConocimiento'.
          // Esta relación debe definirse en el modelo Beneficiarios.cs y migrarse a la BD.
          // .Include(b => b.FuenteConocimiento) 

          .Include(b => b.BeneficiariosProgramasProyectos) // Esta colección SÍ existe en Beneficiarios.cs
              .ThenInclude(bpp => bpp.ProgramaProyecto)
          .Include(b => b.BeneficiarioGrupos) // Esta colección SÍ existe en Beneficiarios.cs
              .ThenInclude(bg => bg.GrupoComunitario)
          .Include(b => b.BeneficiarioAsistenciaRecibida) // Esta colección SÍ existe en Beneficiarios.cs
               .ThenInclude(bar => bar.TipoAsistencia) // Para acceder al nombre del tipo de asistencia

          // Las siguientes colecciones NO existen directamente en la entidad Beneficiarios.cs
          // Probablemente necesites obtener estos datos de otra manera o a través de otras entidades.
          // .Include(b => b.ParticipacionesActivas) 
          // .Include(b => b.EvaluacionesPrograma)
          //     .ThenInclude(ep => ep.ProgramaProyecto)

          .FirstOrDefaultAsync(m => m.BeneficiarioID == id);

      if (beneficiario == null)
      {
        return NotFound();
      }

      string wwwRootPath = _webHostEnvironment.WebRootPath;
      string logoPath = Path.Combine(wwwRootPath, "img", "logo_vncenter_mini.png");

      var document = new BeneficiarioDetailPdfDocument(beneficiario, logoPath);
      var pdfBytes = document.GeneratePdf();

      // La propiedad Cedula NO existe en tu entidad Beneficiarios.cs.
      // Debes añadirla o usar otro identificador para el nombre del archivo.
      // string fileIdentifier = !string.IsNullOrEmpty(beneficiario.Cedula) ? beneficiario.Cedula : beneficiario.BeneficiarioID.ToString();
      string fileIdentifier = beneficiario.BeneficiarioID.ToString(); // Usando ID por ahora
      return File(pdfBytes, "application/pdf", $"Beneficiario_{fileIdentifier}_Detalle.pdf");
    }

    // GET: Beneficiarios/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null) { return NotFound(); }
      var beneficiarios = await _context.Beneficiarios
          .Include(b => b.Comunidad)
          // Aquí también, incluye otras propiedades de navegación si las necesitas en la vista Details.cshtml
          .Include(b => b.BeneficiarioAsistenciaRecibida!)
              .ThenInclude(bar => bar.TipoAsistencia)
          .Include(b => b.BeneficiarioGrupos!)
              .ThenInclude(bg => bg.GrupoComunitario)
          .Include(b => b.BeneficiariosProgramasProyectos!)
              .ThenInclude(bpp => bpp.ProgramaProyecto)
          .FirstOrDefaultAsync(m => m.BeneficiarioID == id);
      if (beneficiarios == null) { return NotFound(); }
      return View(beneficiarios);
    }

    private void PopulateComunidadesDropDownList(object? selectedComunidad = null)
    {
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
    // Asegúrate que el Bind incluya todas las propiedades que esperas del formulario
    // La propiedad 'Cedula' no está en tu entidad Beneficiarios.cs. Si la necesitas, añádela.
    public async Task<IActionResult> Create([Bind("BeneficiarioID,FechaRegistroBeneficiario,ComunidadID,Nombres,Apellidos,RangoEdad,Genero,PaisOrigen,OtroPaisOrigen,EstadoMigratorio,OtroEstadoMigratorio,NumeroPersonasHogar,ViviendaAlquiladaOPropia,MiembrosHogarEmpleados,EstaEmpleadoPersonalmente,TipoSituacionLaboral,OtroTipoSituacionLaboral,TipoTrabajoRealizadoSiEmpleado,OtroTipoTrabajoRealizado,EstadoCivil,TiempoEnCostaRicaSiMigrante,TiempoViviendoEnComunidadActual,IngresosSuficientesNecesidades,NivelEducacionCompletado,OtroNivelEducacion,InscritoProgramaEducacionCapacitacionActual,NinosHogarAsistenEscuela,BarrerasAsistenciaEscolarNinos,OtroBarrerasAsistenciaEscolar,PercepcionAccesoIgualOportunidadesLaboralesMujeres,DisponibilidadServiciosMujeresVictimasViolencia,DisponibilidadServiciosSaludMujer,DisponibilidadServiciosApoyoAdultosMayores,AccesibilidadServiciosTransporteComunidad,AccesoComputadora,AccesoInternet")] Beneficiarios beneficiarios)
    {
      ModelState.Remove("Comunidad");
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
      var beneficiarios = await _context.Beneficiarios.FindAsync(id);
      if (beneficiarios == null) { return NotFound(); }
      PopulateComunidadesDropDownList(beneficiarios.ComunidadID);
      return View(beneficiarios);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    // Asegúrate que el Bind incluya todas las propiedades que esperas del formulario
    // La propiedad 'Cedula' no está en tu entidad Beneficiarios.cs. Si la necesitas, añádela.
    public async Task<IActionResult> Edit(int id, [Bind("BeneficiarioID,FechaRegistroBeneficiario,ComunidadID,Nombres,Apellidos,RangoEdad,Genero,PaisOrigen,OtroPaisOrigen,EstadoMigratorio,OtroEstadoMigratorio,NumeroPersonasHogar,ViviendaAlquiladaOPropia,MiembrosHogarEmpleados,EstaEmpleadoPersonalmente,TipoSituacionLaboral,OtroTipoSituacionLaboral,TipoTrabajoRealizadoSiEmpleado,OtroTipoTrabajoRealizado,EstadoCivil,TiempoEnCostaRicaSiMigrante,TiempoViviendoEnComunidadActual,IngresosSuficientesNecesidades,NivelEducacionCompletado,OtroNivelEducacion,InscritoProgramaEducacionCapacitacionActual,NinosHogarAsistenEscuela,BarrerasAsistenciaEscolarNinos,OtroBarrerasAsistenciaEscolar,PercepcionAccesoIgualOportunidadesLaboralesMujeres,DisponibilidadServiciosMujeresVictimasViolencia,DisponibilidadServiciosSaludMujer,DisponibilidadServiciosApoyoAdultosMayores,AccesibilidadServiciosTransporteComunidad,AccesoComputadora,AccesoInternet")] Beneficiarios beneficiarios)
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
          .FirstOrDefaultAsync(m => m.BeneficiarioID == id);
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
      return _context.Beneficiarios.Any(e => e.BeneficiarioID == id);
    }
  }
}
