// VN_Center/Controllers/ParticipacionesActivasController.cs
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
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace VN_Center.Controllers
{
  [Authorize]
  public class ParticipacionesActivasController : Controller
  {
    private readonly VNCenterDbContext _context;
    private readonly UserManager<UsuariosSistema> _userManager;

    public ParticipacionesActivasController(VNCenterDbContext context, UserManager<UsuariosSistema> userManager)
    {
      _context = context;
      _userManager = userManager;
    }

    // GET: ParticipacionesActivas
    public async Task<IActionResult> Index()
    {
      IQueryable<ParticipacionesActivas> query = _context.ParticipacionesActivas
                                                      .Include(p => p.ProgramaProyecto)
                                                      .Include(p => p.Solicitud) // Incluimos la Solicitud para acceder a UsuarioCreadorId
                                                      .OrderByDescending(p => p.FechaAsignacion);

      if (!User.IsInRole("Administrador"))
      {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // ID del usuario del sistema logueado
        if (!string.IsNullOrEmpty(currentUserId))
        {
          // Filtrar las participaciones donde la Solicitud asociada fue creada por el usuario actual
          query = query.Where(p => p.Solicitud.UsuarioCreadorId == currentUserId);
        }
        else
        {
          query = query.Where(p => false); // No mostrar nada si no se puede obtener el UserID
        }
      }
      return View(await query.ToListAsync());
    }

    // GET: ParticipacionesActivas/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var participacionActiva = await _context.ParticipacionesActivas
          .Include(p => p.ProgramaProyecto)
          .Include(p => p.Solicitud) // Incluir Solicitud para verificar UsuarioCreadorId
          .FirstOrDefaultAsync(m => m.ParticipacionID == id);

      if (participacionActiva == null)
      {
        return NotFound();
      }

      if (!User.IsInRole("Administrador"))
      {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // Verificar si la Solicitud asociada a esta participación fue creada por el usuario actual
        if (participacionActiva.Solicitud?.UsuarioCreadorId != currentUserId)
        {
          TempData["ErrorMessage"] = "No tiene permiso para ver los detalles de esta participación.";
          return RedirectToAction(nameof(Index));
        }
      }
      return View(participacionActiva);
    }

    private void PopulateDropdowns(object? selectedSolicitud = null, object? selectedPrograma = null)
    {
      // Al poblar solicitudes, si el usuario no es admin, solo debería ver las que él creó para asignar.
      // O, si la lógica es que un admin asigna, entonces este dropdown no necesita filtrarse aquí.
      // Por ahora, se muestran todas las solicitudes para simplificar, asumiendo que un admin crea la participación.
      // Si un usuario no-admin pudiera crear participaciones (lo cual dijiste que no), habría que filtrar aquí también.
      var solicitudesQuery = from s in _context.Solicitudes
                             orderby s.Apellidos, s.Nombres
                             select new
                             {
                               s.SolicitudID,
                               NombreCompletoSolicitante = s.Nombres + " " + s.Apellidos + " (ID: " + s.SolicitudID + ")"
                             };
      ViewData["SolicitudID"] = new SelectList(solicitudesQuery.AsNoTracking(), "SolicitudID", "NombreCompletoSolicitante", selectedSolicitud);

      var programasQuery = from p in _context.ProgramasProyectosONG
                           orderby p.NombreProgramaProyecto
                           select p;
      ViewData["ProgramaProyectoID"] = new SelectList(programasQuery.AsNoTracking(), "ProgramaProyectoID", "NombreProgramaProyecto", selectedPrograma);
    }

    // GET: ParticipacionesActivas/Create
    [Authorize(Roles = "Administrador")]
    public IActionResult Create()
    {
      PopulateDropdowns();
      var model = new ParticipacionesActivas
      {
        FechaAsignacion = DateTime.Today,
        FechaInicioParticipacion = DateTime.Today
      };
      return View(model);
    }

    // POST: ParticipacionesActivas/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Create([Bind("ParticipacionID,SolicitudID,ProgramaProyectoID,FechaAsignacion,FechaInicioParticipacion,FechaFinParticipacion,RolDesempenado,HorasTCUCompletadas,NotasSupervisor")] ParticipacionesActivas participacionesActivas)
    {
      ModelState.Remove("Solicitud");
      ModelState.Remove("ProgramaProyecto");
      ModelState.Remove("EvaluacionesPrograma");
      // ModelState.Remove("UsuarioAsignador"); // Si añades propiedad de navegación

      if (ModelState.IsValid)
      {
        // El UsuarioAsignadorId es el administrador que está creando esta participación
        participacionesActivas.UsuarioAsignadorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        _context.Add(participacionesActivas);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Participación activa creada exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      PopulateDropdowns(participacionesActivas.SolicitudID, participacionesActivas.ProgramaProyectoID);
      return View(participacionesActivas);
    }

    // GET: ParticipacionesActivas/Edit/5
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var participacionActiva = await _context.ParticipacionesActivas.FindAsync(id);
      if (participacionActiva == null)
      {
        return NotFound();
      }
      PopulateDropdowns(participacionActiva.SolicitudID, participacionActiva.ProgramaProyectoID);
      return View(participacionActiva);
    }

    // POST: ParticipacionesActivas/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Edit(int id, [Bind("ParticipacionID,SolicitudID,ProgramaProyectoID,FechaAsignacion,FechaInicioParticipacion,FechaFinParticipacion,RolDesempenado,HorasTCUCompletadas,NotasSupervisor")] ParticipacionesActivas participacionModificada)
    {
      if (id != participacionModificada.ParticipacionID)
      {
        return NotFound();
      }

      var participacionOriginal = await _context.ParticipacionesActivas.AsNoTracking().FirstOrDefaultAsync(p => p.ParticipacionID == id);
      if (participacionOriginal == null)
      {
        return NotFound();
      }
      // Preservar el UsuarioAsignadorId original
      participacionModificada.UsuarioAsignadorId = participacionOriginal.UsuarioAsignadorId;

      ModelState.Remove("Solicitud");
      ModelState.Remove("ProgramaProyecto");
      ModelState.Remove("EvaluacionesPrograma");

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(participacionModificada);
          await _context.SaveChangesAsync();
          TempData["SuccessMessage"] = "Participación activa actualizada exitosamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!ParticipacionesActivasExists(participacionModificada.ParticipacionID))
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
      PopulateDropdowns(participacionModificada.SolicitudID, participacionModificada.ProgramaProyectoID);
      return View(participacionModificada);
    }

    // GET: ParticipacionesActivas/Delete/5
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var participacionActiva = await _context.ParticipacionesActivas
          .Include(p => p.ProgramaProyecto)
          .Include(p => p.Solicitud)
          .FirstOrDefaultAsync(m => m.ParticipacionID == id);
      if (participacionActiva == null)
      {
        return NotFound();
      }
      return View(participacionActiva);
    }

    // POST: ParticipacionesActivas/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var participacionActiva = await _context.ParticipacionesActivas.FindAsync(id);
      if (participacionActiva != null)
      {
        bool tieneEvaluaciones = await _context.EvaluacionesPrograma.AnyAsync(e => e.ParticipacionID == id);
        if (tieneEvaluaciones)
        {
          TempData["ErrorMessage"] = "Esta participación no se puede eliminar porque tiene evaluaciones asociadas. Elimine primero las evaluaciones.";
          return RedirectToAction(nameof(Index));
        }
        _context.ParticipacionesActivas.Remove(participacionActiva);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Participación activa eliminada exitosamente.";
      }
      else
      {
        TempData["ErrorMessage"] = "La participación no fue encontrada.";
      }
      return RedirectToAction(nameof(Index));
    }

    private bool ParticipacionesActivasExists(int id)
    {
      return _context.ParticipacionesActivas.Any(e => e.ParticipacionID == id);
    }
  }
}
