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
  public class GruposComunitariosController : Controller
  {
    private readonly VNCenterDbContext _context;

    public GruposComunitariosController(VNCenterDbContext context)
    {
      _context = context;
    }

    // GET: GruposComunitarios
    public async Task<IActionResult> Index()
    {
      var vNCenterDbContext = _context.GruposComunitarios.Include(g => g.Comunidad);
      return View(await vNCenterDbContext.OrderBy(g => g.NombreGrupo).ToListAsync());
    }

    // GET: GruposComunitarios/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var gruposComunitarios = await _context.GruposComunitarios
          .Include(g => g.Comunidad)
          .FirstOrDefaultAsync(m => m.GrupoID == id);
      if (gruposComunitarios == null)
      {
        return NotFound();
      }

      return View(gruposComunitarios);
    }

    private void PopulateComunidadesDropDownList(object? selectedComunidad = null)
    {
      var comunidadesQuery = from c in _context.Comunidades
                             orderby c.NombreComunidad
                             select c;
      ViewData["ComunidadID"] = new SelectList(comunidadesQuery.AsNoTracking(), "ComunidadID", "NombreComunidad", selectedComunidad);
    }

    // GET: GruposComunitarios/Create
    public IActionResult Create()
    {
      PopulateComunidadesDropDownList();
      return View();
    }

    // POST: GruposComunitarios/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("GrupoID,NombreGrupo,DescripcionGrupo,ComunidadID,TipoGrupo,PersonaContactoPrincipal,TelefonoContactoGrupo,EmailContactoGrupo")] GruposComunitarios gruposComunitarios)
    {
      ModelState.Remove("Comunidad");
      ModelState.Remove("BeneficiarioGrupos");
      ModelState.Remove("ProgramaProyectoGrupos");

      if (ModelState.IsValid)
      {
        _context.Add(gruposComunitarios);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Grupo comunitario creado exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      PopulateComunidadesDropDownList(gruposComunitarios.ComunidadID);
      return View(gruposComunitarios);
    }

    // GET: GruposComunitarios/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var gruposComunitarios = await _context.GruposComunitarios.FindAsync(id);
      if (gruposComunitarios == null)
      {
        return NotFound();
      }
      PopulateComunidadesDropDownList(gruposComunitarios.ComunidadID);
      return View(gruposComunitarios);
    }

    // POST: GruposComunitarios/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("GrupoID,NombreGrupo,DescripcionGrupo,ComunidadID,TipoGrupo,PersonaContactoPrincipal,TelefonoContactoGrupo,EmailContactoGrupo")] GruposComunitarios gruposComunitarios)
    {
      if (id != gruposComunitarios.GrupoID)
      {
        return NotFound();
      }

      ModelState.Remove("Comunidad");
      ModelState.Remove("BeneficiarioGrupos");
      ModelState.Remove("ProgramaProyectoGrupos");

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(gruposComunitarios);
          await _context.SaveChangesAsync();
          TempData["SuccessMessage"] = "Grupo comunitario actualizado exitosamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!GruposComunitariosExists(gruposComunitarios.GrupoID))
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
      PopulateComunidadesDropDownList(gruposComunitarios.ComunidadID);
      return View(gruposComunitarios);
    }

    // GET: GruposComunitarios/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var gruposComunitarios = await _context.GruposComunitarios
          .Include(g => g.Comunidad)
          .FirstOrDefaultAsync(m => m.GrupoID == id);
      if (gruposComunitarios == null)
      {
        return NotFound();
      }

      return View(gruposComunitarios);
    }

    // POST: GruposComunitarios/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var gruposComunitarios = await _context.GruposComunitarios.FindAsync(id);
      if (gruposComunitarios != null)
      {
        _context.GruposComunitarios.Remove(gruposComunitarios);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Grupo comunitario eliminado exitosamente.";
      }

      return RedirectToAction(nameof(Index));
    }

    private bool GruposComunitariosExists(int id)
    {
      return _context.GruposComunitarios.Any(e => e.GrupoID == id);
    }
  }
}
