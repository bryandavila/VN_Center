using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VN_Center.Data;
using VN_Center.Models.Entities;

namespace VN_Center.Controllers
{
  public class EvaluacionesProgramaController : Controller
  {
    private readonly VNCenterDbContext _context;

    public EvaluacionesProgramaController(VNCenterDbContext context)
    {
      _context = context;
    }

    // GET: EvaluacionesPrograma
    public async Task<IActionResult> Index()
    {
      var vNCenterDbContext = _context.EvaluacionesPrograma
                                      .Include(e => e.ParticipacionActiva)
                                          .ThenInclude(pa => pa!.Solicitud) // Encadenar Include
                                      .Include(e => e.ParticipacionActiva)
                                          .ThenInclude(pa => pa!.ProgramaProyecto); // Encadenar Include
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
                                   // Opcional: Filtrar participaciones que aún no tienen evaluación
                                   // where !_context.EvaluacionesPrograma.Any(e => e.ParticipacionID == pa.ParticipacionID)
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

      // Verificar si ya existe una evaluación para esta participación
      if (await _context.EvaluacionesPrograma.AnyAsync(e => e.ParticipacionID == evaluacionesPrograma.ParticipacionID && e.EvaluacionID != evaluacionesPrograma.EvaluacionID))
      {
        ModelState.AddModelError("ParticipacionID", "Ya existe una evaluación registrada para esta participación.");
      }


      if (ModelState.IsValid)
      {
        evaluacionesPrograma.FechaEvaluacion = DateTime.UtcNow; // Asegurar fecha actual
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

      // Verificar si se está cambiando ParticipacionID a una que ya tiene evaluación
      if (await _context.EvaluacionesPrograma.AnyAsync(e => e.ParticipacionID == evaluacionesPrograma.ParticipacionID && e.EvaluacionID != evaluacionesPrograma.EvaluacionID))
      {
        var originalParticipacionId = await _context.EvaluacionesPrograma.AsNoTracking().Where(e => e.EvaluacionID == id).Select(e => e.ParticipacionID).FirstOrDefaultAsync();
        if (originalParticipacionId != evaluacionesPrograma.ParticipacionID) // Solo si el ParticipacionID ha cambiado
        {
          ModelState.AddModelError("ParticipacionID", "Ya existe una evaluación registrada para la nueva participación seleccionada.");
        }
      }

      if (ModelState.IsValid)
      {
        try
        {
          // Preservar la fecha de evaluación original si no se permite editar
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
  }
}
