// VN_Center/Controllers/EvaluacionesProgramaController.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VN_Center.Data;
using VN_Center.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using QuestPDF.Fluent; // Para QuestPDF
using VN_Center.Documents; // Para EvaluacionesProgramaPdfDocument
using Microsoft.AspNetCore.Hosting; // Para IWebHostEnvironment
using System.IO; // Para Path

namespace VN_Center.Controllers
{
  [Authorize]
  public class EvaluacionesProgramaController : Controller
  {
    private readonly VNCenterDbContext _context;
    // Asegúrate de que _webHostEnvironment esté declarado como un campo de la clase
    private readonly IWebHostEnvironment _webHostEnvironment;

    // El constructor debe recibir IWebHostEnvironment y asignarlo al campo
    public EvaluacionesProgramaController(VNCenterDbContext context, IWebHostEnvironment webHostEnvironment)
    {
      _context = context;
      _webHostEnvironment = webHostEnvironment;
    }

    // GET: EvaluacionesPrograma
    public async Task<IActionResult> Index()
    {
      var vNCenterDbContext = _context.EvaluacionesPrograma
                                  .Include(e => e.ParticipacionActiva)
                                      .ThenInclude(pa => pa!.Solicitud)
                                  .Include(e => e.ParticipacionActiva)
                                      .ThenInclude(pa => pa!.ProgramaProyecto);
      return View(await vNCenterDbContext.OrderByDescending(e => e.FechaEvaluacion).ToListAsync());
    }

    // GET: EvaluacionesPrograma/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var evaluacionesPrograma = await _context.EvaluacionesPrograma
          .Include(e => e.ParticipacionActiva)
              .ThenInclude(pa => pa!.Solicitud)
          .Include(e => e.ParticipacionActiva)
              .ThenInclude(pa => pa!.ProgramaProyecto)
          .FirstOrDefaultAsync(m => m.EvaluacionID == id);
      if (evaluacionesPrograma == null)
      {
        return NotFound();
      }

      return View(evaluacionesPrograma);
    }

    private void PopulateParticipacionesDropDownList(object? selectedParticipacion = null)
    {
      var participacionesQuery = from pa in _context.ParticipacionesActivas
                                 .Include(p => p.Solicitud)
                                 .Include(p => p.ProgramaProyecto)
                                 orderby pa.Solicitud.Apellidos, pa.Solicitud.Nombres, pa.ProgramaProyecto.NombreProgramaProyecto
                                 select new
                                 {
                                   pa.ParticipacionID,
                                   DisplayText = (pa.Solicitud.Nombres + " " + pa.Solicitud.Apellidos ?? "Solicitante Desconocido") +
                                                   " - " +
                                                   (pa.ProgramaProyecto.NombreProgramaProyecto ?? "Programa Desconocido") +
                                                   " (Inicio: " + pa.FechaInicioParticipacion.ToString("dd/MM/yy") + ")"
                                 };
      ViewData["ParticipacionID"] = new SelectList(participacionesQuery.AsNoTracking(), "ParticipacionID", "DisplayText", selectedParticipacion);
    }

    // GET: EvaluacionesPrograma/Create
    public IActionResult Create()
    {
      PopulateParticipacionesDropDownList();
      var model = new EvaluacionesPrograma { FechaEvaluacion = DateTime.UtcNow };
      return View(model);
    }

    // POST: EvaluacionesPrograma/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("EvaluacionID,ParticipacionID,FechaEvaluacion,NombreProgramaUniversidadEvaluador,ParteMasGratificante,ParteMasDificil,RazonesOriginalesParticipacion,ExpectativasOriginalesCumplidas,InformacionPreviaUtil,EsfuerzoIntegracionComunidades,ComentariosAlojamientoHotel,ProgramaInmersionCulturalAyudoHumildad,ActividadesRecreativasCulturalesInteresantes,VisitaSitioComunidadFavoritaYPorQue,AspectoMasValiosoExperiencia,AplicaraLoAprendidoFuturo,TresCosasAprendidasSobreSiMismo,ComoCompartiraAprendidoUniversidad,RecomendariaProgramaOtros,QueDiraOtrosSobrePrograma,PermiteSerUsadoComoReferencia,ComentariosAdicionalesEvaluacion")] EvaluacionesPrograma evaluacionesPrograma)
    {
      ModelState.Remove("ParticipacionActiva");

      if (await _context.EvaluacionesPrograma.AnyAsync(e => e.ParticipacionID == evaluacionesPrograma.ParticipacionID && e.EvaluacionID != evaluacionesPrograma.EvaluacionID))
      {
        ModelState.AddModelError("ParticipacionID", "Ya existe una evaluación registrada para esta participación.");
      }

      if (ModelState.IsValid)
      {
        evaluacionesPrograma.FechaEvaluacion = DateTime.UtcNow;
        _context.Add(evaluacionesPrograma);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Evaluación de programa creada exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      PopulateParticipacionesDropDownList(evaluacionesPrograma.ParticipacionID);
      return View(evaluacionesPrograma);
    }

    // GET: EvaluacionesPrograma/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var evaluacionesPrograma = await _context.EvaluacionesPrograma.FindAsync(id);
      if (evaluacionesPrograma == null)
      {
        return NotFound();
      }
      PopulateParticipacionesDropDownList(evaluacionesPrograma.ParticipacionID);
      return View(evaluacionesPrograma);
    }

    // POST: EvaluacionesPrograma/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("EvaluacionID,ParticipacionID,FechaEvaluacion,NombreProgramaUniversidadEvaluador,ParteMasGratificante,ParteMasDificil,RazonesOriginalesParticipacion,ExpectativasOriginalesCumplidas,InformacionPreviaUtil,EsfuerzoIntegracionComunidades,ComentariosAlojamientoHotel,ProgramaInmersionCulturalAyudoHumildad,ActividadesRecreativasCulturalesInteresantes,VisitaSitioComunidadFavoritaYPorQue,AspectoMasValiosoExperiencia,AplicaraLoAprendidoFuturo,TresCosasAprendidasSobreSiMismo,ComoCompartiraAprendidoUniversidad,RecomendariaProgramaOtros,QueDiraOtrosSobrePrograma,PermiteSerUsadoComoReferencia,ComentariosAdicionalesEvaluacion")] EvaluacionesPrograma evaluacionesPrograma)
    {
      if (id != evaluacionesPrograma.EvaluacionID)
      {
        return NotFound();
      }

      ModelState.Remove("ParticipacionActiva");

      if (await _context.EvaluacionesPrograma.AnyAsync(e => e.ParticipacionID == evaluacionesPrograma.ParticipacionID && e.EvaluacionID != evaluacionesPrograma.EvaluacionID))
      {
        var originalParticipacionId = await _context.EvaluacionesPrograma.AsNoTracking().Where(e => e.EvaluacionID == id).Select(e => e.ParticipacionID).FirstOrDefaultAsync();
        if (originalParticipacionId != evaluacionesPrograma.ParticipacionID)
        {
          ModelState.AddModelError("ParticipacionID", "Ya existe una evaluación registrada para la nueva participación seleccionada.");
        }
      }

      if (ModelState.IsValid)
      {
        try
        {
          var originalEntity = await _context.EvaluacionesPrograma.AsNoTracking().FirstOrDefaultAsync(e => e.EvaluacionID == id);
          if (originalEntity != null)
          {
            evaluacionesPrograma.FechaEvaluacion = originalEntity.FechaEvaluacion;
          }

          _context.Update(evaluacionesPrograma);
          await _context.SaveChangesAsync();
          TempData["SuccessMessage"] = "Evaluación de programa actualizada exitosamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!EvaluacionesProgramaExists(evaluacionesPrograma.EvaluacionID))
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
      PopulateParticipacionesDropDownList(evaluacionesPrograma.ParticipacionID);
      return View(evaluacionesPrograma);
    }

    // GET: EvaluacionesPrograma/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var evaluacionesPrograma = await _context.EvaluacionesPrograma
          .Include(e => e.ParticipacionActiva)
              .ThenInclude(pa => pa!.Solicitud)
          .Include(e => e.ParticipacionActiva)
              .ThenInclude(pa => pa!.ProgramaProyecto)
          .FirstOrDefaultAsync(m => m.EvaluacionID == id);
      if (evaluacionesPrograma == null)
      {
        return NotFound();
      }

      return View(evaluacionesPrograma);
    }

    // POST: EvaluacionesPrograma/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var evaluacionesPrograma = await _context.EvaluacionesPrograma.FindAsync(id);
      if (evaluacionesPrograma != null)
      {
        _context.EvaluacionesPrograma.Remove(evaluacionesPrograma);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Evaluación de programa eliminada exitosamente.";
      }

      return RedirectToAction(nameof(Index));
    }

    private bool EvaluacionesProgramaExists(int id)
    {
      return _context.EvaluacionesPrograma.Any(e => e.EvaluacionID == id);
    }

    // ACCIÓN PARA EXPORTAR A PDF
    public async Task<IActionResult> ExportToPdf()
    {
      var evaluaciones = await _context.EvaluacionesPrograma
                                  .Include(e => e.ParticipacionActiva)
                                      .ThenInclude(pa => pa!.Solicitud)
                                  .Include(e => e.ParticipacionActiva)
                                      .ThenInclude(pa => pa!.ProgramaProyecto)
                                  .ToListAsync();

      // _webHostEnvironment se usa aquí. Debe estar correctamente inicializado.
      string wwwRootPath = _webHostEnvironment.WebRootPath;
      string logoPath = Path.Combine(wwwRootPath, "img", "logo_vncenter_mini.png");

      var document = new EvaluacionesProgramaPdfDocument(evaluaciones, logoPath);
      var pdfBytes = document.GeneratePdf();

      return File(pdfBytes, "application/pdf", $"Lista_EvaluacionesPrograma_{DateTime.Now:yyyyMMddHHmmss}.pdf");
    }

    // NUEVA ACCIÓN PARA EXPORTAR DETALLES A PDF
    public async Task<IActionResult> ExportDetailToPdf(int id)
    {
      var evaluacionPrograma = await _context.EvaluacionesPrograma
          .Include(e => e.ParticipacionActiva)
              .ThenInclude(pa => pa!.Solicitud) // Para el nombre del solicitante
          .Include(e => e.ParticipacionActiva)
              .ThenInclude(pa => pa!.ProgramaProyecto) // Para el nombre del programa
          .FirstOrDefaultAsync(m => m.EvaluacionID == id);

      if (evaluacionPrograma == null)
      {
        return NotFound();
      }

      string wwwRootPath = _webHostEnvironment.WebRootPath;
      string logoPath = Path.Combine(wwwRootPath, "img", "logo_vncenter_mini.png");

      var document = new EvaluacionProgramaDetailPdfDocument(evaluacionPrograma, logoPath);
      var pdfBytes = document.GeneratePdf();

      string participanteNombre = "Evaluacion"; // Default
      if (evaluacionPrograma.ParticipacionActiva?.Solicitud != null)
      {
        participanteNombre = $"{evaluacionPrograma.ParticipacionActiva.Solicitud.Nombres}_{evaluacionPrograma.ParticipacionActiva.Solicitud.Apellidos}".Replace(" ", "_");
      }

      return File(pdfBytes, "application/pdf", $"Detalle_Evaluacion_{participanteNombre}_ID{evaluacionPrograma.EvaluacionID}_{DateTime.Now:yyyyMMdd}.pdf");
    }
  }
}
