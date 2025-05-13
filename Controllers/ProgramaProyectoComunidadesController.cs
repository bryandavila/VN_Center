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

namespace VN_Center.Controllers
{
  [Authorize]
  public class ProgramaProyectoComunidadesController : Controller
  {
    private readonly VNCenterDbContext _context;

    public ProgramaProyectoComunidadesController(VNCenterDbContext context)
    {
      _context = context;
    }

    // GET: ProgramaProyectoComunidades
    public async Task<IActionResult> Index()
    {
      var vNCenterDbContext = _context.ProgramaProyectoComunidades
                                          .Include(p => p.Comunidad)
                                          .Include(p => p.ProgramaProyecto)
                                          .OrderBy(p => p.ProgramaProyecto.NombreProgramaProyecto)
                                          .ThenBy(p => p.Comunidad.NombreComunidad);
      return View(await vNCenterDbContext.ToListAsync());
    }

    // GET: ProgramaProyectoComunidades/Details/5/10
    public async Task<IActionResult> Details(int? programaProyectoId, int? comunidadId)
    {
      if (programaProyectoId == null || comunidadId == null)
      {
        return NotFound();
      }

      var programaProyectoComunidades = await _context.ProgramaProyectoComunidades
          .Include(p => p.Comunidad)
          .Include(p => p.ProgramaProyecto)
          .FirstOrDefaultAsync(m => m.ProgramaProyectoID == programaProyectoId && m.ComunidadID == comunidadId);
      if (programaProyectoComunidades == null)
      {
        return NotFound();
      }

      return View(programaProyectoComunidades);
    }

    private void PopulateProgramasProyectosDropDownList(object? selectedPrograma = null)
    {
      var programasQuery = from p in _context.ProgramasProyectosONG
                           orderby p.NombreProgramaProyecto
                           select p;
      ViewData["ProgramaProyectoID"] = new SelectList(programasQuery.AsNoTracking(), "ProgramaProyectoID", "NombreProgramaProyecto", selectedPrograma);
    }

    private void PopulateComunidadesDropDownList(object? selectedComunidad = null)
    {
      var comunidadesQuery = from c in _context.Comunidades
                             orderby c.NombreComunidad
                             select c;
      ViewData["ComunidadID"] = new SelectList(comunidadesQuery.AsNoTracking(), "ComunidadID", "NombreComunidad", selectedComunidad);
    }

    // GET: ProgramaProyectoComunidades/Create
    public IActionResult Create()
    {
      PopulateProgramasProyectosDropDownList();
      PopulateComunidadesDropDownList();
      return View();
    }

    // POST: ProgramaProyectoComunidades/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ProgramaProyectoID,ComunidadID")] ProgramaProyectoComunidades programaProyectoComunidades)
    {
      ModelState.Remove("ProgramaProyecto");
      ModelState.Remove("Comunidad");

      if (await _context.ProgramaProyectoComunidades.AnyAsync(ppc => ppc.ProgramaProyectoID == programaProyectoComunidades.ProgramaProyectoID && ppc.ComunidadID == programaProyectoComunidades.ComunidadID))
      {
        ModelState.AddModelError(string.Empty, "Este programa/proyecto ya está asignado a esta comunidad.");
      }

      if (ModelState.IsValid)
      {
        _context.Add(programaProyectoComunidades);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Programa/Proyecto asignado a la comunidad exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      PopulateProgramasProyectosDropDownList(programaProyectoComunidades.ProgramaProyectoID);
      PopulateComunidadesDropDownList(programaProyectoComunidades.ComunidadID);
      return View(programaProyectoComunidades);
    }

    // La acción Edit para una tabla de cruce pura como esta no es necesaria.
    // Si se quiere cambiar la asignación, se elimina la existente y se crea una nueva.
    // Por lo tanto, las acciones Edit (GET y POST) generadas por el scaffolder se omiten.

    // GET: ProgramaProyectoComunidades/Delete/5/10
    public async Task<IActionResult> Delete(int? programaProyectoId, int? comunidadId)
    {
      if (programaProyectoId == null || comunidadId == null)
      {
        return NotFound();
      }

      var programaProyectoComunidades = await _context.ProgramaProyectoComunidades
          .Include(p => p.Comunidad)
          .Include(p => p.ProgramaProyecto)
          .FirstOrDefaultAsync(m => m.ProgramaProyectoID == programaProyectoId && m.ComunidadID == comunidadId);
      if (programaProyectoComunidades == null)
      {
        return NotFound();
      }

      return View(programaProyectoComunidades);
    }

    // POST: ProgramaProyectoComunidades/Delete/5/10
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int ProgramaProyectoID, int ComunidadID)
    {
      var programaProyectoComunidades = await _context.ProgramaProyectoComunidades.FindAsync(ProgramaProyectoID, ComunidadID);
      if (programaProyectoComunidades != null)
      {
        _context.ProgramaProyectoComunidades.Remove(programaProyectoComunidades);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Asignación de programa/proyecto a comunidad eliminada exitosamente.";
      }

      return RedirectToAction(nameof(Index));
    }

    private bool ProgramaProyectoComunidadesExists(int programaProyectoId, int comunidadId)
    {
      return _context.ProgramaProyectoComunidades.Any(e => e.ProgramaProyectoID == programaProyectoId && e.ComunidadID == comunidadId);
    }
  }
}
