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
  public class BeneficiarioAsistenciaRecibidaController : Controller
  {
    private readonly VNCenterDbContext _context;

    public BeneficiarioAsistenciaRecibidaController(VNCenterDbContext context)
    {
      _context = context;
    }

    // GET: BeneficiarioAsistenciaRecibida
    public async Task<IActionResult> Index()
    {
      var vNCenterDbContext = _context.BeneficiarioAsistenciaRecibida
                                          .Include(b => b.Beneficiario)
                                          .Include(b => b.TipoAsistencia)
                                          .OrderBy(b => b.Beneficiario.Apellidos)
                                          .ThenBy(b => b.Beneficiario.Nombres)
                                          .ThenBy(b => b.TipoAsistencia.NombreAsistencia);
      return View(await vNCenterDbContext.ToListAsync());
    }

    // GET: BeneficiarioAsistenciaRecibida/Details/5/10
    public async Task<IActionResult> Details(int? beneficiarioId, int? tipoAsistenciaId)
    {
      if (beneficiarioId == null || tipoAsistenciaId == null)
      {
        return NotFound();
      }

      var beneficiarioAsistenciaRecibida = await _context.BeneficiarioAsistenciaRecibida
          .Include(b => b.Beneficiario)
          .Include(b => b.TipoAsistencia)
          .FirstOrDefaultAsync(m => m.BeneficiarioID == beneficiarioId && m.TipoAsistenciaID == tipoAsistenciaId);
      if (beneficiarioAsistenciaRecibida == null)
      {
        return NotFound();
      }

      return View(beneficiarioAsistenciaRecibida);
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

    private void PopulateTiposAsistenciaDropDownList(object? selectedTipo = null)
    {
      var tiposQuery = from t in _context.TiposAsistencia
                       orderby t.NombreAsistencia
                       select t;
      ViewData["TipoAsistenciaID"] = new SelectList(tiposQuery.AsNoTracking(), "TipoAsistenciaID", "NombreAsistencia", selectedTipo);
    }

    // GET: BeneficiarioAsistenciaRecibida/Create
    public IActionResult Create()
    {
      PopulateBeneficiariosDropDownList();
      PopulateTiposAsistenciaDropDownList();
      return View();
    }

    // POST: BeneficiarioAsistenciaRecibida/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("BeneficiarioID,TipoAsistenciaID,NotasAdicionales")] BeneficiarioAsistenciaRecibida beneficiarioAsistenciaRecibida)
    {
      ModelState.Remove("Beneficiario");
      ModelState.Remove("TipoAsistencia");

      if (await _context.BeneficiarioAsistenciaRecibida.AnyAsync(ba => ba.BeneficiarioID == beneficiarioAsistenciaRecibida.BeneficiarioID && ba.TipoAsistenciaID == beneficiarioAsistenciaRecibida.TipoAsistenciaID))
      {
        ModelState.AddModelError(string.Empty, "Este tipo de asistencia ya está registrado para este beneficiario.");
      }

      if (ModelState.IsValid)
      {
        _context.Add(beneficiarioAsistenciaRecibida);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Asistencia asignada al beneficiario exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      PopulateBeneficiariosDropDownList(beneficiarioAsistenciaRecibida.BeneficiarioID);
      PopulateTiposAsistenciaDropDownList(beneficiarioAsistenciaRecibida.TipoAsistenciaID);
      return View(beneficiarioAsistenciaRecibida);
    }

    // GET: BeneficiarioAsistenciaRecibida/Edit/5/10
    public async Task<IActionResult> Edit(int? beneficiarioId, int? tipoAsistenciaId)
    {
      if (beneficiarioId == null || tipoAsistenciaId == null)
      {
        return NotFound();
      }

      var beneficiarioAsistenciaRecibida = await _context.BeneficiarioAsistenciaRecibida
                                                          .FirstOrDefaultAsync(ba => ba.BeneficiarioID == beneficiarioId && ba.TipoAsistenciaID == tipoAsistenciaId);
      if (beneficiarioAsistenciaRecibida == null)
      {
        return NotFound();
      }
      // Para la vista Edit, los dropdowns de Beneficiario y TipoAsistencia deberían estar deshabilitados
      // ya que son parte de la clave primaria y no deberían cambiarse. Solo se editan las NotasAdicionales.
      PopulateBeneficiariosDropDownList(beneficiarioAsistenciaRecibida.BeneficiarioID);
      PopulateTiposAsistenciaDropDownList(beneficiarioAsistenciaRecibida.TipoAsistenciaID);
      return View(beneficiarioAsistenciaRecibida);
    }

    // POST: BeneficiarioAsistenciaRecibida/Edit/5/10
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int beneficiarioId, int tipoAsistenciaId, [Bind("BeneficiarioID,TipoAsistenciaID,NotasAdicionales")] BeneficiarioAsistenciaRecibida beneficiarioAsistenciaRecibida)
    {
      if (beneficiarioId != beneficiarioAsistenciaRecibida.BeneficiarioID || tipoAsistenciaId != beneficiarioAsistenciaRecibida.TipoAsistenciaID)
      {
        return NotFound();
      }

      ModelState.Remove("Beneficiario");
      ModelState.Remove("TipoAsistencia");

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(beneficiarioAsistenciaRecibida);
          await _context.SaveChangesAsync();
          TempData["SuccessMessage"] = "Notas de asistencia actualizadas exitosamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!BeneficiarioAsistenciaRecibidaExists(beneficiarioAsistenciaRecibida.BeneficiarioID, beneficiarioAsistenciaRecibida.TipoAsistenciaID))
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
      PopulateBeneficiariosDropDownList(beneficiarioAsistenciaRecibida.BeneficiarioID);
      PopulateTiposAsistenciaDropDownList(beneficiarioAsistenciaRecibida.TipoAsistenciaID);
      return View(beneficiarioAsistenciaRecibida);
    }

    // GET: BeneficiarioAsistenciaRecibida/Delete/5/10
    public async Task<IActionResult> Delete(int? beneficiarioId, int? tipoAsistenciaId)
    {
      if (beneficiarioId == null || tipoAsistenciaId == null)
      {
        return NotFound();
      }

      var beneficiarioAsistenciaRecibida = await _context.BeneficiarioAsistenciaRecibida
          .Include(b => b.Beneficiario)
          .Include(b => b.TipoAsistencia)
          .FirstOrDefaultAsync(m => m.BeneficiarioID == beneficiarioId && m.TipoAsistenciaID == tipoAsistenciaId);
      if (beneficiarioAsistenciaRecibida == null)
      {
        return NotFound();
      }

      return View(beneficiarioAsistenciaRecibida);
    }

    // POST: BeneficiarioAsistenciaRecibida/Delete/5/10
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int BeneficiarioID, int TipoAsistenciaID)
    {
      var beneficiarioAsistenciaRecibida = await _context.BeneficiarioAsistenciaRecibida.FindAsync(BeneficiarioID, TipoAsistenciaID);
      if (beneficiarioAsistenciaRecibida != null)
      {
        _context.BeneficiarioAsistenciaRecibida.Remove(beneficiarioAsistenciaRecibida);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Registro de asistencia eliminado exitosamente.";
      }

      return RedirectToAction(nameof(Index));
    }

    private bool BeneficiarioAsistenciaRecibidaExists(int beneficiarioId, int tipoAsistenciaId)
    {
      return _context.BeneficiarioAsistenciaRecibida.Any(e => e.BeneficiarioID == beneficiarioId && e.TipoAsistenciaID == tipoAsistenciaId);
    }
  }
}
