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
  public class SolicitudesInformacionGeneralController : Controller
  {
    private readonly VNCenterDbContext _context;

    public SolicitudesInformacionGeneralController(VNCenterDbContext context)
    {
      _context = context;
    }

    // GET: SolicitudesInformacionGeneral
    public async Task<IActionResult> Index()
    {
      var vNCenterDbContext = _context.SolicitudesInformacionGeneral.Include(s => s.UsuarioAsignado);
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
          .Include(s => s.UsuarioAsignado)
          .FirstOrDefaultAsync(m => m.SolicitudInfoID == id);
      if (solicitudesInformacionGeneral == null)
      {
        return NotFound();
      }

      return View(solicitudesInformacionGeneral);
    }

    private void PopulateUsuariosDropDownList(object? selectedUsuario = null)
    {
      var usuariosQuery = from u in _context.UsuariosSistema
                          where u.Activo // Solo usuarios activos
                          orderby u.Nombres, u.Apellidos
                          select new
                          {
                            u.UsuarioID,
                            NombreCompleto = u.Nombres + " " + u.Apellidos
                          };
      ViewData["UsuarioAsignadoID"] = new SelectList(usuariosQuery.AsNoTracking(), "UsuarioID", "NombreCompleto", selectedUsuario);
    }

    // GET: SolicitudesInformacionGeneral/Create
    public IActionResult Create()
    {
      PopulateUsuariosDropDownList();
      // Establecer FechaRecepcion por defecto si se desea, aunque ya lo hace el modelo
      // var model = new SolicitudesInformacionGeneral { FechaRecepcion = DateTime.UtcNow };
      // return View(model);
      return View();
    }

    // POST: SolicitudesInformacionGeneral/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("SolicitudInfoID,FechaRecepcion,NombreContacto,EmailContacto,TelefonoContacto,PermiteContactoWhatsApp,ProgramaDeInteres,ProgramaDeInteresOtro,PreguntasEspecificas,EstadoSolicitudInfo,NotasSeguimiento,UsuarioAsignadoID")] SolicitudesInformacionGeneral solicitudesInformacionGeneral)
    {
      ModelState.Remove("UsuarioAsignado"); // Para propiedad de navegación

      if (ModelState.IsValid)
      {
        solicitudesInformacionGeneral.FechaRecepcion = DateTime.UtcNow; // Asegurar fecha actual al crear
        _context.Add(solicitudesInformacionGeneral);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Solicitud de información creada exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      PopulateUsuariosDropDownList(solicitudesInformacionGeneral.UsuarioAsignadoID);
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
      PopulateUsuariosDropDownList(solicitudesInformacionGeneral.UsuarioAsignadoID);
      return View(solicitudesInformacionGeneral);
    }

    // POST: SolicitudesInformacionGeneral/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("SolicitudInfoID,FechaRecepcion,NombreContacto,EmailContacto,TelefonoContacto,PermiteContactoWhatsApp,ProgramaDeInteres,ProgramaDeInteresOtro,PreguntasEspecificas,EstadoSolicitudInfo,NotasSeguimiento,UsuarioAsignadoID")] SolicitudesInformacionGeneral solicitudesInformacionGeneral)
    {
      if (id != solicitudesInformacionGeneral.SolicitudInfoID)
      {
        return NotFound();
      }

      ModelState.Remove("UsuarioAsignado");

      if (ModelState.IsValid)
      {
        try
        {
          // No queremos que FechaRecepcion se modifique al editar, así que la cargamos del original
          var originalEntity = await _context.SolicitudesInformacionGeneral.AsNoTracking().FirstOrDefaultAsync(s => s.SolicitudInfoID == id);
          if (originalEntity != null)
          {
            solicitudesInformacionGeneral.FechaRecepcion = originalEntity.FechaRecepcion;
          }

          _context.Update(solicitudesInformacionGeneral);
          await _context.SaveChangesAsync();
          TempData["SuccessMessage"] = "Solicitud de información actualizada exitosamente.";
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
      PopulateUsuariosDropDownList(solicitudesInformacionGeneral.UsuarioAsignadoID);
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
        TempData["SuccessMessage"] = "Solicitud de información eliminada exitosamente.";
      }

      return RedirectToAction(nameof(Index));
    }

    private bool SolicitudesInformacionGeneralExists(int id)
    {
      return _context.SolicitudesInformacionGeneral.Any(e => e.SolicitudInfoID == id);
    }
  }
}
