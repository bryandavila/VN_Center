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
    public class RolesSistemaController : Controller
    {
        private readonly VNCenterDbContext _context;

        public RolesSistemaController(VNCenterDbContext context)
        {
            _context = context;
        }

        // GET: RolesSistema
        public async Task<IActionResult> Index()
        {
            return View(await _context.RolesSistema.ToListAsync());
        }

        // GET: RolesSistema/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rolesSistema = await _context.RolesSistema
                .FirstOrDefaultAsync(m => m.RolUsuarioID == id);
            if (rolesSistema == null)
            {
                return NotFound();
            }

            return View(rolesSistema);
        }

        // GET: RolesSistema/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RolesSistema/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RolUsuarioID,NombreRol,DescripcionRol")] RolesSistema rolesSistema)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rolesSistema);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(rolesSistema);
        }

        // GET: RolesSistema/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rolesSistema = await _context.RolesSistema.FindAsync(id);
            if (rolesSistema == null)
            {
                return NotFound();
            }
            return View(rolesSistema);
        }

        // POST: RolesSistema/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RolUsuarioID,NombreRol,DescripcionRol")] RolesSistema rolesSistema)
        {
            if (id != rolesSistema.RolUsuarioID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rolesSistema);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RolesSistemaExists(rolesSistema.RolUsuarioID))
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
            return View(rolesSistema);
        }

        // GET: RolesSistema/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rolesSistema = await _context.RolesSistema
                .FirstOrDefaultAsync(m => m.RolUsuarioID == id);
            if (rolesSistema == null)
            {
                return NotFound();
            }

            return View(rolesSistema);
        }

        // POST: RolesSistema/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rolesSistema = await _context.RolesSistema.FindAsync(id);
            if (rolesSistema != null)
            {
                _context.RolesSistema.Remove(rolesSistema);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RolesSistemaExists(int id)
        {
            return _context.RolesSistema.Any(e => e.RolUsuarioID == id);
        }
    }
}
