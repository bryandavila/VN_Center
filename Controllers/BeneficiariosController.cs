using System;
using System.Collections.Generic; // Para IEnumerable
using System.IO;                  // Para MemoryStream
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // *** Para SelectList ***
using Microsoft.EntityFrameworkCore;
using VN_Center.Data;
using VN_Center.Models.Entities;
using ClosedXML.Excel;                 // Para XLWorkbook, XLColor, etc.
using QuestPDF.Fluent;                 // Para la API fluida de QuestPDF
using QuestPDF.Infrastructure;         // Para IDocument, etc. de QuestPDF
using VN_Center.Documents;             // *** Para BeneficiariosPdfDocument y BeneficiarioDetailPdfDocument ***

namespace VN_Center.Controllers
{
  [Authorize]
  public class BeneficiariosController : Controller
  {
    private readonly VNCenterDbContext _context;

    public BeneficiariosController(VNCenterDbContext context)
    {
      _context = context;
    }

    // GET: Beneficiarios
    public async Task<IActionResult> Index()
    {
      var vNCenterDbContext = _context.Beneficiarios.Include(b => b.Comunidad)
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
    // La línea 96 (aproximadamente) es la siguiente:
    public async Task<IActionResult> ExportToPdf()
    {
      var beneficiarios = await _context.Beneficiarios
                                  .Include(b => b.Comunidad)
                                  .OrderBy(b => b.Apellidos)
                                  .ThenBy(b => b.Nombres)
                                  .ToListAsync();
      // Esta línea requiere 'using VN_Center.Documents;' y que BeneficiariosPdfDocument.cs exista en esa carpeta/namespace
      var document = new BeneficiariosPdfDocument(beneficiarios);
      byte[] pdfBytes = document.GeneratePdf();
      return File(pdfBytes, "application/pdf", $"Beneficiarios_Lista_{DateTime.Now:yyyyMMddHHmmss}.pdf");
    }

    // GET: Beneficiarios/ExportDetailToPdf/5 (Usando QuestPDF)
    public async Task<IActionResult> ExportDetailToPdf(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }
      var beneficiario = await _context.Beneficiarios
          .Include(b => b.Comunidad)
          .FirstOrDefaultAsync(m => m.BeneficiarioID == id);

      if (beneficiario == null)
      {
        return NotFound();
      }

      // Esta línea requiere 'using VN_Center.Documents;' y que BeneficiarioDetailPdfDocument.cs exista en esa carpeta/namespace
      var document = new BeneficiarioDetailPdfDocument(beneficiario);
      byte[] pdfBytes = document.GeneratePdf();

      return File(pdfBytes, "application/pdf", $"Beneficiario_Detalle_{beneficiario.BeneficiarioID}_{DateTime.Now:yyyyMMddHHmmss}.pdf");
    }

    // GET: Beneficiarios/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null) { return NotFound(); }
      var beneficiarios = await _context.Beneficiarios
          .Include(b => b.Comunidad)
          .FirstOrDefaultAsync(m => m.BeneficiarioID == id);
      if (beneficiarios == null) { return NotFound(); }
      return View(beneficiarios);
    }

    // La línea 137 (aproximadamente) está dentro de este método:
    private void PopulateComunidadesDropDownList(object? selectedComunidad = null)
    {
      var comunidadesQuery = from c in _context.Comunidades orderby c.NombreComunidad select c;
      // Esta línea requiere 'using Microsoft.AspNetCore.Mvc.Rendering;' para SelectList
      ViewData["ComunidadID"] = new SelectList(comunidadesQuery.AsNoTracking(), "ComunidadID", "NombreComunidad", selectedComunidad);
    }
    public IActionResult Create()
    {
      PopulateComunidadesDropDownList();
      return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
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
