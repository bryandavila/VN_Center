// VN_Center/Controllers/GruposComunitariosController.cs
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
using Microsoft.AspNetCore.Identity; // <--- AÑADIDO para UserManager
using System.Security.Claims;      // <--- AÑADIDO para ClaimTypes

namespace VN_Center.Controllers
{
  [Authorize] // Todos los usuarios autenticados pueden acceder al controlador
  public class GruposComunitariosController : Controller
  {
    private readonly VNCenterDbContext _context;
    private readonly UserManager<UsuariosSistema> _userManager; // <--- AÑADIDO

    public GruposComunitariosController(VNCenterDbContext context, UserManager<UsuariosSistema> userManager) // <--- MODIFICADO
    {
      _context = context;
      _userManager = userManager; // <--- AÑADIDO
    }

    // GET: GruposComunitarios
    // Todos los usuarios autenticados pueden ver la lista
    public async Task<IActionResult> Index()
    {
      var query = _context.GruposComunitarios
          .Include(g => g.Comunidad)
          .OrderBy(g => g.NombreGrupo);
      // No se filtra por UsuarioCreadorId para usuarios no administradores
      return View(await query.ToListAsync());
    }

    // GET: GruposComunitarios/Details/5
    // Todos los usuarios autenticados pueden ver los detalles
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var grupoComunitario = await _context.GruposComunitarios
          .Include(g => g.Comunidad)
          .FirstOrDefaultAsync(m => m.GrupoID == id);
      if (grupoComunitario == null)
      {
        return NotFound();
      }
      // No se verifica UsuarioCreadorId para la visualización de detalles
      return View(grupoComunitario);
    }

    private void PopulateComunidadesDropDownList(object? selectedComunidad = null)
    {
      var comunidadesQuery = from c in _context.Comunidades
                             orderby c.NombreComunidad
                             select c;
      ViewData["ComunidadID"] = new SelectList(comunidadesQuery.AsNoTracking(), "ComunidadID", "NombreComunidad", selectedComunidad);
    }

    // GET: GruposComunitarios/Create
    [Authorize(Roles = "Administrador")] // Solo Administradores
    public IActionResult Create()
    {
      PopulateComunidadesDropDownList();
      return View();
    }

    // POST: GruposComunitarios/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")] // Solo Administradores
    public async Task<IActionResult> Create([Bind("GrupoID,NombreGrupo,DescripcionGrupo,ComunidadID,TipoGrupo,PersonaContactoPrincipal,TelefonoContactoGrupo,EmailContactoGrupo")] GruposComunitarios gruposComunitarios)
    {
      ModelState.Remove("Comunidad");
      ModelState.Remove("BeneficiarioGrupos");
      ModelState.Remove("ProgramaProyectoGrupos");
      // ModelState.Remove("UsuarioCreador");

      if (ModelState.IsValid)
      {
        gruposComunitarios.UsuarioCreadorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        _context.Add(gruposComunitarios);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Grupo comunitario creado exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      PopulateComunidadesDropDownList(gruposComunitarios.ComunidadID);
      return View(gruposComunitarios);
    }

    // GET: GruposComunitarios/Edit/5
    [Authorize(Roles = "Administrador")] // Solo Administradores
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var grupoComunitario = await _context.GruposComunitarios.FindAsync(id);
      if (grupoComunitario == null)
      {
        return NotFound();
      }
      // No es necesario verificar UsuarioCreadorId aquí debido a [Authorize(Roles = "Administrador")]
      PopulateComunidadesDropDownList(grupoComunitario.ComunidadID);
      return View(grupoComunitario);
    }

    // POST: GruposComunitarios/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")] // Solo Administradores
    public async Task<IActionResult> Edit(int id, [Bind("GrupoID,NombreGrupo,DescripcionGrupo,ComunidadID,TipoGrupo,PersonaContactoPrincipal,TelefonoContactoGrupo,EmailContactoGrupo")] GruposComunitarios grupoModificado)
    {
      if (id != grupoModificado.GrupoID)
      {
        return NotFound();
      }

      var grupoOriginal = await _context.GruposComunitarios.AsNoTracking().FirstOrDefaultAsync(g => g.GrupoID == id);
      if (grupoOriginal == null)
      {
        return NotFound();
      }
      // Preservar el UsuarioCreadorId original
      grupoModificado.UsuarioCreadorId = grupoOriginal.UsuarioCreadorId;

      ModelState.Remove("Comunidad");
      ModelState.Remove("BeneficiarioGrupos");
      ModelState.Remove("ProgramaProyectoGrupos");

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(grupoModificado);
          await _context.SaveChangesAsync();
          TempData["SuccessMessage"] = "Grupo comunitario actualizado exitosamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!GruposComunitariosExists(grupoModificado.GrupoID))
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
      PopulateComunidadesDropDownList(grupoModificado.ComunidadID);
      return View(grupoModificado);
    }

    // GET: GruposComunitarios/Delete/5
    [Authorize(Roles = "Administrador")] // Solo Administradores
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var grupoComunitario = await _context.GruposComunitarios
          .Include(g => g.Comunidad)
          .FirstOrDefaultAsync(m => m.GrupoID == id);
      if (grupoComunitario == null)
      {
        return NotFound();
      }
      // No es necesario verificar UsuarioCreadorId aquí.
      return View(grupoComunitario);
    }

    // POST: GruposComunitarios/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")] // Solo Administradores
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var grupoComunitario = await _context.GruposComunitarios.FindAsync(id);
      if (grupoComunitario != null)
      {
        // No es necesario verificar UsuarioCreadorId aquí.
        _context.GruposComunitarios.Remove(grupoComunitario);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Grupo comunitario eliminado exitosamente.";
      }
      else
      {
        TempData["ErrorMessage"] = "El grupo comunitario no fue encontrado.";
      }
      return RedirectToAction(nameof(Index));
    }

    private bool GruposComunitariosExists(int id)
    {
      return _context.GruposComunitarios.Any(e => e.GrupoID == id);
    }
  }
}
