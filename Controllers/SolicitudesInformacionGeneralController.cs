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
using Microsoft.Extensions.Logging; // Asegúrate que ILogger está importado

namespace VN_Center.Controllers
{
  [Authorize]
  public class SolicitudesInformacionGeneralController : Controller
  {
    private readonly VNCenterDbContext _context;
    private readonly UserManager<UsuariosSistema> _userManager;
    private readonly ILogger<SolicitudesInformacionGeneralController> _logger;

    public SolicitudesInformacionGeneralController(
        VNCenterDbContext context,
        UserManager<UsuariosSistema> userManager,
        ILogger<SolicitudesInformacionGeneralController> logger)
    {
      _context = context;
      _userManager = userManager;
      _logger = logger;
    }

    // GET: SolicitudesInformacionGeneral
    public async Task<IActionResult> Index()
    {
      _logger.LogInformation("[DIAGNÓSTICO PERMISOS SolicitudesInformacionGeneral.Index] Verificando rol de administrador. User.IsInRole('Administrador'): {isAdmin}", User.IsInRole("Administrador"));

      IQueryable<SolicitudesInformacionGeneral> query = _context.SolicitudesInformacionGeneral
          .Include(s => s.FuenteConocimiento)
          .Include(s => s.UsuarioAsignado)
          .OrderByDescending(s => s.FechaRecepcion);

      if (!User.IsInRole("Administrador"))
      {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogInformation("[DIAGNÓSTICO PERMISOS SolicitudesInformacionGeneral.Index] Usuario NO es Administrador. Filtrando por UsuarioCreadorId: {userId}", userId);
        if (!string.IsNullOrEmpty(userId))
        {
          query = query.Where(s => s.UsuarioCreadorId == userId);
        }
        else
        {
          _logger.LogWarning("[DIAGNÓSTICO PERMISOS SolicitudesInformacionGeneral.Index] Usuario NO es Administrador y NO se pudo obtener userId. Mostrando ninguna solicitud.");
          query = query.Where(s => false);
        }
      }
      else
      {
        _logger.LogInformation("[DIAGNÓSTICO PERMISOS SolicitudesInformacionGeneral.Index] Usuario ES Administrador. Mostrando todas las solicitudes de información general.");
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

      var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      bool isAdmin = User.IsInRole("Administrador");
      _logger.LogInformation("[DIAGNÓSTICO PERMISOS SolicitudesInformacionGeneral.Details ID: {SolicitudID}] isAdmin: {isAdmin}, Solicitud.UsuarioCreadorId: {creadorId}, CurrentUserId: {currentUserId}", solicitud.SolicitudInfoID, isAdmin, solicitud.UsuarioCreadorId, currentUserId);

      if (!isAdmin && solicitud.UsuarioCreadorId != currentUserId)
      {
        _logger.LogWarning("[DIAGNÓSTICO PERMISOS SolicitudesInformacionGeneral.Details ID: {SolicitudID}] ACCESO DENEGADO (Lógica original). Usuario no es admin ni creador.", solicitud.SolicitudInfoID);
        TempData["ErrorMessage"] = "No tiene permiso para ver esta solicitud.";
        return RedirectToAction(nameof(Index));
      }
      _logger.LogInformation("[DIAGNÓSTICO PERMISOS SolicitudesInformacionGeneral.Details ID: {SolicitudID}] ACCESO PERMITIDO.", solicitud.SolicitudInfoID);
      return View(solicitud);
    }

    private async Task PopulateDropdownsAsync(SolicitudesInformacionGeneral? model = null)
    {
      ViewData["FuenteConocimientoID"] = new SelectList(await _context.FuentesConocimiento.OrderBy(f => f.NombreFuente).ToListAsync(), "FuenteConocimientoID", "NombreFuente", model?.FuenteConocimientoID);

      // Solo los administradores pueden asignar, y solo deben poder asignar a otros administradores (o usuarios activos si se cambia la lógica)
      if (User.IsInRole("Administrador"))
      {
        // *** MODIFICACIÓN AQUÍ para obtener solo administradores ***
        var administradores = await _userManager.GetUsersInRoleAsync("Administrador");

        var usuariosParaDropdown = administradores
                                  .Where(u => u.Activo) // Opcional: asegurar que solo admins activos puedan ser asignados
                                  .OrderBy(u => u.Nombres).ThenBy(u => u.Apellidos)
                                  .Select(u => new { u.Id, NombreCompleto = u.Nombres + " " + u.Apellidos + " (" + u.UserName + ")" })
                                  .ToList(); // ToList() aquí ya que GetUsersInRoleAsync devuelve IList

        ViewData["UsuarioAsignadoID"] = new SelectList(usuariosParaDropdown, "Id", "NombreCompleto", model?.UsuarioAsignadoID);
      }
    }

    // GET: SolicitudesInformacionGeneral/Create
    public async Task<IActionResult> Create()
    {
      await PopulateDropdownsAsync();
      var model = new SolicitudesInformacionGeneral();
      return View(model);
    }

    // POST: SolicitudesInformacionGeneral/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("SolicitudInfoID,NombreContacto,EmailContacto,TelefonoContacto,PermiteContactoWhatsApp,ProgramaDeInteres,ProgramaDeInteresOtro,PreguntasEspecificas,FuenteConocimientoID,EstadoSolicitudInfo,UsuarioAsignadoID,NotasSeguimiento")] SolicitudesInformacionGeneral solicitud)
    {
      ModelState.Remove("UsuarioAsignado");
      ModelState.Remove("FuenteConocimiento");
      ModelState.Remove("UsuarioCreadorId");

      bool isAdmin = User.IsInRole("Administrador");
      _logger.LogInformation("[DIAGNÓSTICO PERMISOS SolicitudesInformacionGeneral.Create POST] isAdmin: {isAdmin}", isAdmin);

      if (!isAdmin)
      {
        _logger.LogInformation("[DIAGNÓSTICO PERMISOS SolicitudesInformacionGeneral.Create POST] Usuario NO es Administrador. Forzando valores por defecto para campos de gestión interna.");
        solicitud.EstadoSolicitudInfo = "Nueva";
        solicitud.UsuarioAsignadoID = null;
        solicitud.NotasSeguimiento = null;

        ModelState.Remove(nameof(solicitud.EstadoSolicitudInfo));
        ModelState.Remove(nameof(solicitud.UsuarioAsignadoID));
        ModelState.Remove(nameof(solicitud.NotasSeguimiento));
      }


      if (ModelState.IsValid)
      {
        solicitud.UsuarioCreadorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogInformation("[DIAGNÓSTICO PERMISOS SolicitudesInformacionGeneral.Create POST] Asignando UsuarioCreadorId: {userId}", solicitud.UsuarioCreadorId);
        solicitud.FechaRecepcion = DateTime.UtcNow;

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
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Edit(int? id)
    {
      _logger.LogInformation("[DIAGNÓSTICO PERMISOS SolicitudesInformacionGeneral.Edit GET ID: {SolicitudID}] Acceso por [Authorize(Roles = 'Administrador')]. User.IsInRole('Administrador'): {isAdmin}", id, User.IsInRole("Administrador"));
      if (id == null)
      {
        return NotFound();
      }

      var solicitud = await _context.SolicitudesInformacionGeneral.FindAsync(id);
      if (solicitud == null)
      {
        return NotFound();
      }
      await PopulateDropdownsAsync(solicitud); // PopulateDropdownsAsync se encargará de mostrar solo admins si el usuario actual es admin
      return View(solicitud);
    }

    // POST: SolicitudesInformacionGeneral/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Edit(int id, [Bind("SolicitudInfoID,FechaRecepcion,NombreContacto,EmailContacto,TelefonoContacto,PermiteContactoWhatsApp,ProgramaDeInteres,ProgramaDeInteresOtro,PreguntasEspecificas,EstadoSolicitudInfo,NotasSeguimiento,UsuarioAsignadoID,FuenteConocimientoID")] SolicitudesInformacionGeneral solicitudModificada)
    {
      _logger.LogInformation("[DIAGNÓSTICO PERMISOS SolicitudesInformacionGeneral.Edit POST ID: {SolicitudID}] Acceso por [Authorize(Roles = 'Administrador')]. User.IsInRole('Administrador'): {isAdmin}", id, User.IsInRole("Administrador"));
      if (id != solicitudModificada.SolicitudInfoID)
      {
        return NotFound();
      }

      var solicitudOriginal = await _context.SolicitudesInformacionGeneral.AsNoTracking().FirstOrDefaultAsync(s => s.SolicitudInfoID == id);
      if (solicitudOriginal == null)
      {
        return NotFound();
      }

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
      await PopulateDropdownsAsync(solicitudModificada); // Repopulate con la lógica actualizada
      return View(solicitudModificada);
    }

    // GET: SolicitudesInformacionGeneral/Delete/5
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int? id)
    {
      _logger.LogInformation("[DIAGNÓSTICO PERMISOS SolicitudesInformacionGeneral.Delete GET ID: {SolicitudID}] Acceso por [Authorize(Roles = 'Administrador')]. User.IsInRole('Administrador'): {isAdmin}", id, User.IsInRole("Administrador"));
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
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      _logger.LogInformation("[DIAGNÓSTICO PERMISOS SolicitudesInformacionGeneral.DeleteConfirmed POST ID: {SolicitudID}] Acceso por [Authorize(Roles = 'Administrador')]. User.IsInRole('Administrador'): {isAdmin}", id, User.IsInRole("Administrador"));
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
