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
  public class NivelesIdiomaController : Controller
  {
    private readonly VNCenterDbContext _context;

    public NivelesIdiomaController(VNCenterDbContext context)
    {
      _context = context;
    }

    // GET: NivelesIdioma
    public async Task<IActionResult> Index()
    {
      return View(await _context.NivelesIdioma.OrderBy(n => n.NombreNivel).ToListAsync());
    }

    // GET: NivelesIdioma/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var nivelesIdioma = await _context.NivelesIdioma
          .FirstOrDefaultAsync(m => m.NivelIdiomaID == id);
      if (nivelesIdioma == null)
      {
        return NotFound();
      }

      return View(nivelesIdioma);
    }

    // GET: NivelesIdioma/Create
    public IActionResult Create()
    {
      return View();
    }

    // POST: NivelesIdioma/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("NivelIdiomaID,NombreNivel")] NivelesIdioma nivelesIdioma)
    {
      ModelState.Remove("Solicitudes"); // Propiedad de navegación

      if (ModelState.IsValid)
      {
        // Verificar si ya existe un nivel con el mismo NombreNivel (que debe ser único)
        if (await _context.NivelesIdioma.AnyAsync(n => n.NombreNivel == nivelesIdioma.NombreNivel))
        {
          ModelState.AddModelError("NombreNivel", "Ya existe un nivel de idioma con este nombre. Debe ser único.");
          return View(nivelesIdioma);
        }
        _context.Add(nivelesIdioma);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Nivel de idioma creado exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      return View(nivelesIdioma);
    }

    // GET: NivelesIdioma/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var nivelesIdioma = await _context.NivelesIdioma.FindAsync(id);
      if (nivelesIdioma == null)
      {
        return NotFound();
      }
      return View(nivelesIdioma);
    }

    // POST: NivelesIdioma/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("NivelIdiomaID,NombreNivel")] NivelesIdioma nivelesIdioma)
    {
      if (id != nivelesIdioma.NivelIdiomaID)
      {
        return NotFound();
      }

      ModelState.Remove("Solicitudes"); // Propiedad de navegación

      if (ModelState.IsValid)
      {
        // Verificar si el NombreNivel ha cambiado y si el nuevo nombre ya existe para otro nivel
        var nivelExistenteConMismoNombre = await _context.NivelesIdioma
                                                    .AsNoTracking()
                                                    .FirstOrDefaultAsync(n => n.NombreNivel == nivelesIdioma.NombreNivel && n.NivelIdiomaID != nivelesIdioma.NivelIdiomaID);
        if (nivelExistenteConMismoNombre != null)
        {
          ModelState.AddModelError("NombreNivel", "Ya existe otro nivel de idioma con este nombre. Debe ser único.");
          return View(nivelesIdioma);
        }
        try
        {
          _context.Update(nivelesIdioma);
          await _context.SaveChangesAsync();
          TempData["SuccessMessage"] = "Nivel de idioma actualizado exitosamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!NivelesIdiomaExists(nivelesIdioma.NivelIdiomaID))
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
      return View(nivelesIdioma);
    }

    // GET: NivelesIdioma/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var nivelesIdioma = await _context.NivelesIdioma
          .FirstOrDefaultAsync(m => m.NivelIdiomaID == id);
      if (nivelesIdioma == null)
      {
        return NotFound();
      }

      return View(nivelesIdioma);
    }

    // POST: NivelesIdioma/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var nivelesIdioma = await _context.NivelesIdioma.FindAsync(id);
      if (nivelesIdioma != null)
      {
        // Verificar si el nivel está en uso en Solicitudes
        bool enUso = await _context.Solicitudes.AnyAsync(s => s.NivelIdiomaEspañolID == id);
        if (enUso)
        {
          TempData["ErrorMessage"] = "Este nivel de idioma no se puede eliminar porque está asignado a una o más solicitudes. Primero debe reasignar o eliminar esas solicitudes.";
          return RedirectToAction(nameof(Index));
        }

        _context.NivelesIdioma.Remove(nivelesIdioma);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Nivel de idioma eliminado exitosamente.";
      }

      return RedirectToAction(nameof(Index));
    }

    private bool NivelesIdiomaExists(int id)
    {
      return _context.NivelesIdioma.Any(e => e.NivelIdiomaID == id);
    }
  }
}
