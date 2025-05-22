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
using QuestPDF.Fluent;
using VN_Center.Documents;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text;
using System.Collections.Generic;
using Microsoft.Extensions.Logging; // Asegúrate de tener el logger

namespace VN_Center.Controllers
{
  [Authorize]
  public class EvaluacionesProgramaController : Controller
  {
    private readonly VNCenterDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly UserManager<UsuariosSistema> _userManager;
    private readonly ILogger<EvaluacionesProgramaController> _logger; // Logger

    public EvaluacionesProgramaController(VNCenterDbContext context,
                                        IWebHostEnvironment webHostEnvironment,
                                        UserManager<UsuariosSistema> userManager,
                                        ILogger<EvaluacionesProgramaController> logger) // Inyectar Logger
    {
      _context = context;
      _webHostEnvironment = webHostEnvironment;
      _userManager = userManager;
      _logger = logger; // Asignar Logger
    }

    // GET: EvaluacionesPrograma
    public async Task<IActionResult> Index()
    {
      // Start with IQueryable, including necessary navigation properties
      IQueryable<EvaluacionesPrograma> query = _context.EvaluacionesPrograma
                                  .Include(e => e.ParticipacionActiva)
                                      .ThenInclude(pa => pa!.Solicitud)
                                  .Include(e => e.ParticipacionActiva)
                                      .ThenInclude(pa => pa!.ProgramaProyecto);

      // Apply conditional filtering based on user role
      if (!User.IsInRole("Administrador"))
      {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // A non-admin user sees evaluations they created OR evaluations for participations linked to their created requests.
        query = query.Where(e => e.UsuarioEvaluadorId == currentUserId ||
                             (e.ParticipacionActiva != null && e.ParticipacionActiva.Solicitud != null && e.ParticipacionActiva.Solicitud.UsuarioCreadorId == currentUserId));
      }

      // Apply ordering AFTER all filtering
      var orderedQuery = query.OrderByDescending(e => e.FechaEvaluacion);

      // Materialize the query
      return View(await orderedQuery.ToListAsync());
    }

    // GET: EvaluacionesPrograma/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var evaluacionPrograma = await _context.EvaluacionesPrograma
          .Include(e => e.ParticipacionActiva)
              .ThenInclude(pa => pa!.Solicitud)
          .Include(e => e.ParticipacionActiva)
              .ThenInclude(pa => pa!.ProgramaProyecto)
          .FirstOrDefaultAsync(m => m.EvaluacionID == id);

      if (evaluacionPrograma == null)
      {
        return NotFound();
      }

      // Authorization logic to view details
      if (!User.IsInRole("Administrador"))
      {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        bool canView = evaluacionPrograma.UsuarioEvaluadorId == currentUserId ||
                       (evaluacionPrograma.ParticipacionActiva?.Solicitud?.UsuarioCreadorId == currentUserId);

        if (!canView)
        {
          _logger.LogWarning($"Acceso denegado a Details de Evaluación ID: {id} por Usuario ID: {currentUserId}.");
          TempData["ErrorMessage"] = "No tiene permiso para ver los detalles de esta evaluación.";
          return RedirectToAction(nameof(Index));
        }
      }
      return View(evaluacionPrograma);
    }

    private async Task PopulateParticipacionesDropDownListAsync(object? selectedParticipacion = null)
    {
      // Start with IQueryable
      IQueryable<ParticipacionesActivas> participacionesQuery = _context.ParticipacionesActivas
                                                              .Include(p => p.Solicitud)
                                                              .Include(p => p.ProgramaProyecto)
                                                              .Where(pa => !pa.EvaluacionesPrograma.Any()); // Only participations without an evaluation

      // Apply conditional filtering
      if (!User.IsInRole("Administrador"))
      {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(currentUserId))
        {
          // A non-admin user can only create evaluations for participations from their own requests
          participacionesQuery = participacionesQuery.Where(p => p.Solicitud != null && p.Solicitud.UsuarioCreadorId == currentUserId);
        }
        else
        {
          participacionesQuery = participacionesQuery.Where(p => false); // Show nothing if UserID cannot be obtained
        }
      }

      // Apply ordering
      var orderedParticipacionesQuery = participacionesQuery.OrderBy(p => p.Solicitud!.Apellidos)
                                                            .ThenBy(p => p.Solicitud!.Nombres)
                                                            .ThenBy(p => p.ProgramaProyecto!.NombreProgramaProyecto);

      var participacionesList = await orderedParticipacionesQuery.Select(pa => new
      {
        pa.ParticipacionID,
        DisplayText = (pa.Solicitud!.Nombres + " " + pa.Solicitud.Apellidos ?? "Solicitante Desconocido") +
                                                    " - " +
                                                    (pa.ProgramaProyecto!.NombreProgramaProyecto ?? "Programa Desconocido") +
                                                    " (Inicio: " + pa.FechaInicioParticipacion.ToString("dd/MM/yy") + ")"
      }).ToListAsync();

      ViewData["ParticipacionID"] = new SelectList(participacionesList, "ParticipacionID", "DisplayText", selectedParticipacion);
    }

    // GET: EvaluacionesPrograma/Create
    public async Task<IActionResult> Create()
    {
      await PopulateParticipacionesDropDownListAsync();
      var model = new EvaluacionesPrograma { FechaEvaluacion = DateTime.UtcNow };
      return View(model);
    }

    // POST: EvaluacionesPrograma/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("EvaluacionID,ParticipacionID,NombreProgramaUniversidadEvaluador,ParteMasGratificante,ParteMasDificil,RazonesOriginalesParticipacion,ExpectativasOriginalesCumplidas,InformacionPreviaUtil,EsfuerzoIntegracionComunidades,ComentariosAlojamientoHotel,ProgramaInmersionCulturalAyudoHumildad,ActividadesRecreativasCulturalesInteresantes,VisitaSitioComunidadFavoritaYPorQue,AspectoMasValiosoExperiencia,AplicaraLoAprendidoFuturo,TresCosasAprendidasSobreSiMismo,ComoCompartiraAprendidoUniversidad,RecomendariaProgramaOtros,QueDiraOtrosSobrePrograma,PermiteSerUsadoComoReferencia,ComentariosAdicionalesEvaluacion")] EvaluacionesPrograma evaluacionesPrograma)
    {
      ModelState.Remove("ParticipacionActiva");

      var participacionSeleccionada = await _context.ParticipacionesActivas
          .Include(p => p.Solicitud)
          .FirstOrDefaultAsync(p => p.ParticipacionID == evaluacionesPrograma.ParticipacionID);

      if (participacionSeleccionada == null)
      {
        ModelState.AddModelError("ParticipacionID", "La participación seleccionada no es válida.");
      }
      else if (!User.IsInRole("Administrador"))
      {
        // A non-admin user can only create evaluations for participations from their own requests
        if (participacionSeleccionada.Solicitud?.UsuarioCreadorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
        {
          ModelState.AddModelError("ParticipacionID", "No tiene permiso para evaluar esta participación.");
        }
      }

      // Check if an evaluation already exists for this participation
      if (await _context.EvaluacionesPrograma.AnyAsync(e => e.ParticipacionID == evaluacionesPrograma.ParticipacionID && e.EvaluacionID != evaluacionesPrograma.EvaluacionID))
      {
        ModelState.AddModelError("ParticipacionID", "Ya existe una evaluación registrada para esta participación.");
      }

      if (ModelState.IsValid)
      {
        evaluacionesPrograma.FechaEvaluacion = DateTime.UtcNow;
        evaluacionesPrograma.UsuarioEvaluadorId = User.FindFirstValue(ClaimTypes.NameIdentifier); // The user creating the evaluation

        _context.Add(evaluacionesPrograma);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Evaluación de programa creada exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      await PopulateParticipacionesDropDownListAsync(evaluacionesPrograma.ParticipacionID);
      return View(evaluacionesPrograma);
    }

    // GET: EvaluacionesPrograma/Edit/5
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }
      var evaluacionPrograma = await _context.EvaluacionesPrograma
                                  .Include(e => e.ParticipacionActiva!.Solicitud)
                                  .Include(e => e.ParticipacionActiva!.ProgramaProyecto)
                                  .FirstOrDefaultAsync(e => e.EvaluacionID == id);
      if (evaluacionPrograma == null)
      {
        return NotFound();
      }
      // It's not typical to change the participation being evaluated during an edit.
      // If it were allowed, PopulateParticipacionesDropDownListAsync would be called here.
      return View(evaluacionPrograma);
    }

    // POST: EvaluacionesPrograma/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Edit(int id, [Bind("EvaluacionID,ParticipacionID,FechaEvaluacion,NombreProgramaUniversidadEvaluador,ParteMasGratificante,ParteMasDificil,RazonesOriginalesParticipacion,ExpectativasOriginalesCumplidas,InformacionPreviaUtil,EsfuerzoIntegracionComunidades,ComentariosAlojamientoHotel,ProgramaInmersionCulturalAyudoHumildad,ActividadesRecreativasCulturalesInteresantes,VisitaSitioComunidadFavoritaYPorQue,AspectoMasValiosoExperiencia,AplicaraLoAprendidoFuturo,TresCosasAprendidasSobreSiMismo,ComoCompartiraAprendidoUniversidad,RecomendariaProgramaOtros,QueDiraOtrosSobrePrograma,PermiteSerUsadoComoReferencia,ComentariosAdicionalesEvaluacion")] EvaluacionesPrograma evaluacionModificada)
    {
      if (id != evaluacionModificada.EvaluacionID)
      {
        return NotFound();
      }

      var evaluacionOriginal = await _context.EvaluacionesPrograma.AsNoTracking().FirstOrDefaultAsync(e => e.EvaluacionID == id);
      if (evaluacionOriginal == null)
      {
        return NotFound();
      }

      // Preserve original creator and creation date, and the participation link
      evaluacionModificada.UsuarioEvaluadorId = evaluacionOriginal.UsuarioEvaluadorId;
      evaluacionModificada.FechaEvaluacion = evaluacionOriginal.FechaEvaluacion;
      evaluacionModificada.ParticipacionID = evaluacionOriginal.ParticipacionID; // Participation should not be changed on edit

      ModelState.Remove("ParticipacionActiva"); // Navigation property

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(evaluacionModificada);
          await _context.SaveChangesAsync();
          TempData["SuccessMessage"] = "Evaluación de programa actualizada exitosamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!EvaluacionesProgramaExists(evaluacionModificada.EvaluacionID))
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
      // If model is not valid, repopulate necessary data for the view if any (e.g., dropdowns if they were editable)
      return View(evaluacionModificada);
    }

    // GET: EvaluacionesPrograma/Delete/5
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }
      var evaluacionPrograma = await _context.EvaluacionesPrograma
          .Include(e => e.ParticipacionActiva!.Solicitud)
          .Include(e => e.ParticipacionActiva!.ProgramaProyecto)
          .FirstOrDefaultAsync(m => m.EvaluacionID == id);
      if (evaluacionPrograma == null)
      {
        return NotFound();
      }
      return View(evaluacionPrograma);
    }

    // POST: EvaluacionesPrograma/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var evaluacionPrograma = await _context.EvaluacionesPrograma.FindAsync(id);
      if (evaluacionPrograma != null)
      {
        _context.EvaluacionesPrograma.Remove(evaluacionPrograma);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Evaluación de programa eliminada exitosamente.";
      }
      else
      {
        TempData["ErrorMessage"] = "La evaluación no fue encontrada.";
      }
      return RedirectToAction(nameof(Index));
    }

    private bool EvaluacionesProgramaExists(int id)
    {
      return _context.EvaluacionesPrograma.Any(e => e.EvaluacionID == id);
    }

    // ACCIÓN PARA EXPORTAR LISTA A PDF
    public async Task<IActionResult> ExportToPdf()
    {
      // Start with IQueryable
      IQueryable<EvaluacionesPrograma> query = _context.EvaluacionesPrograma
                                  .Include(e => e.ParticipacionActiva)
                                      .ThenInclude(pa => pa!.Solicitud)
                                  .Include(e => e.ParticipacionActiva)
                                      .ThenInclude(pa => pa!.ProgramaProyecto);

      // Apply conditional filtering
      if (!User.IsInRole("Administrador"))
      {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(currentUserId))
        {
          // A non-admin user exports a list of evaluations they created OR evaluations for participations linked to their created requests.
          query = query.Where(e => e.UsuarioEvaluadorId == currentUserId ||
                              (e.ParticipacionActiva != null && e.ParticipacionActiva.Solicitud != null && e.ParticipacionActiva.Solicitud.UsuarioCreadorId == currentUserId));
        }
        else
        {
          query = query.Where(e => false); // Show nothing if UserID cannot be obtained
        }
      }

      // Apply ordering
      var orderedQuery = query.OrderByDescending(e => e.FechaEvaluacion);
      var evaluaciones = await orderedQuery.ToListAsync();

      string wwwRootPath = _webHostEnvironment.WebRootPath;
      string logoPath = Path.Combine(wwwRootPath, "img", "logo_vncenter_mini.png");

      var document = new EvaluacionesProgramaPdfDocument(evaluaciones, logoPath);
      var pdfBytes = document.GeneratePdf();

      return File(pdfBytes, "application/pdf", $"Lista_EvaluacionesPrograma_{DateTime.Now:yyyyMMddHHmmss}.pdf");
    }

    // ACCIÓN PARA EXPORTAR DETALLES A PDF
    public async Task<IActionResult> ExportDetailToPdf(int id)
    {
      _logger.LogInformation("Iniciando ExportDetailToPdf para Evaluación ID: {EvaluacionID}", id);
      var evaluacionPrograma = await _context.EvaluacionesPrograma
          .Include(e => e.ParticipacionActiva)
              .ThenInclude(pa => pa!.Solicitud)
          .Include(e => e.ParticipacionActiva)
              .ThenInclude(pa => pa!.ProgramaProyecto)
          .FirstOrDefaultAsync(m => m.EvaluacionID == id);

      if (evaluacionPrograma == null)
      {
        _logger.LogWarning("Evaluación con ID: {EvaluacionID} no encontrada para exportar.", id);
        return NotFound();
      }

      // Permission logic
      if (!User.IsInRole("Administrador"))
      {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // A non-admin user can export if they created the evaluation OR if they created the original request for the evaluated participation.
        bool canExport = evaluacionPrograma.UsuarioEvaluadorId == currentUserId ||
                         (evaluacionPrograma.ParticipacionActiva?.Solicitud?.UsuarioCreadorId == currentUserId);

        _logger.LogInformation("[DIAGNÓSTICO PERMISOS EvaluacionesPrograma.ExportDetailToPdf ID: {EvaluacionID}] isAdmin: false, Evaluacion.UsuarioEvaluadorId: {evaluadorId}, Solicitud.UsuarioCreadorId: {creadorSolicitudId}, CurrentUserId: {currentUserId}, CanExport: {canExport}",
            id, evaluacionPrograma.UsuarioEvaluadorId, evaluacionPrograma.ParticipacionActiva?.Solicitud?.UsuarioCreadorId, currentUserId, canExport);

        if (!canExport)
        {
          _logger.LogWarning("Acceso denegado a ExportDetailToPdf para Evaluación ID: {EvaluacionID} por Usuario ID: {CurrentUserId}. No es admin, ni evaluador, ni creador de la solicitud.", id, currentUserId);
          TempData["ErrorMessage"] = "No tiene permiso para exportar los detalles de esta evaluación.";
          return RedirectToAction(nameof(Index));
        }
      }
      else
      {
        _logger.LogInformation("[DIAGNÓSTICO PERMISOS EvaluacionesPrograma.ExportDetailToPdf ID: {EvaluacionID}] Usuario es Administrador. Permiso concedido.", id);
      }

      _logger.LogInformation("Permiso concedido para exportar Evaluación ID: {EvaluacionID}", id);
      string wwwRootPath = _webHostEnvironment.WebRootPath;
      string logoPath = Path.Combine(wwwRootPath, "img", "logo_vncenter_mini.png");

      var document = new EvaluacionProgramaDetailPdfDocument(evaluacionPrograma, logoPath);
      var pdfBytes = document.GeneratePdf();

      string participanteNombre = "Evaluacion";
      if (evaluacionPrograma.ParticipacionActiva?.Solicitud != null)
      {
        participanteNombre = $"{evaluacionPrograma.ParticipacionActiva.Solicitud.Nombres}_{evaluacionPrograma.ParticipacionActiva.Solicitud.Apellidos}".Replace(" ", "_");
      }

      return File(pdfBytes, "application/pdf", $"Detalle_Evaluacion_{participanteNombre}_ID{evaluacionPrograma.EvaluacionID}_{DateTime.Now:yyyyMMdd}.pdf");
    }
  }
}
