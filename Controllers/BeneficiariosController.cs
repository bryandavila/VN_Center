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
using VN_Center.Documents;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity; // <--- AÑADIDO para UserManager
using System.Security.Claims;      // <--- AÑADIDO para ClaimTypes

namespace VN_Center.Controllers
{
  [Authorize]
  public class BeneficiariosController : Controller
  {
    private readonly VNCenterDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly UserManager<UsuariosSistema> _userManager; // <--- AÑADIDO

    public BeneficiariosController(
        VNCenterDbContext context,
        IWebHostEnvironment webHostEnvironment,
        UserManager<UsuariosSistema> userManager) // <--- MODIFICADO
    {
      _context = context;
      _webHostEnvironment = webHostEnvironment;
      _userManager = userManager; // <--- AÑADIDO
    }

    // GET: Beneficiarios
    public async Task<IActionResult> Index()
    {
      IQueryable<Beneficiarios> query = _context.Beneficiarios
                                          .Include(b => b.Comunidad)
                                          .OrderBy(b => b.Apellidos)
                                          .ThenBy(b => b.Nombres);

      if (!User.IsInRole("Administrador"))
      {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
        {
          query = query.Where(b => b.UsuarioCreadorId == userId);
        }
        else
        {
          query = query.Where(b => false); // No mostrar nada si no se puede obtener el UserID
        }
      }
      return View(await query.ToListAsync());
    }

    // GET: Beneficiarios/ExportToExcel
    public async Task<IActionResult> ExportToExcel()
    {
      IQueryable<Beneficiarios> query = _context.Beneficiarios
                                          .Include(b => b.Comunidad)
                                          .OrderBy(b => b.Apellidos)
                                          .ThenBy(b => b.Nombres);

      if (!User.IsInRole("Administrador"))
      {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
        {
          query = query.Where(b => b.UsuarioCreadorId == userId);
        }
        else
        {
          query = query.Where(b => false);
        }
      }
      var beneficiarios = await query.ToListAsync();

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

    // GET: Beneficiarios/ExportToPdf (Lista)
    public async Task<IActionResult> ExportToPdf()
    {
      IQueryable<Beneficiarios> query = _context.Beneficiarios
                                          .Include(b => b.Comunidad)
                                          .OrderBy(b => b.Apellidos)
                                          .ThenBy(b => b.Nombres);
      if (!User.IsInRole("Administrador"))
      {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
        {
          query = query.Where(b => b.UsuarioCreadorId == userId);
        }
        else
        {
          query = query.Where(b => false);
        }
      }
      var beneficiarios = await query.ToListAsync();

      var document = new BeneficiariosPdfDocument(beneficiarios); // Asume que este documento ya existe y funciona
      byte[] pdfBytes = document.GeneratePdf();
      return File(pdfBytes, "application/pdf", $"Beneficiarios_Lista_{DateTime.Now:yyyyMMddHHmmss}.pdf");
    }

    // GET: Beneficiarios/ExportDetailToPdf/5
    public async Task<IActionResult> ExportDetailToPdf(int id)
    {
      var beneficiario = await _context.Beneficiarios
          .Include(b => b.Comunidad) // Para el PDF de detalle, incluimos las relaciones necesarias
          .Include(b => b.BeneficiariosProgramasProyectos)
              .ThenInclude(bpp => bpp.ProgramaProyecto)
          .Include(b => b.BeneficiarioGrupos)
              .ThenInclude(bg => bg.GrupoComunitario)
          .Include(b => b.BeneficiarioAsistenciaRecibida)
              .ThenInclude(bar => bar.TipoAsistencia)
          .FirstOrDefaultAsync(m => m.BeneficiarioID == id);

      if (beneficiario == null)
      {
        return NotFound();
      }

      if (!User.IsInRole("Administrador") && beneficiario.UsuarioCreadorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
      {
        TempData["ErrorMessage"] = "No tiene permiso para exportar los detalles de este beneficiario.";
        return RedirectToAction(nameof(Index));
      }

      string wwwRootPath = _webHostEnvironment.WebRootPath;
      string logoPath = Path.Combine(wwwRootPath, "img", "logo_vncenter_mini.png");

      var document = new BeneficiarioDetailPdfDocument(beneficiario, logoPath); // Asume que este documento ya existe y funciona
      var pdfBytes = document.GeneratePdf();

      string fileIdentifier = beneficiario.BeneficiarioID.ToString();
      return File(pdfBytes, "application/pdf", $"Beneficiario_{fileIdentifier}_Detalle.pdf");
    }

    // GET: Beneficiarios/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null) { return NotFound(); }

      var beneficiario = await _context.Beneficiarios
          .Include(b => b.Comunidad)
          .Include(b => b.BeneficiarioAsistenciaRecibida!)
              .ThenInclude(bar => bar.TipoAsistencia)
          .Include(b => b.BeneficiarioGrupos!)
              .ThenInclude(bg => bg.GrupoComunitario)
          .Include(b => b.BeneficiariosProgramasProyectos!)
              .ThenInclude(bpp => bpp.ProgramaProyecto)
          .FirstOrDefaultAsync(m => m.BeneficiarioID == id);

      if (beneficiario == null) { return NotFound(); }

      if (!User.IsInRole("Administrador") && beneficiario.UsuarioCreadorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
      {
        TempData["ErrorMessage"] = "No tiene permiso para ver los detalles de este beneficiario.";
        return RedirectToAction(nameof(Index));
      }
      return View(beneficiario);
    }

    private void PopulateComunidadesDropDownList(object? selectedComunidad = null)
    {
      var comunidadesQuery = from c in _context.Comunidades orderby c.NombreComunidad select c;
      ViewData["ComunidadID"] = new SelectList(comunidadesQuery.AsNoTracking(), "ComunidadID", "NombreComunidad", selectedComunidad);
    }

    public IActionResult Create()
    {
      PopulateComunidadesDropDownList();
      var model = new Beneficiarios { FechaRegistroBeneficiario = DateTime.UtcNow }; // Establecer fecha por defecto
      return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("BeneficiarioID,ComunidadID,Nombres,Apellidos,RangoEdad,Genero,PaisOrigen,OtroPaisOrigen,EstadoMigratorio,OtroEstadoMigratorio,NumeroPersonasHogar,ViviendaAlquiladaOPropia,MiembrosHogarEmpleados,EstaEmpleadoPersonalmente,TipoSituacionLaboral,OtroTipoSituacionLaboral,TipoTrabajoRealizadoSiEmpleado,OtroTipoTrabajoRealizado,EstadoCivil,TiempoEnCostaRicaSiMigrante,TiempoViviendoEnComunidadActual,IngresosSuficientesNecesidades,NivelEducacionCompletado,OtroNivelEducacion,InscritoProgramaEducacionCapacitacionActual,NinosHogarAsistenEscuela,BarrerasAsistenciaEscolarNinos,OtroBarrerasAsistenciaEscolar,PercepcionAccesoIgualOportunidadesLaboralesMujeres,DisponibilidadServiciosMujeresVictimasViolencia,DisponibilidadServiciosSaludMujer,DisponibilidadServiciosApoyoAdultosMayores,AccesibilidadServiciosTransporteComunidad,AccesoComputadora,AccesoInternet")] Beneficiarios beneficiario)
    {
      ModelState.Remove("Comunidad");
      ModelState.Remove("BeneficiarioAsistenciaRecibida");
      ModelState.Remove("BeneficiarioGrupos");
      ModelState.Remove("BeneficiariosProgramasProyectos");
      // ModelState.Remove("UsuarioCreador"); // Si añades la propiedad de navegación

      if (ModelState.IsValid)
      {
        beneficiario.UsuarioCreadorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        beneficiario.FechaRegistroBeneficiario = DateTime.UtcNow; // Asegurar fecha de registro

        _context.Add(beneficiario);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Beneficiario creado exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      PopulateComunidadesDropDownList(beneficiario.ComunidadID);
      return View(beneficiario);
    }

    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null) { return NotFound(); }
      var beneficiario = await _context.Beneficiarios.FindAsync(id);
      if (beneficiario == null) { return NotFound(); }

      if (!User.IsInRole("Administrador") && beneficiario.UsuarioCreadorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
      {
        TempData["ErrorMessage"] = "No tiene permiso para editar este beneficiario.";
        return RedirectToAction(nameof(Index));
      }

      PopulateComunidadesDropDownList(beneficiario.ComunidadID);
      return View(beneficiario);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("BeneficiarioID,FechaRegistroBeneficiario,ComunidadID,Nombres,Apellidos,RangoEdad,Genero,PaisOrigen,OtroPaisOrigen,EstadoMigratorio,OtroEstadoMigratorio,NumeroPersonasHogar,ViviendaAlquiladaOPropia,MiembrosHogarEmpleados,EstaEmpleadoPersonalmente,TipoSituacionLaboral,OtroTipoSituacionLaboral,TipoTrabajoRealizadoSiEmpleado,OtroTipoTrabajoRealizado,EstadoCivil,TiempoEnCostaRicaSiMigrante,TiempoViviendoEnComunidadActual,IngresosSuficientesNecesidades,NivelEducacionCompletado,OtroNivelEducacion,InscritoProgramaEducacionCapacitacionActual,NinosHogarAsistenEscuela,BarrerasAsistenciaEscolarNinos,OtroBarrerasAsistenciaEscolar,PercepcionAccesoIgualOportunidadesLaboralesMujeres,DisponibilidadServiciosMujeresVictimasViolencia,DisponibilidadServiciosSaludMujer,DisponibilidadServiciosApoyoAdultosMayores,AccesibilidadServiciosTransporteComunidad,AccesoComputadora,AccesoInternet")] Beneficiarios beneficiarioModificado)
    {
      if (id != beneficiarioModificado.BeneficiarioID) { return NotFound(); }

      var beneficiarioOriginal = await _context.Beneficiarios.AsNoTracking().FirstOrDefaultAsync(b => b.BeneficiarioID == id);
      if (beneficiarioOriginal == null) { return NotFound(); }

      if (!User.IsInRole("Administrador") && beneficiarioOriginal.UsuarioCreadorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
      {
        TempData["ErrorMessage"] = "No tiene permiso para editar este beneficiario.";
        return RedirectToAction(nameof(Index));
      }

      // Preservar UsuarioCreadorId y FechaRegistroBeneficiario
      beneficiarioModificado.UsuarioCreadorId = beneficiarioOriginal.UsuarioCreadorId;
      beneficiarioModificado.FechaRegistroBeneficiario = beneficiarioOriginal.FechaRegistroBeneficiario;

      ModelState.Remove("Comunidad");
      ModelState.Remove("BeneficiarioAsistenciaRecibida");
      ModelState.Remove("BeneficiarioGrupos");
      ModelState.Remove("BeneficiariosProgramasProyectos");

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(beneficiarioModificado);
          await _context.SaveChangesAsync();
          TempData["SuccessMessage"] = "Beneficiario actualizado exitosamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!BeneficiariosExists(beneficiarioModificado.BeneficiarioID)) { return NotFound(); }
          else { throw; }
        }
        return RedirectToAction(nameof(Index));
      }
      PopulateComunidadesDropDownList(beneficiarioModificado.ComunidadID);
      return View(beneficiarioModificado);
    }

    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null) { return NotFound(); }
      var beneficiario = await _context.Beneficiarios
          .Include(b => b.Comunidad)
          .FirstOrDefaultAsync(m => m.BeneficiarioID == id);
      if (beneficiario == null) { return NotFound(); }

      if (!User.IsInRole("Administrador") && beneficiario.UsuarioCreadorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
      {
        TempData["ErrorMessage"] = "No tiene permiso para eliminar este beneficiario.";
        return RedirectToAction(nameof(Index));
      }
      return View(beneficiario);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var beneficiario = await _context.Beneficiarios.FindAsync(id);
      if (beneficiario != null)
      {
        if (!User.IsInRole("Administrador") && beneficiario.UsuarioCreadorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
        {
          TempData["ErrorMessage"] = "No tiene permiso para eliminar este beneficiario.";
          return RedirectToAction(nameof(Index));
        }
        _context.Beneficiarios.Remove(beneficiario);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Beneficiario eliminado exitosamente.";
      }
      else
      {
        TempData["ErrorMessage"] = "El beneficiario no fue encontrado.";
      }
      return RedirectToAction(nameof(Index));
    }

    private bool BeneficiariosExists(int id)
    {
      return _context.Beneficiarios.Any(e => e.BeneficiarioID == id);
    }
  }
}
