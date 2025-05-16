// VN_Center/Controllers/SolicitudesInformacionGeneralController.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VN_Center.Data;
using VN_Center.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace VN_Center.Controllers
{
  [Authorize]
  public class SolicitudesInformacionGeneralController : Controller
  {
    private readonly VNCenterDbContext _context;
    private readonly UserManager<UsuariosSistema> _userManager;

    public SolicitudesInformacionGeneralController(VNCenterDbContext context, UserManager<UsuariosSistema> userManager)
    {
      _context = context;
      _userManager = userManager;
    }

    // GET: SolicitudesInformacionGeneral
    public async Task<IActionResult> Index()
    {
      IQueryable<SolicitudesInformacionGeneral> query = _context.SolicitudesInformacionGeneral
          .Include(s => s.FuenteConocimiento)
          .Include(s => s.UsuarioAsignado)
          .OrderByDescending(s => s.FechaRecepcion);

      if (!User.IsInRole("Administrador"))
      {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
        {
          query = query.Where(s => s.UsuarioCreadorId == userId);
        }
        else
        {
          query = query.Where(s => false);
        }
      }
      return View(await query.ToListAsync());
    }

    // GET: SolicitudesInformacionGeneral/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var solicitud = await _context.SolicitudesInformacionGeneral
          .Include(s => s.FuenteConocimiento)
          .Include(s => s.UsuarioAsignado)
          .FirstOrDefaultAsync(m => m.SolicitudInfoID == id);

      if (solicitud == null)
      {
        return NotFound();
      }

      if (!User.IsInRole("Administrador") && solicitud.UsuarioCreadorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
      {
        TempData["ErrorMessage"] = "No tiene permiso para ver esta solicitud.";
        return RedirectToAction(nameof(Index));
      }

      return View(solicitud);
    }

    private async Task PopulateDropdownsAsync(SolicitudesInformacionGeneral? model = null)
    {
      ViewData["FuenteConocimientoID"] = new SelectList(await _context.FuentesConocimiento.OrderBy(f => f.NombreFuente).ToListAsync(), "FuenteConocimientoID", "NombreFuente", model?.FuenteConocimientoID);

      if (User.IsInRole("Administrador")) // Solo poblar usuarios asignados para administradores
      {
        var usuariosQuery = await _userManager.Users
                               .Where(u => u.Activo) // Solo usuarios activos
                               .OrderBy(u => u.Nombres).ThenBy(u => u.Apellidos)
                               .Select(u => new { u.Id, NombreCompleto = u.Nombres + " " + u.Apellidos + " (" + u.UserName + ")" })
                               .ToListAsync();
        ViewData["UsuarioAsignadoID"] = new SelectList(usuariosQuery, "Id", "NombreCompleto", model?.UsuarioAsignadoID);
      }
    }

    // GET: SolicitudesInformacionGeneral/Create
    public async Task<IActionResult> Create()
    {
      await PopulateDropdownsAsync();
      var model = new SolicitudesInformacionGeneral(); // FechaRecepcion y EstadoSolicitudInfo se setean en el constructor
      return View(model);
    }

    // POST: SolicitudesInformacionGeneral/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("SolicitudInfoID,NombreContacto,EmailContacto,TelefonoContacto,PermiteContactoWhatsApp,ProgramaDeInteres,ProgramaDeInteresOtro,PreguntasEspecificas,FuenteConocimientoID,EstadoSolicitudInfo,UsuarioAsignadoID,NotasSeguimiento")] SolicitudesInformacionGeneral solicitud)
    {
      // Remover propiedades de navegación para evitar problemas de binding si no se incluyen explícitamente
      ModelState.Remove("UsuarioAsignado");
      ModelState.Remove("FuenteConocimiento");
      ModelState.Remove("UsuarioCreadorId"); // No viene del formulario

      // Si el usuario no es admin, los campos de gestión interna no deben venir del Bind
      // y se les asignará valores por defecto o se ignorarán.
      if (!User.IsInRole("Administrador"))
      {
        // Forzar valores por defecto para campos de gestión interna si no es admin
        solicitud.EstadoSolicitudInfo = "Nueva"; // Estado inicial por defecto
        solicitud.UsuarioAsignadoID = null;    // No se puede auto-asignar
        solicitud.NotasSeguimiento = null;     // No puede añadir notas de seguimiento al crear

        // Remover del ModelState para que no fallen las validaciones si estos campos
        // se incluyeron accidentalmente en el Bind para un no-admin
        ModelState.Remove(nameof(solicitud.EstadoSolicitudInfo));
        ModelState.Remove(nameof(solicitud.UsuarioAsignadoID));
        ModelState.Remove(nameof(solicitud.NotasSeguimiento));
      }


      if (ModelState.IsValid)
      {
        solicitud.UsuarioCreadorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        solicitud.FechaRecepcion = DateTime.UtcNow; // Asegurar fecha de recepción actual

        // Si no es admin y el estado es nulo (por si acaso), establecerlo
        if (string.IsNullOrWhiteSpace(solicitud.EstadoSolicitudInfo))
        {
          solicitud.EstadoSolicitudInfo = "Nueva";
        }

        _context.Add(solicitud);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Solicitud de información general creada exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      await PopulateDropdownsAsync(solicitud);
      return View(solicitud);
    }

    // GET: SolicitudesInformacionGeneral/Edit/5
    [Authorize(Roles = "Administrador")] // Solo Administradores
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var solicitud = await _context.SolicitudesInformacionGeneral.FindAsync(id);
      if (solicitud == null)
      {
        return NotFound();
      }
      await PopulateDropdownsAsync(solicitud);
      return View(solicitud);
    }

    // POST: SolicitudesInformacionGeneral/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")] // Solo Administradores
    public async Task<IActionResult> Edit(int id, [Bind("SolicitudInfoID,FechaRecepcion,NombreContacto,EmailContacto,TelefonoContacto,PermiteContactoWhatsApp,ProgramaDeInteres,ProgramaDeInteresOtro,PreguntasEspecificas,EstadoSolicitudInfo,NotasSeguimiento,UsuarioAsignadoID,FuenteConocimientoID")] SolicitudesInformacionGeneral solicitudModificada)
    {
      if (id != solicitudModificada.SolicitudInfoID)
      {
        return NotFound();
      }

      var solicitudOriginal = await _context.SolicitudesInformacionGeneral.AsNoTracking().FirstOrDefaultAsync(s => s.SolicitudInfoID == id);
      if (solicitudOriginal == null)
      {
        return NotFound();
      }
      // Preservar UsuarioCreadorId y FechaRecepcion
      solicitudModificada.UsuarioCreadorId = solicitudOriginal.UsuarioCreadorId;
      solicitudModificada.FechaRecepcion = solicitudOriginal.FechaRecepcion;

      ModelState.Remove("UsuarioAsignado");
      ModelState.Remove("FuenteConocimiento");

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(solicitudModificada);
          await _context.SaveChangesAsync();
          TempData["SuccessMessage"] = "Solicitud de información general actualizada exitosamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!SolicitudesInformacionGeneralExists(solicitudModificada.SolicitudInfoID))
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
      await PopulateDropdownsAsync(solicitudModificada);
      return View(solicitudModificada);
    }

    // GET: SolicitudesInformacionGeneral/Delete/5
    [Authorize(Roles = "Administrador")] // Solo Administradores
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var solicitud = await _context.SolicitudesInformacionGeneral
          .Include(s => s.FuenteConocimiento)
          .Include(s => s.UsuarioAsignado)
          .FirstOrDefaultAsync(m => m.SolicitudInfoID == id);
      if (solicitud == null)
      {
        return NotFound();
      }
      return View(solicitud);
    }

    // POST: SolicitudesInformacionGeneral/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")] // Solo Administradores
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var solicitud = await _context.SolicitudesInformacionGeneral.FindAsync(id);
      if (solicitud != null)
      {
        _context.SolicitudesInformacionGeneral.Remove(solicitud);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Solicitud de información general eliminada exitosamente.";
      }
      else
      {
        TempData["ErrorMessage"] = "La solicitud no fue encontrada.";
      }
      return RedirectToAction(nameof(Index));
    }

    private bool SolicitudesInformacionGeneralExists(int id)
    {
      return _context.SolicitudesInformacionGeneral.Any(e => e.SolicitudInfoID == id);
    }
  }
}
