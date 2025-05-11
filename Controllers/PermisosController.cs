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
  public class PermisosController : Controller
  {
    private readonly VNCenterDbContext _context;

    public PermisosController(VNCenterDbContext context)
    {
      _context = context;
    }

    // GET: Permisos
    public async Task<IActionResult> Index()
    {
      return View(await _context.Permisos.OrderBy(p => p.NombrePermiso).ToListAsync());
    }

    // GET: Permisos/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var permisos = await _context.Permisos
          .FirstOrDefaultAsync(m => m.PermisoID == id);
      if (permisos == null)
      {
        return NotFound();
      }

      return View(permisos);
    }

    // GET: Permisos/Create
    public IActionResult Create()
    {
      return View();
    }

    // POST: Permisos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("PermisoID,NombrePermiso,DescripcionPermiso")] Permisos permisos)
    {
      ModelState.Remove("RolPermisos"); // Propiedad de navegación

      if (ModelState.IsValid)
      {
        // Verificar si ya existe un permiso con el mismo NombrePermiso (que debe ser único)
        if (await _context.Permisos.AnyAsync(p => p.NombrePermiso == permisos.NombrePermiso))
        {
          ModelState.AddModelError("NombrePermiso", "Ya existe un permiso con este nombre (clave). Debe ser único.");
          return View(permisos);
        }

        _context.Add(permisos);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Permiso creado exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      return View(permisos);
    }

    // GET: Permisos/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var permisos = await _context.Permisos.FindAsync(id);
      if (permisos == null)
      {
        return NotFound();
      }
      return View(permisos);
    }

    // POST: Permisos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("PermisoID,NombrePermiso,DescripcionPermiso")] Permisos permisos)
    {
      if (id != permisos.PermisoID)
      {
        return NotFound();
      }

      ModelState.Remove("RolPermisos"); // Propiedad de navegación

      if (ModelState.IsValid)
      {
        // Verificar si el NombrePermiso ha cambiado y si el nuevo nombre ya existe para otro permiso
        var permisoExistenteConMismoNombre = await _context.Permisos
                                                    .AsNoTracking() // No rastrear para evitar conflictos con 'permisos'
                                                    .FirstOrDefaultAsync(p => p.NombrePermiso == permisos.NombrePermiso && p.PermisoID != permisos.PermisoID);
        if (permisoExistenteConMismoNombre != null)
        {
          ModelState.AddModelError("NombrePermiso", "Ya existe otro permiso con este nombre (clave). Debe ser único.");
          return View(permisos);
        }

        try
        {
          _context.Update(permisos);
          await _context.SaveChangesAsync();
          TempData["SuccessMessage"] = "Permiso actualizado exitosamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!PermisosExists(permisos.PermisoID))
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
      return View(permisos);
    }

    // GET: Permisos/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var permisos = await _context.Permisos
          .FirstOrDefaultAsync(m => m.PermisoID == id);
      if (permisos == null)
      {
        return NotFound();
      }

      return View(permisos);
    }

    // POST: Permisos/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var permisos = await _context.Permisos.FindAsync(id);
      if (permisos != null)
      {
        // Antes de eliminar, verificar si este permiso está en uso en RolPermisos
        bool enUso = await _context.RolPermisos.AnyAsync(rp => rp.PermisoID == id);
        if (enUso)
        {
          TempData["ErrorMessage"] = "Este permiso no se puede eliminar porque está asignado a uno o más roles. Primero debe quitarlo de los roles.";
          // Podrías redirigir a Details o a Index, o pasar el modelo de nuevo a la vista Delete con el error.
          // Por simplicidad, redirigimos a Index.
          return RedirectToAction(nameof(Index));
        }

        _context.Permisos.Remove(permisos);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Permiso eliminado exitosamente.";
      }

      return RedirectToAction(nameof(Index));
    }

    private bool PermisosExists(int id)
    {
      return _context.Permisos.Any(e => e.PermisoID == id);
    }
  }
}
