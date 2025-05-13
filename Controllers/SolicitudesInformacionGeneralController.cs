using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VN_Center.Data;
using VN_Center.Models.Entities; // Asegúrate que este using apunta al namespace correcto de tus entidades
using Microsoft.AspNetCore.Identity; // Para UserManager
using Microsoft.AspNetCore.Authorization;

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
      var vNCenterDbContext = _context.SolicitudesInformacionGeneral
                                  .Include(s => s.FuenteConocimiento)
                                  .Include(s => s.UsuarioAsignado);
      return View(await vNCenterDbContext.ToListAsync());
    }

    // GET: SolicitudesInformacionGeneral/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var solicitudesInformacionGeneral = await _context.SolicitudesInformacionGeneral
          .Include(s => s.FuenteConocimiento)
          .Include(s => s.UsuarioAsignado)
          .FirstOrDefaultAsync(m => m.SolicitudInfoID == id);
      if (solicitudesInformacionGeneral == null)
      {
        return NotFound();
      }

      return View(solicitudesInformacionGeneral);
    }

    private async Task PopulateUsuariosAsignadosDropDownListAsync(object? selectedUsuario = null)
    {
      var usuariosQuery = await _userManager.Users
                                  .Where(u => u.Activo)
                                  .OrderBy(u => u.Nombres)
                                  .ThenBy(u => u.Apellidos)
                                  .Select(u => new
                                  {
                                    u.Id,
                                    NombreCompleto = u.Nombres + " " + u.Apellidos + " (" + u.UserName + ")"
                                  })
                                  .ToListAsync();
      ViewData["UsuarioAsignadoID"] = new SelectList(usuariosQuery, "Id", "NombreCompleto", selectedUsuario);
    }

    private async Task PopulateFuentesConocimientoDropDownListAsync(object? selectedFuente = null)
    {
      var fuentesQuery = await _context.FuentesConocimiento
                                  .OrderBy(f => f.NombreFuente)
                                  .ToListAsync();
      ViewData["FuenteConocimientoID"] = new SelectList(fuentesQuery, "FuenteConocimientoID", "NombreFuente", selectedFuente);
    }

    // GET: SolicitudesInformacionGeneral/Create
    public async Task<IActionResult> Create()
    {
      await PopulateFuentesConocimientoDropDownListAsync();
      await PopulateUsuariosAsignadosDropDownListAsync();
      return View(new SolicitudesInformacionGeneral()); // Pasar un nuevo modelo para inicializar FechaRecepcion y EstadoSolicitudInfo
    }

    // POST: SolicitudesInformacionGeneral/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    // *** ACTUALIZADO EL ATRIBUTO BIND ***
    public async Task<IActionResult> Create([Bind("SolicitudInfoID,FechaRecepcion,NombreContacto,EmailContacto,TelefonoContacto,PermiteContactoWhatsApp,ProgramaDeInteres,ProgramaDeInteresOtro,PreguntasEspecificas,EstadoSolicitudInfo,NotasSeguimiento,UsuarioAsignadoID,FuenteConocimientoID")] SolicitudesInformacionGeneral solicitudesInformacionGeneral)
    {
      ModelState.Remove("UsuarioAsignado");
      ModelState.Remove("FuenteConocimiento");

      if (ModelState.IsValid)
      {
        _context.Add(solicitudesInformacionGeneral);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Solicitud de información general creada exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      await PopulateFuentesConocimientoDropDownListAsync(solicitudesInformacionGeneral.FuenteConocimientoID);
      await PopulateUsuariosAsignadosDropDownListAsync(solicitudesInformacionGeneral.UsuarioAsignadoID);
      return View(solicitudesInformacionGeneral);
    }

    // GET: SolicitudesInformacionGeneral/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var solicitudesInformacionGeneral = await _context.SolicitudesInformacionGeneral.FindAsync(id);
      if (solicitudesInformacionGeneral == null)
      {
        return NotFound();
      }
      await PopulateFuentesConocimientoDropDownListAsync(solicitudesInformacionGeneral.FuenteConocimientoID);
      await PopulateUsuariosAsignadosDropDownListAsync(solicitudesInformacionGeneral.UsuarioAsignadoID);
      return View(solicitudesInformacionGeneral);
    }

    // POST: SolicitudesInformacionGeneral/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    // *** ACTUALIZADO EL ATRIBUTO BIND ***
    public async Task<IActionResult> Edit(int id, [Bind("SolicitudInfoID,FechaRecepcion,NombreContacto,EmailContacto,TelefonoContacto,PermiteContactoWhatsApp,ProgramaDeInteres,ProgramaDeInteresOtro,PreguntasEspecificas,EstadoSolicitudInfo,NotasSeguimiento,UsuarioAsignadoID,FuenteConocimientoID")] SolicitudesInformacionGeneral solicitudesInformacionGeneral)
    {
      if (id != solicitudesInformacionGeneral.SolicitudInfoID)
      {
        return NotFound();
      }

      ModelState.Remove("UsuarioAsignado");
      ModelState.Remove("FuenteConocimiento");

      if (ModelState.IsValid)
      {
        try
        {
          // Para evitar que FechaRecepcion se sobrescriba si no se envía desde el formulario de edición
          var originalEntity = await _context.SolicitudesInformacionGeneral.AsNoTracking().FirstOrDefaultAsync(s => s.SolicitudInfoID == id);
          if (originalEntity != null)
          {
            solicitudesInformacionGeneral.FechaRecepcion = originalEntity.FechaRecepcion;
          }

          _context.Update(solicitudesInformacionGeneral);
          await _context.SaveChangesAsync();
          TempData["SuccessMessage"] = "Solicitud de información general actualizada exitosamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!SolicitudesInformacionGeneralExists(solicitudesInformacionGeneral.SolicitudInfoID))
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
      await PopulateFuentesConocimientoDropDownListAsync(solicitudesInformacionGeneral.FuenteConocimientoID);
      await PopulateUsuariosAsignadosDropDownListAsync(solicitudesInformacionGeneral.UsuarioAsignadoID);
      return View(solicitudesInformacionGeneral);
    }

    // GET: SolicitudesInformacionGeneral/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var solicitudesInformacionGeneral = await _context.SolicitudesInformacionGeneral
          .Include(s => s.FuenteConocimiento)
          .Include(s => s.UsuarioAsignado)
          .FirstOrDefaultAsync(m => m.SolicitudInfoID == id);
      if (solicitudesInformacionGeneral == null)
      {
        return NotFound();
      }

      return View(solicitudesInformacionGeneral);
    }

    // POST: SolicitudesInformacionGeneral/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var solicitudesInformacionGeneral = await _context.SolicitudesInformacionGeneral.FindAsync(id);
      if (solicitudesInformacionGeneral != null)
      {
        _context.SolicitudesInformacionGeneral.Remove(solicitudesInformacionGeneral);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Solicitud de información general eliminada exitosamente.";
      }

      return RedirectToAction(nameof(Index));
    }

    private bool SolicitudesInformacionGeneralExists(int id)
    {
      return _context.SolicitudesInformacionGeneral.Any(e => e.SolicitudInfoID == id);
    }
  }
}
