using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Aún necesario para DbUpdateConcurrencyException
using VN_Center.Data;
using VN_Center.Models.Entities;
using Microsoft.AspNetCore.Identity; // Necesario para RoleManager

namespace VN_Center.Controllers
{
  // TODO: Considerar añadir autorización [Authorize(Roles = "Administrador")] si es necesario
  public class RolesSistemaController : Controller
  {
    // Inyectar RoleManager. Mantenemos el DbContext por si se usa para otras cosas no relacionadas con Identity directamente.
    private readonly RoleManager<RolesSistema> _roleManager;
    private readonly VNCenterDbContext _context; // Podría ser necesario para relaciones como RolPermisos

    public RolesSistemaController(RoleManager<RolesSistema> roleManager, VNCenterDbContext context)
    {
      _roleManager = roleManager;
      _context = context;
    }

    // GET: RolesSistema
    public async Task<IActionResult> Index()
    {
      // Usar RoleManager para obtener la lista de roles
      var roles = await _roleManager.Roles.ToListAsync();
      return View(roles); // Pasar la lista de entidades RolesSistema
    }

    // GET: RolesSistema/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      // Usar RoleManager para encontrar por ID (el ID es int, pero RoleManager lo espera como string)
      var rol = await _roleManager.FindByIdAsync(id.Value.ToString());
      if (rol == null)
      {
        return NotFound();
      }

      // Aquí podríamos cargar los permisos asociados a este rol si fuera necesario para los detalles
      // Ejemplo: await _context.Entry(rol).Collection(r => r.RolPermisos).LoadAsync();
      // O cargar los permisos directamente: ViewBag.Permisos = await _context.RolPermisos.Include(rp => rp.Permisos).Where(rp => rp.RolUsuarioID == id).Select(rp => rp.Permisos.NombrePermiso).ToListAsync();

      return View(rol); // Pasar la entidad RolesSistema
    }

    // GET: RolesSistema/Create
    public IActionResult Create()
    {
      // Simplemente mostramos la vista con un modelo vacío (o un ViewModel si fuera necesario)
      return View(new RolesSistema()); // Pasar una nueva instancia para el formulario
    }

    // POST: RolesSistema/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    // Usamos Bind para incluir solo las propiedades que queremos crear/modificar
    // Usamos Name (de IdentityRole) en lugar de NombreRol
    public async Task<IActionResult> Create([Bind("Name,DescripcionRol")] RolesSistema rolViewModel)
    {
      // El Id se genera automáticamente
      // ModelState.Remove("RolPermisos"); // Si la entidad tiene colecciones, excluirlas de la validación inicial si no se manejan aquí

      // Validamos el Name manualmente si queremos un mensaje específico para duplicados antes de llamar a RoleManager
      if (await _roleManager.RoleExistsAsync(rolViewModel.Name))
      {
        ModelState.AddModelError("Name", "Ya existe un rol con este nombre.");
      }

      if (ModelState.IsValid)
      {
        // Crear una nueva instancia de RolesSistema (importante si el modelo enlazado no es la entidad completa)
        var nuevoRol = new RolesSistema
        {
          Name = rolViewModel.Name, // Usar Name
          DescripcionRol = rolViewModel.DescripcionRol
          // NormalizedName será establecido por RoleManager
        };

        // Usar RoleManager para crear el rol
        var result = await _roleManager.CreateAsync(nuevoRol);

        if (result.Succeeded)
        {
          TempData["SuccessMessage"] = "Rol del sistema creado exitosamente.";
          return RedirectToAction(nameof(Index));
        }
        // Si hubo errores (ej: nombre duplicado detectado por Identity), añadirlos al ModelState
        foreach (var error in result.Errors)
        {
          // Evitar añadir el error de duplicado si ya lo añadimos manualmente
          if (!(error.Code == "DuplicateRoleName" && ModelState.ContainsKey("Name")))
          {
            ModelState.AddModelError(string.Empty, error.Description);
          }
        }
      }
      // Si el ModelState no es válido o la creación falló, volver a mostrar el formulario
      return View(rolViewModel); // Devolver el ViewModel/modelo con los errores
    }

    // GET: RolesSistema/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      // Usar RoleManager para encontrar el rol
      var rol = await _roleManager.FindByIdAsync(id.Value.ToString());
      if (rol == null)
      {
        return NotFound();
      }
      // Pasar la entidad RolesSistema a la vista de edición
      return View(rol);
    }

    // POST: RolesSistema/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    // Usamos Bind para incluir solo las propiedades que queremos modificar
    // Usamos Id y Name (de IdentityRole)
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,DescripcionRol")] RolesSistema rolViewModel)
    {
      // Validar que el ID de la ruta coincida con el ID del modelo enlazado
      if (id != rolViewModel.Id) // Usar Id
      {
        return NotFound();
      }

      // ModelState.Remove("RolPermisos"); // Excluir colecciones si es necesario

      // Validar nombre duplicado si cambió (evita error si solo se cambia descripción)
      var rolOriginal = await _roleManager.FindByIdAsync(id.ToString());
      if (rolOriginal == null) return NotFound();

      if (rolOriginal.Name != rolViewModel.Name && await _roleManager.RoleExistsAsync(rolViewModel.Name))
      {
        ModelState.AddModelError("Name", "Ya existe otro rol con este nombre.");
      }


      if (ModelState.IsValid)
      {
        try
        {
          // Obtener el rol actual de la base de datos usando RoleManager
          var rol = await _roleManager.FindByIdAsync(id.ToString());
          if (rol == null)
          {
            return NotFound();
          }

          // Actualizar las propiedades desde el modelo enlazado
          rol.Name = rolViewModel.Name; // Actualizar Name
          rol.DescripcionRol = rolViewModel.DescripcionRol;
          // NormalizedName será actualizado por RoleManager

          // Usar RoleManager para actualizar el rol
          var result = await _roleManager.UpdateAsync(rol);

          if (result.Succeeded)
          {
            TempData["SuccessMessage"] = "Rol del sistema actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
          }
          // Si hubo errores, añadirlos al ModelState
          foreach (var error in result.Errors)
          {
            if (!(error.Code == "DuplicateRoleName" && ModelState.ContainsKey("Name")))
            {
              ModelState.AddModelError(string.Empty, error.Description);
            }
          }
        }
        catch (DbUpdateConcurrencyException)
        {
          // Verificar si el rol todavía existe usando RoleManager
          if (!await RoleExistsAsync(rolViewModel.Id)) // Usar Id
          {
            return NotFound();
          }
          else
          {
            // Podríamos añadir un error al ModelState indicando conflicto de concurrencia
            ModelState.AddModelError(string.Empty, "El rol fue modificado por otro usuario. Por favor, recarga la página e intenta de nuevo.");
            // O simplemente lanzar la excepción si queremos que falle
            // throw;
          }
        }
      }
      // Si el ModelState no es válido o la actualización falló, volver a mostrar el formulario
      return View(rolViewModel); // Devolver el modelo con los errores
    }

    // GET: RolesSistema/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      // Usar RoleManager para encontrar el rol
      var rol = await _roleManager.FindByIdAsync(id.Value.ToString());
      if (rol == null)
      {
        return NotFound();
      }

      // Podríamos verificar si hay usuarios asignados a este rol antes de permitir borrarlo
      // var usersInRole = await _userManager.GetUsersInRoleAsync(rol.Name);
      // if (usersInRole.Any()) ViewBag.HasUsers = true; else ViewBag.HasUsers = false;

      return View(rol); // Pasar la entidad a la vista de confirmación
    }

    // POST: RolesSistema/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      // Usar RoleManager para encontrar el rol
      var rol = await _roleManager.FindByIdAsync(id.ToString());
      if (rol != null)
      {
        // Antes de eliminar, podrías querer eliminar dependencias (como RolPermisos)
        // var permisosAsociados = await _context.RolPermisos.Where(rp => rp.RolUsuarioID == id).ToListAsync();
        // _context.RolPermisos.RemoveRange(permisosAsociados);
        // await _context.SaveChangesAsync(); // Guardar eliminación de permisos

        // Usar RoleManager para eliminar el rol
        var result = await _roleManager.DeleteAsync(rol);
        if (result.Succeeded)
        {
          TempData["SuccessMessage"] = "Rol del sistema eliminado exitosamente.";
          return RedirectToAction(nameof(Index));
        }
        else
        {
          // Si la eliminación falla (raro, a menos que haya dependencias inesperadas o concurrencia)
          TempData["ErrorMessage"] = "No se pudo eliminar el rol. " + string.Join(" ", result.Errors.Select(e => e.Description));
          // Podrías redirigir a la vista Delete de nuevo mostrando los errores
          // return View(rol); // O redirigir a Index mostrando el TempData
        }
      }
      else
      {
        TempData["ErrorMessage"] = "No se encontró el rol a eliminar.";
      }

      return RedirectToAction(nameof(Index));
    }

    // Método auxiliar para verificar existencia usando RoleManager (adapta el método original)
    private async Task<bool> RoleExistsAsync(int id)
    {
      return await _roleManager.FindByIdAsync(id.ToString()) != null;
    }

    // El método RolesSistemaExists original ya no es necesario con RoleManager
  }
}
