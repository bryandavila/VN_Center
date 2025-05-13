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
  public class CamposInteresVocacionalController : Controller
  {
    private readonly VNCenterDbContext _context;

    public CamposInteresVocacionalController(VNCenterDbContext context)
    {
      _context = context;
    }

    // GET: CamposInteresVocacional
    public async Task<IActionResult> Index()
    {
      return View(await _context.CamposInteresVocacional.OrderBy(c => c.NombreCampo).ToListAsync());
    }

    // GET: CamposInteresVocacional/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var camposInteresVocacional = await _context.CamposInteresVocacional
          .FirstOrDefaultAsync(m => m.CampoInteresID == id);
      if (camposInteresVocacional == null)
      {
        return NotFound();
      }

      return View(camposInteresVocacional);
    }

    // GET: CamposInteresVocacional/Create
    public IActionResult Create()
    {
      return View();
    }

    // POST: CamposInteresVocacional/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("CampoInteresID,NombreCampo,DescripcionCampo")] CamposInteresVocacional camposInteresVocacional)
    {
      ModelState.Remove("SolicitudCamposInteres"); // Propiedad de navegación

      if (ModelState.IsValid)
      {
        if (await _context.CamposInteresVocacional.AnyAsync(c => c.NombreCampo == camposInteresVocacional.NombreCampo))
        {
          ModelState.AddModelError("NombreCampo", "Ya existe un campo de interés con este nombre. Debe ser único.");
          return View(camposInteresVocacional);
        }
        _context.Add(camposInteresVocacional);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Campo de interés creado exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      return View(camposInteresVocacional);
    }

    // GET: CamposInteresVocacional/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var camposInteresVocacional = await _context.CamposInteresVocacional.FindAsync(id);
      if (camposInteresVocacional == null)
      {
        return NotFound();
      }
      return View(camposInteresVocacional);
    }

    // POST: CamposInteresVocacional/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("CampoInteresID,NombreCampo,DescripcionCampo")] CamposInteresVocacional camposInteresVocacional)
    {
      if (id != camposInteresVocacional.CampoInteresID)
      {
        return NotFound();
      }

      ModelState.Remove("SolicitudCamposInteres"); // Propiedad de navegación

      if (ModelState.IsValid)
      {
        var campoExistenteConMismoNombre = await _context.CamposInteresVocacional
                                                    .AsNoTracking()
                                                    .FirstOrDefaultAsync(c => c.NombreCampo == camposInteresVocacional.NombreCampo && c.CampoInteresID != camposInteresVocacional.CampoInteresID);
        if (campoExistenteConMismoNombre != null)
        {
          ModelState.AddModelError("NombreCampo", "Ya existe otro campo de interés con este nombre. Debe ser único.");
          return View(camposInteresVocacional);
        }
        try
        {
          _context.Update(camposInteresVocacional);
          await _context.SaveChangesAsync();
          TempData["SuccessMessage"] = "Campo de interés actualizado exitosamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!CamposInteresVocacionalExists(camposInteresVocacional.CampoInteresID))
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
      return View(camposInteresVocacional);
    }

    // GET: CamposInteresVocacional/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var camposInteresVocacional = await _context.CamposInteresVocacional
          .FirstOrDefaultAsync(m => m.CampoInteresID == id);
      if (camposInteresVocacional == null)
      {
        return NotFound();
      }

      return View(camposInteresVocacional);
    }

    // POST: CamposInteresVocacional/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var camposInteresVocacional = await _context.CamposInteresVocacional.FindAsync(id);
      if (camposInteresVocacional != null)
      {
        // Verificar si el campo de interés está en uso en SolicitudCamposInteres
        bool enUso = await _context.SolicitudCamposInteres.AnyAsync(sci => sci.CampoInteresID == id);
        if (enUso)
        {
          TempData["ErrorMessage"] = "Este campo de interés no se puede eliminar porque está asignado a una o más solicitudes.";
          return RedirectToAction(nameof(Index));
        }

        _context.CamposInteresVocacional.Remove(camposInteresVocacional);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Campo de interés eliminado exitosamente.";
      }

      return RedirectToAction(nameof(Index));
    }

    private bool CamposInteresVocacionalExists(int id)
    {
      return _context.CamposInteresVocacional.Any(e => e.CampoInteresID == id);
    }
  }
}
