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
  public class ProgramaProyectoGruposController : Controller
  {
    private readonly VNCenterDbContext _context;

    public ProgramaProyectoGruposController(VNCenterDbContext context)
    {
      _context = context;
    }

    // GET: ProgramaProyectoGrupos
    public async Task<IActionResult> Index()
    {
      var vNCenterDbContext = _context.ProgramaProyectoGrupos
                                          .Include(p => p.GrupoComunitario)
                                          .Include(p => p.ProgramaProyecto)
                                          .OrderBy(p => p.ProgramaProyecto.NombreProgramaProyecto)
                                          .ThenBy(p => p.GrupoComunitario.NombreGrupo);
      return View(await vNCenterDbContext.ToListAsync());
    }

    // GET: ProgramaProyectoGrupos/Details/5/10
    public async Task<IActionResult> Details(int? programaProyectoId, int? grupoId)
    {
      if (programaProyectoId == null || grupoId == null)
      {
        return NotFound();
      }

      var programaProyectoGrupos = await _context.ProgramaProyectoGrupos
          .Include(p => p.GrupoComunitario)
          .Include(p => p.ProgramaProyecto)
          .FirstOrDefaultAsync(m => m.ProgramaProyectoID == programaProyectoId && m.GrupoID == grupoId);
      if (programaProyectoGrupos == null)
      {
        return NotFound();
      }

      return View(programaProyectoGrupos);
    }

    private void PopulateProgramasProyectosDropDownList(object? selectedPrograma = null)
    {
      var programasQuery = from p in _context.ProgramasProyectosONG
                           orderby p.NombreProgramaProyecto
                           select p;
      ViewData["ProgramaProyectoID"] = new SelectList(programasQuery.AsNoTracking(), "ProgramaProyectoID", "NombreProgramaProyecto", selectedPrograma);
    }

    private void PopulateGruposDropDownList(object? selectedGrupo = null)
    {
      var gruposQuery = from g in _context.GruposComunitarios
                        orderby g.NombreGrupo
                        select g;
      ViewData["GrupoID"] = new SelectList(gruposQuery.AsNoTracking(), "GrupoID", "NombreGrupo", selectedGrupo);
    }

    // GET: ProgramaProyectoGrupos/Create
    public IActionResult Create()
    {
      PopulateProgramasProyectosDropDownList();
      PopulateGruposDropDownList();
      return View();
    }

    // POST: ProgramaProyectoGrupos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ProgramaProyectoID,GrupoID")] ProgramaProyectoGrupos programaProyectoGrupos)
    {
      ModelState.Remove("ProgramaProyecto");
      ModelState.Remove("GrupoComunitario");

      if (await _context.ProgramaProyectoGrupos.AnyAsync(ppg => ppg.ProgramaProyectoID == programaProyectoGrupos.ProgramaProyectoID && ppg.GrupoID == programaProyectoGrupos.GrupoID))
      {
        ModelState.AddModelError(string.Empty, "Este grupo ya está asignado a este programa/proyecto.");
      }

      if (ModelState.IsValid)
      {
        _context.Add(programaProyectoGrupos);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Grupo asignado al programa/proyecto exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      PopulateProgramasProyectosDropDownList(programaProyectoGrupos.ProgramaProyectoID);
      PopulateGruposDropDownList(programaProyectoGrupos.GrupoID);
      return View(programaProyectoGrupos);
    }

    // La acción Edit para una tabla de cruce pura como esta no es necesaria.
    // Se omiten las acciones Edit (GET y POST) generadas por el scaffolder.

    // GET: ProgramaProyectoGrupos/Delete/5/10
    public async Task<IActionResult> Delete(int? programaProyectoId, int? grupoId)
    {
      if (programaProyectoId == null || grupoId == null)
      {
        return NotFound();
      }

      var programaProyectoGrupos = await _context.ProgramaProyectoGrupos
          .Include(p => p.GrupoComunitario)
          .Include(p => p.ProgramaProyecto)
          .FirstOrDefaultAsync(m => m.ProgramaProyectoID == programaProyectoId && m.GrupoID == grupoId);
      if (programaProyectoGrupos == null)
      {
        return NotFound();
      }

      return View(programaProyectoGrupos);
    }

    // POST: ProgramaProyectoGrupos/Delete/5/10
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int ProgramaProyectoID, int GrupoID)
    {
      var programaProyectoGrupos = await _context.ProgramaProyectoGrupos.FindAsync(ProgramaProyectoID, GrupoID);
      if (programaProyectoGrupos != null)
      {
        _context.ProgramaProyectoGrupos.Remove(programaProyectoGrupos);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Asignación de grupo a programa/proyecto eliminada exitosamente.";
      }

      return RedirectToAction(nameof(Index));
    }

    private bool ProgramaProyectoGruposExists(int programaProyectoId, int grupoId)
    {
      return _context.ProgramaProyectoGrupos.Any(e => e.ProgramaProyectoID == programaProyectoId && e.GrupoID == grupoId);
    }
  }
}
