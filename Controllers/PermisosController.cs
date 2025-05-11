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
    public class PermisosController : Controller
    {
        private readonly VNCenterDbContext _context;

        public PermisosController(VNCenterDbContext context)
        {
            _context = context;
        }

        // GET: Permisos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Permisos.ToListAsync());
        }

        // GET: Permisos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permisos = await _context.Permisos
                .FirstOrDefaultAsync(m => m.PermisoID == id);
            if (permisos == null)
            {
                return NotFound();
            }

            return View(permisos);
        }

        // GET: Permisos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Permisos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PermisoID,NombrePermiso,DescripcionPermiso")] Permisos permisos)
        {
            if (ModelState.IsValid)
            {
                _context.Add(permisos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(permisos);
        }

        // GET: Permisos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permisos = await _context.Permisos.FindAsync(id);
            if (permisos == null)
            {
                return NotFound();
            }
            return View(permisos);
        }

        // POST: Permisos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PermisoID,NombrePermiso,DescripcionPermiso")] Permisos permisos)
        {
            if (id != permisos.PermisoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(permisos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PermisosExists(permisos.PermisoID))
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
            return View(permisos);
        }

        // GET: Permisos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permisos = await _context.Permisos
                .FirstOrDefaultAsync(m => m.PermisoID == id);
            if (permisos == null)
            {
                return NotFound();
            }

            return View(permisos);
        }

        // POST: Permisos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var permisos = await _context.Permisos.FindAsync(id);
            if (permisos != null)
            {
                _context.Permisos.Remove(permisos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PermisosExists(int id)
        {
            return _context.Permisos.Any(e => e.PermisoID == id);
        }
    }
}
