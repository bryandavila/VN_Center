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
  public class FuentesConocimientoController : Controller
  {
    private readonly VNCenterDbContext _context;

    public FuentesConocimientoController(VNCenterDbContext context)
    {
      _context = context;
    }

    // GET: FuentesConocimiento
    public async Task<IActionResult> Index()
    {
      return View(await _context.FuentesConocimiento.OrderBy(f => f.NombreFuente).ToListAsync());
    }

    // GET: FuentesConocimiento/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var fuentesConocimiento = await _context.FuentesConocimiento
          .FirstOrDefaultAsync(m => m.FuenteConocimientoID == id);
      if (fuentesConocimiento == null)
      {
        return NotFound();
      }

      return View(fuentesConocimiento);
    }

    // GET: FuentesConocimiento/Create
    public IActionResult Create()
    {
      return View();
    }

    // POST: FuentesConocimiento/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FuenteConocimientoID,NombreFuente")] FuentesConocimiento fuentesConocimiento)
    {
      ModelState.Remove("Solicitudes"); // Propiedad de navegación

      if (ModelState.IsValid)
      {
        if (await _context.FuentesConocimiento.AnyAsync(f => f.NombreFuente == fuentesConocimiento.NombreFuente))
        {
          ModelState.AddModelError("NombreFuente", "Ya existe una fuente de conocimiento con este nombre. Debe ser único.");
          return View(fuentesConocimiento);
        }
        _context.Add(fuentesConocimiento);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Fuente de conocimiento creada exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      return View(fuentesConocimiento);
    }

    // GET: FuentesConocimiento/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var fuentesConocimiento = await _context.FuentesConocimiento.FindAsync(id);
      if (fuentesConocimiento == null)
      {
        return NotFound();
      }
      return View(fuentesConocimiento);
    }

    // POST: FuentesConocimiento/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("FuenteConocimientoID,NombreFuente")] FuentesConocimiento fuentesConocimiento)
    {
      if (id != fuentesConocimiento.FuenteConocimientoID)
      {
        return NotFound();
      }

      ModelState.Remove("Solicitudes"); // Propiedad de navegación

      if (ModelState.IsValid)
      {
        var fuenteExistenteConMismoNombre = await _context.FuentesConocimiento
                                                    .AsNoTracking()
                                                    .FirstOrDefaultAsync(f => f.NombreFuente == fuentesConocimiento.NombreFuente && f.FuenteConocimientoID != fuentesConocimiento.FuenteConocimientoID);
        if (fuenteExistenteConMismoNombre != null)
        {
          ModelState.AddModelError("NombreFuente", "Ya existe otra fuente de conocimiento con este nombre. Debe ser único.");
          return View(fuentesConocimiento);
        }
        try
        {
          _context.Update(fuentesConocimiento);
          await _context.SaveChangesAsync();
          TempData["SuccessMessage"] = "Fuente de conocimiento actualizada exitosamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!FuentesConocimientoExists(fuentesConocimiento.FuenteConocimientoID))
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
      return View(fuentesConocimiento);
    }

    // GET: FuentesConocimiento/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var fuentesConocimiento = await _context.FuentesConocimiento
          .FirstOrDefaultAsync(m => m.FuenteConocimientoID == id);
      if (fuentesConocimiento == null)
      {
        return NotFound();
      }

      return View(fuentesConocimiento);
    }

    // POST: FuentesConocimiento/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var fuentesConocimiento = await _context.FuentesConocimiento.FindAsync(id);
      if (fuentesConocimiento != null)
      {
        bool enUso = await _context.Solicitudes.AnyAsync(s => s.FuenteConocimientoID == id);
        if (enUso)
        {
          TempData["ErrorMessage"] = "Esta fuente de conocimiento no se puede eliminar porque está asignada a una o más solicitudes.";
          return RedirectToAction(nameof(Index));
        }
        _context.FuentesConocimiento.Remove(fuentesConocimiento);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Fuente de conocimiento eliminada exitosamente.";
      }

      return RedirectToAction(nameof(Index));
    }

    private bool FuentesConocimientoExists(int id)
    {
      return _context.FuentesConocimiento.Any(e => e.FuenteConocimientoID == id);
    }
  }
}
