using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VN_Center.Data;
using VN_Center.Models.Entities;
using ClosedXML.Excel;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using VN_Center.Documents;

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

    // GET: Beneficiarios/ExportToExcel (Existente)
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

    // GET: Beneficiarios/ExportToPdf (Lista - Existente)
    public async Task<IActionResult> ExportToPdf()
    {
      var beneficiarios = await _context.Beneficiarios
                                  .Include(b => b.Comunidad)
                                  .OrderBy(b => b.Apellidos)
                                  .ThenBy(b => b.Nombres)
                                  .ToListAsync();
      var document = new BeneficiariosPdfDocument(beneficiarios); // Usa el documento de lista
      byte[] pdfBytes = document.GeneratePdf();
      return File(pdfBytes, "application/pdf", $"Beneficiarios_Lista_{DateTime.Now:yyyyMMddHHmmss}.pdf");
    }

    // *** NUEVA ACCIÓN PARA EXPORTAR DETALLES DE UN BENEFICIARIO A PDF ***
    // GET: Beneficiarios/ExportDetailToPdf/5
    public async Task<IActionResult> ExportDetailToPdf(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      // Cargar el beneficiario con todos los datos necesarios para el detalle
      // Incluye las relaciones que quieras mostrar en el PDF de detalle
      var beneficiario = await _context.Beneficiarios
          .Include(b => b.Comunidad)
          // Podrías incluir otras relaciones si las vas a mostrar en el PDF:
          // .Include(b => b.BeneficiarioAsistenciaRecibida).ThenInclude(ba => ba.TiposAsistencia) 
          // .Include(b => b.BeneficiarioGrupos).ThenInclude(bg => bg.GruposComunitarios)
          // .Include(b => b.BeneficiariosProgramasProyectos).ThenInclude(bpp => bpp.ProgramasProyectosONG)
          .FirstOrDefaultAsync(m => m.BeneficiarioID == id);

      if (beneficiario == null)
      {
        return NotFound();
      }

      var document = new BeneficiarioDetailPdfDocument(beneficiario); // Usa el nuevo documento de detalle
      byte[] pdfBytes = document.GeneratePdf();

      return File(pdfBytes, "application/pdf", $"Beneficiario_Detalle_{beneficiario.BeneficiarioID}_{DateTime.Now:yyyyMMddHHmmss}.pdf");
    }


    // GET: Beneficiarios/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var beneficiarios = await _context.Beneficiarios
          .Include(b => b.Comunidad)
          // Incluye otras relaciones aquí si las muestras en la vista de Detalles HTML
          .FirstOrDefaultAsync(m => m.BeneficiarioID == id);
      if (beneficiarios == null)
      {
        return NotFound();
      }

      return View(beneficiarios);
    }

    private void PopulateComunidadesDropDownList(object? selectedComunidad = null)
    {
      var comunidadesQuery = from c in _context.Comunidades
                             orderby c.NombreComunidad
                             select c;
      ViewData["ComunidadID"] = new SelectList(comunidadesQuery.AsNoTracking(), "ComunidadID", "NombreComunidad", selectedComunidad);
    }

    // GET: Beneficiarios/Create
    public IActionResult Create()
    {
      PopulateComunidadesDropDownList();
      return View();
    }

    // POST: Beneficiarios/Create
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

    // GET: Beneficiarios/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }
      var beneficiarios = await _context.Beneficiarios.FindAsync(id);
      if (beneficiarios == null)
      {
        return NotFound();
      }
      PopulateComunidadesDropDownList(beneficiarios.ComunidadID);
      return View(beneficiarios);
    }

    // POST: Beneficiarios/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("BeneficiarioID,FechaRegistroBeneficiario,ComunidadID,Nombres,Apellidos,RangoEdad,Genero,PaisOrigen,OtroPaisOrigen,EstadoMigratorio,OtroEstadoMigratorio,NumeroPersonasHogar,ViviendaAlquiladaOPropia,MiembrosHogarEmpleados,EstaEmpleadoPersonalmente,TipoSituacionLaboral,OtroTipoSituacionLaboral,TipoTrabajoRealizadoSiEmpleado,OtroTipoTrabajoRealizado,EstadoCivil,TiempoEnCostaRicaSiMigrante,TiempoViviendoEnComunidadActual,IngresosSuficientesNecesidades,NivelEducacionCompletado,OtroNivelEducacion,InscritoProgramaEducacionCapacitacionActual,NinosHogarAsistenEscuela,BarrerasAsistenciaEscolarNinos,OtroBarrerasAsistenciaEscolar,PercepcionAccesoIgualOportunidadesLaboralesMujeres,DisponibilidadServiciosMujeresVictimasViolencia,DisponibilidadServiciosSaludMujer,DisponibilidadServiciosApoyoAdultosMayores,AccesibilidadServiciosTransporteComunidad,AccesoComputadora,AccesoInternet")] Beneficiarios beneficiarios)
    {
      if (id != beneficiarios.BeneficiarioID)
      {
        return NotFound();
      }
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
          if (!BeneficiariosExists(beneficiarios.BeneficiarioID))
          {
            return NotFound();
          }
          else
          {
            throw;
          }
        }
        return RedirectToAction(nameof(Index));
      }
      PopulateComunidadesDropDownList(beneficiarios.ComunidadID);
      return View(beneficiarios);
    }

    // GET: Beneficiarios/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }
      var beneficiarios = await _context.Beneficiarios
          .Include(b => b.Comunidad)
          .FirstOrDefaultAsync(m => m.BeneficiarioID == id);
      if (beneficiarios == null)
      {
        return NotFound();
      }
      return View(beneficiarios);
    }

    // POST: Beneficiarios/Delete/5
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
