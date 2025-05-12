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
  public class BeneficiarioGruposController : Controller
  {
    private readonly VNCenterDbContext _context;

    public BeneficiarioGruposController(VNCenterDbContext context)
    {
      _context = context;
    }

    // GET: BeneficiarioGrupos
    public async Task<IActionResult> Index()
    {
      var vNCenterDbContext = _context.BeneficiarioGrupos
                                          .Include(b => b.Beneficiario)
                                          .Include(b => b.GrupoComunitario)
                                          .OrderBy(b => b.Beneficiario.Apellidos)
                                          .ThenBy(b => b.Beneficiario.Nombres)
                                          .ThenBy(b => b.GrupoComunitario.NombreGrupo);
      return View(await vNCenterDbContext.ToListAsync());
    }

    // GET: BeneficiarioGrupos/Details/5/10
    public async Task<IActionResult> Details(int? beneficiarioId, int? grupoId)
    {
      if (beneficiarioId == null || grupoId == null)
      {
        return NotFound();
      }

      var beneficiarioGrupos = await _context.BeneficiarioGrupos
          .Include(b => b.Beneficiario)
          .Include(b => b.GrupoComunitario)
          .FirstOrDefaultAsync(m => m.BeneficiarioID == beneficiarioId && m.GrupoID == grupoId);
      if (beneficiarioGrupos == null)
      {
        return NotFound();
      }

      return View(beneficiarioGrupos);
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

    private void PopulateGruposDropDownList(object? selectedGrupo = null)
    {
      var gruposQuery = from g in _context.GruposComunitarios
                        orderby g.NombreGrupo
                        select g;
      ViewData["GrupoID"] = new SelectList(gruposQuery.AsNoTracking(), "GrupoID", "NombreGrupo", selectedGrupo);
    }

    // GET: BeneficiarioGrupos/Create
    public IActionResult Create()
    {
      PopulateBeneficiariosDropDownList();
      PopulateGruposDropDownList();
      return View();
    }

    // POST: BeneficiarioGrupos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("BeneficiarioID,GrupoID,FechaUnionGrupo,RolEnGrupo")] BeneficiarioGrupos beneficiarioGrupos)
    {
      ModelState.Remove("Beneficiario");
      ModelState.Remove("GrupoComunitario");

      if (await _context.BeneficiarioGrupos.AnyAsync(bg => bg.BeneficiarioID == beneficiarioGrupos.BeneficiarioID && bg.GrupoID == beneficiarioGrupos.GrupoID))
      {
        ModelState.AddModelError(string.Empty, "Este beneficiario ya está asignado a este grupo.");
      }

      if (ModelState.IsValid)
      {
        _context.Add(beneficiarioGrupos);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Beneficiario asignado al grupo exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      PopulateBeneficiariosDropDownList(beneficiarioGrupos.BeneficiarioID);
      PopulateGruposDropDownList(beneficiarioGrupos.GrupoID);
      return View(beneficiarioGrupos);
    }

    // GET: BeneficiarioGrupos/Edit/5/10
    public async Task<IActionResult> Edit(int? beneficiarioId, int? grupoId)
    {
      if (beneficiarioId == null || grupoId == null)
      {
        return NotFound();
      }

      var beneficiarioGrupos = await _context.BeneficiarioGrupos
                                              .FirstOrDefaultAsync(bg => bg.BeneficiarioID == beneficiarioId && bg.GrupoID == grupoId);
      if (beneficiarioGrupos == null)
      {
        return NotFound();
      }
      // Para la vista Edit, los dropdowns de Beneficiario y Grupo deberían estar deshabilitados
      // ya que son parte de la clave primaria y no deberían cambiarse. Solo se editan los campos adicionales.
      PopulateBeneficiariosDropDownList(beneficiarioGrupos.BeneficiarioID);
      PopulateGruposDropDownList(beneficiarioGrupos.GrupoID);
      return View(beneficiarioGrupos);
    }

    // POST: BeneficiarioGrupos/Edit/5/10
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int beneficiarioId, int grupoId, [Bind("BeneficiarioID,GrupoID,FechaUnionGrupo,RolEnGrupo")] BeneficiarioGrupos beneficiarioGrupos)
    {
      if (beneficiarioId != beneficiarioGrupos.BeneficiarioID || grupoId != beneficiarioGrupos.GrupoID)
      {
        return NotFound();
      }

      ModelState.Remove("Beneficiario");
      ModelState.Remove("GrupoComunitario");

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(beneficiarioGrupos);
          await _context.SaveChangesAsync();
          TempData["SuccessMessage"] = "Información de beneficiario en grupo actualizada exitosamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!BeneficiarioGruposExists(beneficiarioGrupos.BeneficiarioID, beneficiarioGrupos.GrupoID))
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
      PopulateBeneficiariosDropDownList(beneficiarioGrupos.BeneficiarioID);
      PopulateGruposDropDownList(beneficiarioGrupos.GrupoID);
      return View(beneficiarioGrupos);
    }

    // GET: BeneficiarioGrupos/Delete/5/10
    public async Task<IActionResult> Delete(int? beneficiarioId, int? grupoId)
    {
      if (beneficiarioId == null || grupoId == null)
      {
        return NotFound();
      }

      var beneficiarioGrupos = await _context.BeneficiarioGrupos
          .Include(b => b.Beneficiario)
          .Include(b => b.GrupoComunitario)
          .FirstOrDefaultAsync(m => m.BeneficiarioID == beneficiarioId && m.GrupoID == grupoId);
      if (beneficiarioGrupos == null)
      {
        return NotFound();
      }

      return View(beneficiarioGrupos);
    }

    // POST: BeneficiarioGrupos/Delete/5/10
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int BeneficiarioID, int GrupoID)
    {
      var beneficiarioGrupos = await _context.BeneficiarioGrupos.FindAsync(BeneficiarioID, GrupoID);
      if (beneficiarioGrupos != null)
      {
        _context.BeneficiarioGrupos.Remove(beneficiarioGrupos);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Asignación de beneficiario a grupo eliminada exitosamente.";
      }

      return RedirectToAction(nameof(Index));
    }

    private bool BeneficiarioGruposExists(int beneficiarioId, int grupoId)
    {
      return _context.BeneficiarioGrupos.Any(e => e.BeneficiarioID == beneficiarioId && e.GrupoID == grupoId);
    }
  }
}
