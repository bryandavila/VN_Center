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
  public class TiposAsistenciaController : Controller
  {
    private readonly VNCenterDbContext _context;

    public TiposAsistenciaController(VNCenterDbContext context)
    {
      _context = context;
    }

    // GET: TiposAsistencia
    public async Task<IActionResult> Index()
    {
      return View(await _context.TiposAsistencia.OrderBy(t => t.NombreAsistencia).ToListAsync());
    }

    // GET: TiposAsistencia/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var tiposAsistencia = await _context.TiposAsistencia
          .FirstOrDefaultAsync(m => m.TipoAsistenciaID == id);
      if (tiposAsistencia == null)
      {
        return NotFound();
      }

      return View(tiposAsistencia);
    }

    // GET: TiposAsistencia/Create
    public IActionResult Create()
    {
      return View();
    }

    // POST: TiposAsistencia/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("TipoAsistenciaID,NombreAsistencia")] TiposAsistencia tiposAsistencia)
    {
      ModelState.Remove("BeneficiarioAsistenciaRecibida"); // Propiedad de navegación

      if (ModelState.IsValid)
      {
        if (await _context.TiposAsistencia.AnyAsync(t => t.NombreAsistencia == tiposAsistencia.NombreAsistencia))
        {
          ModelState.AddModelError("NombreAsistencia", "Ya existe un tipo de asistencia con este nombre. Debe ser único.");
          return View(tiposAsistencia);
        }
        _context.Add(tiposAsistencia);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Tipo de asistencia creado exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      return View(tiposAsistencia);
    }

    // GET: TiposAsistencia/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var tiposAsistencia = await _context.TiposAsistencia.FindAsync(id);
      if (tiposAsistencia == null)
      {
        return NotFound();
      }
      return View(tiposAsistencia);
    }

    // POST: TiposAsistencia/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("TipoAsistenciaID,NombreAsistencia")] TiposAsistencia tiposAsistencia)
    {
      if (id != tiposAsistencia.TipoAsistenciaID)
      {
        return NotFound();
      }

      ModelState.Remove("BeneficiarioAsistenciaRecibida"); // Propiedad de navegación

      if (ModelState.IsValid)
      {
        var tipoExistenteConMismoNombre = await _context.TiposAsistencia
                                                    .AsNoTracking()
                                                    .FirstOrDefaultAsync(t => t.NombreAsistencia == tiposAsistencia.NombreAsistencia && t.TipoAsistenciaID != tiposAsistencia.TipoAsistenciaID);
        if (tipoExistenteConMismoNombre != null)
        {
          ModelState.AddModelError("NombreAsistencia", "Ya existe otro tipo de asistencia con este nombre. Debe ser único.");
          return View(tiposAsistencia);
        }
        try
        {
          _context.Update(tiposAsistencia);
          await _context.SaveChangesAsync();
          TempData["SuccessMessage"] = "Tipo de asistencia actualizado exitosamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!TiposAsistenciaExists(tiposAsistencia.TipoAsistenciaID))
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
      return View(tiposAsistencia);
    }

    // GET: TiposAsistencia/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var tiposAsistencia = await _context.TiposAsistencia
          .FirstOrDefaultAsync(m => m.TipoAsistenciaID == id);
      if (tiposAsistencia == null)
      {
        return NotFound();
      }

      return View(tiposAsistencia);
    }

    // POST: TiposAsistencia/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var tiposAsistencia = await _context.TiposAsistencia.FindAsync(id);
      if (tiposAsistencia != null)
      {
        // Verificar si el tipo de asistencia está en uso en BeneficiarioAsistenciaRecibida
        bool enUso = await _context.BeneficiarioAsistenciaRecibida.AnyAsync(ba => ba.TipoAsistenciaID == id);
        if (enUso)
        {
          TempData["ErrorMessage"] = "Este tipo de asistencia no se puede eliminar porque está asignado a uno o más beneficiarios.";
          return RedirectToAction(nameof(Index));
        }

        _context.TiposAsistencia.Remove(tiposAsistencia);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Tipo de asistencia eliminado exitosamente.";
      }

      return RedirectToAction(nameof(Index));
    }

    private bool TiposAsistenciaExists(int id)
    {
      return _context.TiposAsistencia.Any(e => e.TipoAsistenciaID == id);
    }
  }
}
