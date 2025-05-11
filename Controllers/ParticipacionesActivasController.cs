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
  public class ParticipacionesActivasController : Controller
  {
    private readonly VNCenterDbContext _context;

    public ParticipacionesActivasController(VNCenterDbContext context)
    {
      _context = context;
    }

    // GET: ParticipacionesActivas
    public async Task<IActionResult> Index()
    {
      var vNCenterDbContext = _context.ParticipacionesActivas
                                      .Include(p => p.ProgramaProyecto)
                                      .Include(p => p.Solicitud)
                                          .ThenInclude(s => s!.NivelesIdioma) // Ejemplo de incluir una relación anidada si fuera necesario para mostrar más info del solicitante
                                      .Include(p => p.Solicitud)
                                          .ThenInclude(s => s!.FuentesConocimiento);
      return View(await vNCenterDbContext.OrderByDescending(p => p.FechaAsignacion).ToListAsync());
    }

    // GET: ParticipacionesActivas/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var participacionesActivas = await _context.ParticipacionesActivas
          .Include(p => p.ProgramaProyecto)
          .Include(p => p.Solicitud)
          .FirstOrDefaultAsync(m => m.ParticipacionID == id);
      if (participacionesActivas == null)
      {
        return NotFound();
      }

      return View(participacionesActivas);
    }

    private void PopulateDropdowns(object? selectedSolicitud = null, object? selectedPrograma = null)
    {
      var solicitudesQuery = from s in _context.Solicitudes
                               // Podrías filtrar aquí por solicitudes aprobadas si tienes ese estado
                               // where s.EstadoSolicitud == "Aprobada" 
                             orderby s.Apellidos, s.Nombres
                             select new
                             {
                               s.SolicitudID,
                               NombreCompletoSolicitante = s.Nombres + " " + s.Apellidos + " (ID: " + s.SolicitudID + ")"
                             };
      ViewData["SolicitudID"] = new SelectList(solicitudesQuery.AsNoTracking(), "SolicitudID", "NombreCompletoSolicitante", selectedSolicitud);

      var programasQuery = from p in _context.ProgramasProyectosONG
                             // Podrías filtrar por programas activos o en curso
                             // where p.EstadoProgramaProyecto == "En Curso"
                           orderby p.NombreProgramaProyecto
                           select p;
      ViewData["ProgramaProyectoID"] = new SelectList(programasQuery.AsNoTracking(), "ProgramaProyectoID", "NombreProgramaProyecto", selectedPrograma);
    }

    // GET: ParticipacionesActivas/Create
    public IActionResult Create()
    {
      PopulateDropdowns();
      // Asignar FechaAsignacion por defecto al día actual
      var model = new ParticipacionesActivas
      {
        FechaAsignacion = DateTime.Today,
        FechaInicioParticipacion = DateTime.Today // Opcional, el usuario puede cambiarla
      };
      return View(model);
    }

    // POST: ParticipacionesActivas/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ParticipacionID,SolicitudID,ProgramaProyectoID,FechaAsignacion,FechaInicioParticipacion,FechaFinParticipacion,RolDesempenado,HorasTCUCompletadas,NotasSupervisor")] ParticipacionesActivas participacionesActivas)
    {
      ModelState.Remove("Solicitud");
      ModelState.Remove("ProgramaProyecto");
      ModelState.Remove("EvaluacionesPrograma");

      if (ModelState.IsValid)
      {
        _context.Add(participacionesActivas);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Participación activa creada exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      PopulateDropdowns(participacionesActivas.SolicitudID, participacionesActivas.ProgramaProyectoID);
      return View(participacionesActivas);
    }

    // GET: ParticipacionesActivas/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var participacionesActivas = await _context.ParticipacionesActivas.FindAsync(id);
      if (participacionesActivas == null)
      {
        return NotFound();
      }
      PopulateDropdowns(participacionesActivas.SolicitudID, participacionesActivas.ProgramaProyectoID);
      return View(participacionesActivas);
    }

    // POST: ParticipacionesActivas/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("ParticipacionID,SolicitudID,ProgramaProyectoID,FechaAsignacion,FechaInicioParticipacion,FechaFinParticipacion,RolDesempenado,HorasTCUCompletadas,NotasSupervisor")] ParticipacionesActivas participacionesActivas)
    {
      if (id != participacionesActivas.ParticipacionID)
      {
        return NotFound();
      }

      ModelState.Remove("Solicitud");
      ModelState.Remove("ProgramaProyecto");
      ModelState.Remove("EvaluacionesPrograma");

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(participacionesActivas);
          await _context.SaveChangesAsync();
          TempData["SuccessMessage"] = "Participación activa actualizada exitosamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!ParticipacionesActivasExists(participacionesActivas.ParticipacionID))
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
      PopulateDropdowns(participacionesActivas.SolicitudID, participacionesActivas.ProgramaProyectoID);
      return View(participacionesActivas);
    }

    // GET: ParticipacionesActivas/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var participacionesActivas = await _context.ParticipacionesActivas
          .Include(p => p.ProgramaProyecto)
          .Include(p => p.Solicitud)
          .FirstOrDefaultAsync(m => m.ParticipacionID == id);
      if (participacionesActivas == null)
      {
        return NotFound();
      }

      return View(participacionesActivas);
    }

    // POST: ParticipacionesActivas/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var participacionesActivas = await _context.ParticipacionesActivas.FindAsync(id);
      if (participacionesActivas != null)
      {
        // Considerar si hay evaluaciones asociadas antes de eliminar
        bool tieneEvaluaciones = await _context.EvaluacionesPrograma.AnyAsync(e => e.ParticipacionID == id);
        if (tieneEvaluaciones)
        {
          TempData["ErrorMessage"] = "Esta participación no se puede eliminar porque tiene evaluaciones asociadas. Elimine primero las evaluaciones.";
          return RedirectToAction(nameof(Index));
        }
        _context.ParticipacionesActivas.Remove(participacionesActivas);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Participación activa eliminada exitosamente.";
      }

      return RedirectToAction(nameof(Index));
    }

    private bool ParticipacionesActivasExists(int id)
    {
      return _context.ParticipacionesActivas.Any(e => e.ParticipacionID == id);
    }
  }
}
