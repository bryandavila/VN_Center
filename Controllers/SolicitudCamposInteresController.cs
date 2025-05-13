using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VN_Center.Data;
using VN_Center.Models.Entities;
using Microsoft.AspNetCore.Authorization;

namespace VN_Center.Controllers
{
  [Authorize]
  public class SolicitudCamposInteresController : Controller
  {
    private readonly VNCenterDbContext _context;

    public SolicitudCamposInteresController(VNCenterDbContext context)
    {
      _context = context;
    }

    // GET: SolicitudCamposInteres
    public async Task<IActionResult> Index()
    {
      var vNCenterDbContext = _context.SolicitudCamposInteres
                                          .Include(s => s.CampoInteresVocacional)
                                          .Include(s => s.Solicitud)
                                          .OrderBy(s => s.Solicitud.Apellidos)
                                          .ThenBy(s => s.Solicitud.Nombres)
                                          .ThenBy(s => s.CampoInteresVocacional.NombreCampo);
      return View(await vNCenterDbContext.ToListAsync());
    }

    // GET: SolicitudCamposInteres/Details/5/10 
    public async Task<IActionResult> Details(int? solicitudId, int? campoInteresId)
    {
      if (solicitudId == null || campoInteresId == null)
      {
        return NotFound();
      }

      var solicitudCamposInteres = await _context.SolicitudCamposInteres
          .Include(s => s.CampoInteresVocacional)
          .Include(s => s.Solicitud)
          .FirstOrDefaultAsync(m => m.SolicitudID == solicitudId && m.CampoInteresID == campoInteresId);
      if (solicitudCamposInteres == null)
      {
        return NotFound();
      }

      return View(solicitudCamposInteres);
    }

    private void PopulateSolicitudesDropDownList(object? selectedSolicitud = null)
    {
      var solicitudesQuery = from s in _context.Solicitudes
                             orderby s.Apellidos, s.Nombres
                             select new
                             {
                               s.SolicitudID,
                               DisplayText = (s.Apellidos + ", " + s.Nombres + " (ID: " + s.SolicitudID + ")")
                             };
      ViewData["SolicitudID"] = new SelectList(solicitudesQuery.AsNoTracking(), "SolicitudID", "DisplayText", selectedSolicitud);
    }

    private void PopulateCamposInteresDropDownList(object? selectedCampo = null)
    {
      var camposQuery = from c in _context.CamposInteresVocacional
                        orderby c.NombreCampo
                        select c;
      ViewData["CampoInteresID"] = new SelectList(camposQuery.AsNoTracking(), "CampoInteresID", "NombreCampo", selectedCampo);
    }

    // GET: SolicitudCamposInteres/Create
    public IActionResult Create()
    {
      PopulateSolicitudesDropDownList();
      PopulateCamposInteresDropDownList();
      return View();
    }

    // POST: SolicitudCamposInteres/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("SolicitudID,CampoInteresID")] SolicitudCamposInteres solicitudCamposInteres)
    {
      // Remover la validación de las propiedades de navegación
      ModelState.Remove("Solicitud");
      ModelState.Remove("CampoInteresVocacional");

      // Verificar si la combinación Rol-Permiso ya existe
      if (await _context.SolicitudCamposInteres.AnyAsync(sci => sci.SolicitudID == solicitudCamposInteres.SolicitudID && sci.CampoInteresID == solicitudCamposInteres.CampoInteresID))
      {
        ModelState.AddModelError(string.Empty, "Este campo de interés ya está asignado a esta solicitud.");
      }

      if (ModelState.IsValid) // Ahora ModelState.IsValid debería ser true si los IDs son válidos
      {
        _context.Add(solicitudCamposInteres);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Campo de interés asignado a la solicitud exitosamente.";
        return RedirectToAction(nameof(Index));
      }

      // Si llegamos aquí, algo falló, repoblar dropdowns y mostrar la vista con errores
      PopulateSolicitudesDropDownList(solicitudCamposInteres.SolicitudID);
      PopulateCamposInteresDropDownList(solicitudCamposInteres.CampoInteresID);
      return View(solicitudCamposInteres);
    }


    // GET: SolicitudCamposInteres/Delete/5/10
    public async Task<IActionResult> Delete(int? solicitudId, int? campoInteresId)
    {
      if (solicitudId == null || campoInteresId == null)
      {
        return NotFound();
      }

      var solicitudCamposInteres = await _context.SolicitudCamposInteres
          .Include(s => s.CampoInteresVocacional)
          .Include(s => s.Solicitud)
          .FirstOrDefaultAsync(m => m.SolicitudID == solicitudId && m.CampoInteresID == campoInteresId);
      if (solicitudCamposInteres == null)
      {
        return NotFound();
      }

      return View(solicitudCamposInteres);
    }

    // POST: SolicitudCamposInteres/Delete/5/10
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int SolicitudID, int CampoInteresID)
    {
      var solicitudCamposInteres = await _context.SolicitudCamposInteres.FindAsync(SolicitudID, CampoInteresID);
      if (solicitudCamposInteres != null)
      {
        _context.SolicitudCamposInteres.Remove(solicitudCamposInteres);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Asignación de campo de interés eliminada exitosamente.";
      }

      return RedirectToAction(nameof(Index));
    }

    private bool SolicitudCamposInteresExists(int solicitudId, int campoInteresId)
    {
      return _context.SolicitudCamposInteres.Any(e => e.SolicitudID == solicitudId && e.CampoInteresID == campoInteresId);
    }
  }
}
