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
// using VN_Center.Models.ViewModels; // No es necesario para este controlador con la lógica actual

namespace VN_Center.Controllers
{
  [Authorize]
  public class EvaluacionesProgramaController : Controller
  {
    private readonly VNCenterDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly UserManager<UsuariosSistema> _userManager;

    public EvaluacionesProgramaController(VNCenterDbContext context,
                                        IWebHostEnvironment webHostEnvironment,
                                        UserManager<UsuariosSistema> userManager)
    {
      _context = context;
      _webHostEnvironment = webHostEnvironment;
      _userManager = userManager;
    }

    // GET: EvaluacionesPrograma
    // Todos los usuarios autenticados pueden ver todas las evaluaciones
    public async Task<IActionResult> Index()
    {
      var query = _context.EvaluacionesPrograma
                                  .Include(e => e.ParticipacionActiva)
                                      .ThenInclude(pa => pa!.Solicitud)
                                  .Include(e => e.ParticipacionActiva)
                                      .ThenInclude(pa => pa!.ProgramaProyecto)
                                  .OrderByDescending(e => e.FechaEvaluacion);
      // No se aplica filtro por UsuarioCreadorId de la solicitud aquí
      return View(await query.ToListAsync());
    }

    // GET: EvaluacionesPrograma/Details/5
    // Todos los usuarios autenticados pueden ver los detalles de cualquier evaluación
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
      // No se aplica filtro por UsuarioCreadorId de la solicitud aquí
      return View(evaluacionPrograma);
    }

    private async Task PopulateParticipacionesDropDownListAsync(object? selectedParticipacion = null)
    {
      IQueryable<ParticipacionesActivas> participacionesQuery = _context.ParticipacionesActivas
                                                              .Include(p => p.Solicitud)
                                                              .Include(p => p.ProgramaProyecto)
                                                              // Solo participaciones que aún no tienen una evaluación
                                                              .Where(pa => !pa.EvaluacionesPrograma.Any())
                                                              .OrderBy(p => p.Solicitud.Apellidos)
                                                              .ThenBy(p => p.Solicitud.Nombres)
                                                              .ThenBy(p => p.ProgramaProyecto.NombreProgramaProyecto);

      if (!User.IsInRole("Administrador"))
      {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(currentUserId))
        {
          // Filtrar participaciones donde la solicitud asociada fue creada por el usuario actual
          participacionesQuery = participacionesQuery.Where(p => p.Solicitud.UsuarioCreadorId == currentUserId);
        }
        else
        {
          participacionesQuery = participacionesQuery.Where(p => false); // No mostrar nada
        }
      }

      var participacionesList = await participacionesQuery.Select(pa => new
      {
        pa.ParticipacionID,
        DisplayText = (pa.Solicitud.Nombres + " " + pa.Solicitud.Apellidos ?? "Solicitante Desconocido") +
                                                    " - " +
                                                    (pa.ProgramaProyecto.NombreProgramaProyecto ?? "Programa Desconocido") +
                                                    " (Inicio: " + pa.FechaInicioParticipacion.ToString("dd/MM/yy") + ")"
      }).ToListAsync();

      ViewData["ParticipacionID"] = new SelectList(participacionesList, "ParticipacionID", "DisplayText", selectedParticipacion);
    }

    // GET: EvaluacionesPrograma/Create
    // Todos los usuarios autenticados pueden acceder
    public async Task<IActionResult> Create()
    {
      await PopulateParticipacionesDropDownListAsync();
      var model = new EvaluacionesPrograma { FechaEvaluacion = DateTime.UtcNow };
      return View(model);
    }

    // POST: EvaluacionesPrograma/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    // Todos los usuarios autenticados pueden crear
    public async Task<IActionResult> Create([Bind("EvaluacionID,ParticipacionID,NombreProgramaUniversidadEvaluador,ParteMasGratificante,ParteMasDificil,RazonesOriginalesParticipacion,ExpectativasOriginalesCumplidas,InformacionPreviaUtil,EsfuerzoIntegracionComunidades,ComentariosAlojamientoHotel,ProgramaInmersionCulturalAyudoHumildad,ActividadesRecreativasCulturalesInteresantes,VisitaSitioComunidadFavoritaYPorQue,AspectoMasValiosoExperiencia,AplicaraLoAprendidoFuturo,TresCosasAprendidasSobreSiMismo,ComoCompartiraAprendidoUniversidad,RecomendariaProgramaOtros,QueDiraOtrosSobrePrograma,PermiteSerUsadoComoReferencia,ComentariosAdicionalesEvaluacion")] EvaluacionesPrograma evaluacionesPrograma)
    {
      ModelState.Remove("ParticipacionActiva");
      // ModelState.Remove("UsuarioEvaluador"); // Si añades la propiedad de navegación

      // Validar que la participación seleccionada pertenezca al usuario (si no es admin)
      // O que el admin esté creando para cualquier participación
      var participacionSeleccionada = await _context.ParticipacionesActivas
          .Include(p => p.Solicitud)
          .FirstOrDefaultAsync(p => p.ParticipacionID == evaluacionesPrograma.ParticipacionID);

      if (participacionSeleccionada == null)
      {
        ModelState.AddModelError("ParticipacionID", "La participación seleccionada no es válida.");
      }
      else if (!User.IsInRole("Administrador"))
      {
        if (participacionSeleccionada.Solicitud?.UsuarioCreadorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
        {
          ModelState.AddModelError("ParticipacionID", "No tiene permiso para evaluar esta participación.");
        }
      }

      // Verificar si ya existe una evaluación para esta participación (esta lógica es correcta)
      if (await _context.EvaluacionesPrograma.AnyAsync(e => e.ParticipacionID == evaluacionesPrograma.ParticipacionID && e.EvaluacionID != evaluacionesPrograma.EvaluacionID))
      {
        ModelState.AddModelError("ParticipacionID", "Ya existe una evaluación registrada para esta participación.");
      }

      if (ModelState.IsValid)
      {
        evaluacionesPrograma.FechaEvaluacion = DateTime.UtcNow;
        evaluacionesPrograma.UsuarioEvaluadorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        _context.Add(evaluacionesPrograma);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Evaluación de programa creada exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      await PopulateParticipacionesDropDownListAsync(evaluacionesPrograma.ParticipacionID);
      return View(evaluacionesPrograma);
    }

    // GET: EvaluacionesPrograma/Edit/5
    [Authorize(Roles = "Administrador")] // Solo Administradores
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }
      var evaluacionPrograma = await _context.EvaluacionesPrograma
                                  .Include(e => e.ParticipacionActiva.Solicitud) // Para mostrar info en la vista
                                  .Include(e => e.ParticipacionActiva.ProgramaProyecto) // Para mostrar info en la vista
                                  .FirstOrDefaultAsync(e => e.EvaluacionID == id);
      if (evaluacionPrograma == null)
      {
        return NotFound();
      }
      // No se puede cambiar la participación en la edición, así que no es necesario repoblar el dropdown aquí.
      // Si se permitiera cambiar, se llamaría a PopulateParticipacionesDropDownListAsync.
      return View(evaluacionPrograma);
    }

    // POST: EvaluacionesPrograma/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")] // Solo Administradores
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
      // Preservar UsuarioEvaluadorId, FechaEvaluacion original y ParticipacionID original
      evaluacionModificada.UsuarioEvaluadorId = evaluacionOriginal.UsuarioEvaluadorId;
      evaluacionModificada.FechaEvaluacion = evaluacionOriginal.FechaEvaluacion;
      evaluacionModificada.ParticipacionID = evaluacionOriginal.ParticipacionID; // No permitir cambio de participación en la edición

      ModelState.Remove("ParticipacionActiva");

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
      // Si el modelo no es válido, repoblar lo necesario (aunque ParticipacionID no se cambia)
      // await PopulateParticipacionesDropDownListAsync(evaluacionModificada.ParticipacionID); // No es necesario si ParticipacionID no se edita
      return View(evaluacionModificada);
    }

    // GET: EvaluacionesPrograma/Delete/5
    [Authorize(Roles = "Administrador")] // Solo Administradores
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }
      var evaluacionPrograma = await _context.EvaluacionesPrograma
          .Include(e => e.ParticipacionActiva.Solicitud)
          .Include(e => e.ParticipacionActiva.ProgramaProyecto)
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
    [Authorize(Roles = "Administrador")] // Solo Administradores
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
    // Todos los usuarios autenticados pueden exportar la lista que ven
    public async Task<IActionResult> ExportToPdf()
    {
      IQueryable<EvaluacionesPrograma> query = _context.EvaluacionesPrograma
                                  .Include(e => e.ParticipacionActiva)
                                      .ThenInclude(pa => pa!.Solicitud)
                                  .Include(e => e.ParticipacionActiva)
                                      .ThenInclude(pa => pa!.ProgramaProyecto)
                                  .OrderByDescending(e => e.FechaEvaluacion);

      if (!User.IsInRole("Administrador"))
      {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(currentUserId))
        {
          query = query.Where(e => e.ParticipacionActiva.Solicitud.UsuarioCreadorId == currentUserId);
        }
        else
        {
          query = query.Where(e => false);
        }
      }
      var evaluaciones = await query.ToListAsync();

      string wwwRootPath = _webHostEnvironment.WebRootPath;
      string logoPath = Path.Combine(wwwRootPath, "img", "logo_vncenter_mini.png");

      var document = new EvaluacionesProgramaPdfDocument(evaluaciones, logoPath);
      var pdfBytes = document.GeneratePdf();

      return File(pdfBytes, "application/pdf", $"Lista_EvaluacionesPrograma_{DateTime.Now:yyyyMMddHHmmss}.pdf");
    }

    // ACCIÓN PARA EXPORTAR DETALLES A PDF
    // Todos los usuarios autenticados pueden exportar el detalle si pueden verlo
    public async Task<IActionResult> ExportDetailToPdf(int id)
    {
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

      if (!User.IsInRole("Administrador"))
      {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (evaluacionPrograma.ParticipacionActiva?.Solicitud?.UsuarioCreadorId != currentUserId)
        {
          TempData["ErrorMessage"] = "No tiene permiso para exportar los detalles de esta evaluación.";
          return RedirectToAction(nameof(Index));
        }
      }

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
