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
  public class BeneficiariosProgramasProyectosController : Controller
  {
    private readonly VNCenterDbContext _context;

    public BeneficiariosProgramasProyectosController(VNCenterDbContext context)
    {
      _context = context;
    }

    // GET: BeneficiariosProgramasProyectos
    public async Task<IActionResult> Index()
    {
      var vNCenterDbContext = _context.BeneficiariosProgramasProyectos
                                          .Include(b => b.Beneficiario)
                                          .Include(b => b.ProgramaProyecto)
                                          .OrderBy(b => b.Beneficiario.Apellidos)
                                          .ThenBy(b => b.Beneficiario.Nombres)
                                          .ThenBy(b => b.ProgramaProyecto.NombreProgramaProyecto);
      return View(await vNCenterDbContext.ToListAsync());
    }

    // GET: BeneficiariosProgramasProyectos/Details/5/10
    public async Task<IActionResult> Details(int? beneficiarioId, int? programaProyectoId)
    {
      if (beneficiarioId == null || programaProyectoId == null)
      {
        return NotFound();
      }

      var beneficiariosProgramasProyectos = await _context.BeneficiariosProgramasProyectos
          .Include(b => b.Beneficiario)
          .Include(b => b.ProgramaProyecto)
          .FirstOrDefaultAsync(m => m.BeneficiarioID == beneficiarioId && m.ProgramaProyectoID == programaProyectoId);
      if (beneficiariosProgramasProyectos == null)
      {
        return NotFound();
      }

      return View(beneficiariosProgramasProyectos);
    }

    private void PopulateBeneficiariosDropDownList(object? selectedBeneficiario = null)
    {
      var beneficiariosQuery = from b in _context.Beneficiarios
                               orderby b.Apellidos, b.Nombres
                               select new
                               {
                                 b.BeneficiarioID,
                                 NombreCompleto = (b.Apellidos + ", " + b.Nombres + " (ID: " + b.BeneficiarioID + ")")
                               };
      ViewData["BeneficiarioID"] = new SelectList(beneficiariosQuery.AsNoTracking(), "BeneficiarioID", "NombreCompleto", selectedBeneficiario);
    }

    private void PopulateProgramasProyectosDropDownList(object? selectedPrograma = null)
    {
      var programasQuery = from p in _context.ProgramasProyectosONG
                             // Se podrían filtrar por programas activos/en curso
                             // where p.EstadoProgramaProyecto == "En Curso" || p.EstadoProgramaProyecto == "Planificación"
                           orderby p.NombreProgramaProyecto
                           select p;
      ViewData["ProgramaProyectoID"] = new SelectList(programasQuery.AsNoTracking(), "ProgramaProyectoID", "NombreProgramaProyecto", selectedPrograma);
    }


    // GET: BeneficiariosProgramasProyectos/Create
    public IActionResult Create()
    {
      PopulateBeneficiariosDropDownList();
      PopulateProgramasProyectosDropDownList();
      var model = new BeneficiariosProgramasProyectos { FechaInscripcionBeneficiario = DateTime.Today };
      return View(model);
    }

    // POST: BeneficiariosProgramasProyectos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("BeneficiarioID,ProgramaProyectoID,FechaInscripcionBeneficiario,EstadoParticipacionBeneficiario,NotasAdicionales")] BeneficiariosProgramasProyectos beneficiariosProgramasProyectos)
    {
      ModelState.Remove("Beneficiario");
      ModelState.Remove("ProgramaProyecto");

      if (await _context.BeneficiariosProgramasProyectos.AnyAsync(bpp => bpp.BeneficiarioID == beneficiariosProgramasProyectos.BeneficiarioID && bpp.ProgramaProyectoID == beneficiariosProgramasProyectos.ProgramaProyectoID))
      {
        ModelState.AddModelError(string.Empty, "Este beneficiario ya está inscrito en este programa/proyecto.");
      }

      if (ModelState.IsValid)
      {
        _context.Add(beneficiariosProgramasProyectos);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Beneficiario inscrito en programa/proyecto exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      PopulateBeneficiariosDropDownList(beneficiariosProgramasProyectos.BeneficiarioID);
      PopulateProgramasProyectosDropDownList(beneficiariosProgramasProyectos.ProgramaProyectoID);
      return View(beneficiariosProgramasProyectos);
    }

    // GET: BeneficiariosProgramasProyectos/Edit/5/10
    public async Task<IActionResult> Edit(int? beneficiarioId, int? programaProyectoId)
    {
      if (beneficiarioId == null || programaProyectoId == null)
      {
        return NotFound();
      }

      var beneficiariosProgramasProyectos = await _context.BeneficiariosProgramasProyectos
          .FirstOrDefaultAsync(bpp => bpp.BeneficiarioID == beneficiarioId && bpp.ProgramaProyectoID == programaProyectoId);
      if (beneficiariosProgramasProyectos == null)
      {
        return NotFound();
      }
      PopulateBeneficiariosDropDownList(beneficiariosProgramasProyectos.BeneficiarioID);
      PopulateProgramasProyectosDropDownList(beneficiariosProgramasProyectos.ProgramaProyectoID);
      return View(beneficiariosProgramasProyectos);
    }

    // POST: BeneficiariosProgramasProyectos/Edit/5/10
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int beneficiarioId, int programaProyectoId, [Bind("BeneficiarioID,ProgramaProyectoID,FechaInscripcionBeneficiario,EstadoParticipacionBeneficiario,NotasAdicionales")] BeneficiariosProgramasProyectos beneficiariosProgramasProyectos)
    {
      if (beneficiarioId != beneficiariosProgramasProyectos.BeneficiarioID || programaProyectoId != beneficiariosProgramasProyectos.ProgramaProyectoID)
      {
        return NotFound();
      }

      ModelState.Remove("Beneficiario");
      ModelState.Remove("ProgramaProyecto");

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(beneficiariosProgramasProyectos);
          await _context.SaveChangesAsync();
          TempData["SuccessMessage"] = "Inscripción de beneficiario actualizada exitosamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!BeneficiariosProgramasProyectosExists(beneficiariosProgramasProyectos.BeneficiarioID, beneficiariosProgramasProyectos.ProgramaProyectoID))
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
      PopulateBeneficiariosDropDownList(beneficiariosProgramasProyectos.BeneficiarioID);
      PopulateProgramasProyectosDropDownList(beneficiariosProgramasProyectos.ProgramaProyectoID);
      return View(beneficiariosProgramasProyectos);
    }

    // GET: BeneficiariosProgramasProyectos/Delete/5/10
    public async Task<IActionResult> Delete(int? beneficiarioId, int? programaProyectoId)
    {
      if (beneficiarioId == null || programaProyectoId == null)
      {
        return NotFound();
      }

      var beneficiariosProgramasProyectos = await _context.BeneficiariosProgramasProyectos
          .Include(b => b.Beneficiario)
          .Include(b => b.ProgramaProyecto)
          .FirstOrDefaultAsync(m => m.BeneficiarioID == beneficiarioId && m.ProgramaProyectoID == programaProyectoId);
      if (beneficiariosProgramasProyectos == null)
      {
        return NotFound();
      }

      return View(beneficiariosProgramasProyectos);
    }

    // POST: BeneficiariosProgramasProyectos/Delete/5/10
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int BeneficiarioID, int ProgramaProyectoID)
    {
      var beneficiariosProgramasProyectos = await _context.BeneficiariosProgramasProyectos.FindAsync(BeneficiarioID, ProgramaProyectoID);
      if (beneficiariosProgramasProyectos != null)
      {
        _context.BeneficiariosProgramasProyectos.Remove(beneficiariosProgramasProyectos);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Inscripción de beneficiario eliminada exitosamente.";
      }

      return RedirectToAction(nameof(Index));
    }

    private bool BeneficiariosProgramasProyectosExists(int beneficiarioId, int programaProyectoId)
    {
      return _context.BeneficiariosProgramasProyectos.Any(e => e.BeneficiarioID == beneficiarioId && e.ProgramaProyectoID == programaProyectoId);
    }
  }
}
