using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VN_Center.Data;
using VN_Center.Models.Entities;
using Microsoft.AspNetCore.Identity; // Necesario para UserManager y RoleManager
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic; // Para List
using VN_Center.Models.ViewModels; // Necesitamos los ViewModels definidos antes

namespace VN_Center.Controllers
{
  // TODO: Considerar añadir autorización [Authorize(Roles = "Administrador")] si es necesario
  public class UsuariosSistemaController : Controller
  {
    private readonly VNCenterDbContext _context; // Aún podemos necesitar el context para otras cosas
    private readonly UserManager<UsuariosSistema> _userManager; // Inyectar UserManager
    private readonly RoleManager<RolesSistema> _roleManager;   // Inyectar RoleManager

    // Constructor actualizado para inyectar UserManager y RoleManager
    public UsuariosSistemaController(VNCenterDbContext context, UserManager<UsuariosSistema> userManager, RoleManager<RolesSistema> roleManager)
    {
      _context = context;
      _userManager = userManager;
      _roleManager = roleManager;
    }

    // GET: UsuariosSistema
    public async Task<IActionResult> Index()
    {
      // Usar UserManager para obtener la lista de usuarios
      // Ya no necesitamos Include(u => u.RolesSistema) aquí, Identity lo maneja diferente
      var usuarios = await _userManager.Users.ToListAsync();
      return View(usuarios); // Pasamos la lista de entidades UsuariosSistema
    }

    // GET: UsuariosSistema/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      // Usar UserManager para encontrar por ID (el ID es int, pero UserManager lo espera como string)
      var usuario = await _userManager.FindByIdAsync(id.Value.ToString());
      if (usuario == null)
      {
        return NotFound();
      }

      // Obtener los roles del usuario usando UserManager
      var roles = await _userManager.GetRolesAsync(usuario);
      ViewBag.UserRoles = roles; // Pasar la lista de nombres de roles a la vista

      // Pasamos la entidad UsuariosSistema a la vista
      return View(usuario);
    }

    // GET: UsuariosSistema/Create
    public async Task<IActionResult> Create()
    {
      // Usar RoleManager para obtener los roles disponibles
      // Pasamos la lista de roles al ViewData para el dropdown
      // Usamos Name como valor y texto porque UserManager trabaja con nombres de rol
      ViewData["RolesList"] = new SelectList(await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync(), "Name", "Name");
      // Retornamos la vista con un ViewModel vacío
      return View(new UsuarioCreateViewModel());
    }

    // POST: UsuariosSistema/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    // Cambiado: Recibimos el ViewModel en lugar de la entidad y la contraseña separada
    public async Task<IActionResult> Create(UsuarioCreateViewModel viewModel)
    {
      // Ya no necesitamos ModelState.Remove

      // Validamos el ViewModel
      if (ModelState.IsValid)
      {
        // Mapear ViewModel a la entidad UsuariosSistema
        var usuario = new UsuariosSistema
        {
          UserName = viewModel.UserName, // Usar UserName (de IdentityUser)
          Email = viewModel.Email,
          Nombres = viewModel.Nombres,
          Apellidos = viewModel.Apellidos,
          Activo = viewModel.Activo,
          EmailConfirmed = true, // O manejar confirmación de email si es necesario
          PhoneNumber = viewModel.PhoneNumber,
          // No asignamos PasswordHash directamente, UserManager lo hace
          // FechaUltimoAcceso se inicializa como null por defecto
        };

        // Crear el usuario usando UserManager (maneja el hash de la contraseña)
        // Usamos viewModel.Password
        var result = await _userManager.CreateAsync(usuario, viewModel.Password);

        if (result.Succeeded)
        {
          // Asignar el rol seleccionado (si se seleccionó uno)
          if (!string.IsNullOrEmpty(viewModel.SelectedRoleName))
          {
            // Verificar que el rol exista antes de asignarlo
            if (await _roleManager.RoleExistsAsync(viewModel.SelectedRoleName))
            {
              await _userManager.AddToRoleAsync(usuario, viewModel.SelectedRoleName);
            }
            else
            {
              // Si el rol no existe (raro con un dropdown), añadir error y volver a la vista
              ModelState.AddModelError("SelectedRoleName", "El rol seleccionado no es válido.");
              ViewData["RolesList"] = new SelectList(await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync(), "Name", "Name", viewModel.SelectedRoleName);
              return View(viewModel);
            }
          }
          TempData["SuccessMessage"] = "Usuario del sistema creado exitosamente."; // Mantenemos TempData
          return RedirectToAction(nameof(Index));
        }
        // Si hubo errores al crear el usuario (ej: contraseña no cumple requisitos, UserName duplicado), añadirlos al ModelState
        foreach (var error in result.Errors)
        {
          ModelState.AddModelError(string.Empty, error.Description);
        }
      }
      // Si el ModelState no es válido (o la creación falló), volver a mostrar el formulario
      // Repoblar la lista de roles
      ViewData["RolesList"] = new SelectList(await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync(), "Name", "Name", viewModel.SelectedRoleName);
      return View(viewModel);
    }

    // GET: UsuariosSistema/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      // Usar UserManager para encontrar el usuario
      var usuario = await _userManager.FindByIdAsync(id.Value.ToString());
      if (usuario == null)
      {
        return NotFound();
      }

      // Obtener los roles actuales del usuario
      var userRoles = await _userManager.GetRolesAsync(usuario);
      // Asumimos un solo rol por usuario para simplificar el ViewModel (si puedes tener múltiples, el ViewModel y la lógica cambian)
      var currentRoleName = userRoles.FirstOrDefault();

      // Crear y poblar el ViewModel para la edición
      var viewModel = new UsuarioEditViewModel
      {
        Id = usuario.Id, // Usar Id (de IdentityUser)
        UserName = usuario.UserName, // Usar UserName
        Email = usuario.Email,
        Nombres = usuario.Nombres,
        Apellidos = usuario.Apellidos,
        Activo = usuario.Activo,
        PhoneNumber = usuario.PhoneNumber,
        SelectedRoleName = currentRoleName // Asignar el rol actual al ViewModel
      };

      // Pasar la lista de roles disponibles al ViewData
      ViewData["RolesList"] = new SelectList(await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync(), "Name", "Name", viewModel.SelectedRoleName);
      // No necesitamos pasar HashContrasena
      return View(viewModel); // Pasar el ViewModel a la vista
    }

    // POST: UsuariosSistema/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    // Cambiado: Recibimos el ViewModel. Quitamos FechaUltimoAcceso y NewPassword del Bind/parámetros.
    public async Task<IActionResult> Edit(int id, UsuarioEditViewModel viewModel)
    {
      // Validar que el ID de la ruta coincida con el ID del ViewModel
      if (id != viewModel.Id) // Usar Id
      {
        return NotFound();
      }

      // Ya no necesitamos ModelState.Remove

      if (ModelState.IsValid)
      {
        // Obtener el usuario actual de la base de datos usando UserManager
        var usuario = await _userManager.FindByIdAsync(id.ToString());
        if (usuario == null)
        {
          return NotFound();
        }

        // Actualizar las propiedades del usuario desde el ViewModel
        // Es buena práctica verificar si el UserName o Email han cambiado y si son únicos si es necesario
        // var oldEmail = usuario.Email; // Guardar por si necesitas lógica extra
        usuario.UserName = viewModel.UserName; // Usar UserName
        usuario.Email = viewModel.Email;
        usuario.Nombres = viewModel.Nombres;
        usuario.Apellidos = viewModel.Apellidos;
        usuario.Activo = viewModel.Activo;
        usuario.PhoneNumber = viewModel.PhoneNumber;
        // No actualizamos FechaUltimoAcceso aquí (generalmente es automático o no editable)
        // **Importante**: No actualizamos la contraseña aquí. Se debe hacer en una acción separada.

        // Actualizar el usuario usando UserManager
        var result = await _userManager.UpdateAsync(usuario);

        if (result.Succeeded)
        {
          // Actualizar el rol del usuario
          var currentRoles = await _userManager.GetRolesAsync(usuario);
          var newRoleName = viewModel.SelectedRoleName; // El rol seleccionado en el dropdown

          // Comprobar si el rol ha cambiado
          if (!currentRoles.Contains(newRoleName ?? string.Empty) || (currentRoles.Any() && string.IsNullOrEmpty(newRoleName)))
          {
            // Quitar todos los roles anteriores (simplificando a un solo rol)
            if (currentRoles.Any())
            {
              await _userManager.RemoveFromRolesAsync(usuario, currentRoles);
            }
            // Añadir el nuevo rol (si se seleccionó uno y existe)
            if (!string.IsNullOrEmpty(newRoleName))
            {
              if (await _roleManager.RoleExistsAsync(newRoleName))
              {
                await _userManager.AddToRoleAsync(usuario, newRoleName);
              }
              else
              {
                // Si el rol seleccionado no existe, añadir error y volver
                ModelState.AddModelError("SelectedRoleName", "El rol seleccionado no es válido.");
                ViewData["RolesList"] = new SelectList(await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync(), "Name", "Name", viewModel.SelectedRoleName);
                return View(viewModel);
              }
            }
          }
          // Si el rol no cambió, no hacemos nada con los roles.

          TempData["SuccessMessage"] = "Usuario del sistema actualizado exitosamente."; // Mantenemos TempData
          return RedirectToAction(nameof(Index));
        }

        // Si hubo errores al actualizar el usuario (ej: UserName/Email duplicado), añadirlos al ModelState
        foreach (var error in result.Errors)
        {
          ModelState.AddModelError(string.Empty, error.Description);
        }
      }
      // Si el ModelState no es válido (o la actualización falló), volver a mostrar el formulario
      // Repoblar la lista de roles
      ViewData["RolesList"] = new SelectList(await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync(), "Name", "Name", viewModel.SelectedRoleName);
      return View(viewModel); // Devolver el ViewModel con los errores
    }


    // GET: UsuariosSistema/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      // Usar UserManager para encontrar el usuario
      var usuario = await _userManager.FindByIdAsync(id.Value.ToString());
      if (usuario == null)
      {
        return NotFound();
      }

      // Obtener roles para mostrar en la vista de confirmación
      ViewBag.UserRoles = await _userManager.GetRolesAsync(usuario);

      return View(usuario); // Pasar la entidad a la vista de confirmación
    }

    // POST: UsuariosSistema/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      // Usar UserManager para encontrar el usuario
      var usuario = await _userManager.FindByIdAsync(id.ToString());
      if (usuario != null)
      {
        // Usar UserManager para eliminar el usuario
        // Identity se encarga de eliminar relaciones en AspNetUserRoles, etc.
        var result = await _userManager.DeleteAsync(usuario);
        if (result.Succeeded)
        {
          TempData["SuccessMessage"] = "Usuario del sistema eliminado exitosamente."; // Mantenemos TempData
        }
        else
        {
          // Si la eliminación falla, añadir errores (podrían mostrarse en Index con TempData)
          TempData["ErrorMessage"] = "No se pudo eliminar el usuario. " + string.Join(" ", result.Errors.Select(e => e.Description));
          // Podrías redirigir a la vista Delete de nuevo mostrando los errores, o al Index mostrando el TempData
          // return View(usuario); // O redirigir a Index
        }
      }
      else
      {
        TempData["ErrorMessage"] = "No se encontró el usuario a eliminar.";
      }

      return RedirectToAction(nameof(Index));
    }

    // Ya no necesitamos el método PopulateRolesDropDownList
    // Ya no necesitamos el método UsuariosSistemaExists (UserManager se encarga de encontrar usuarios)
  }

  // --- Recordatorio: Asegúrate de tener estos ViewModels ---
  // --- en una carpeta Models/ViewModels ---

  // ViewModel para la vista Create
  // public class UsuarioCreateViewModel { /* ... propiedades ... */ }

  // ViewModel para la vista Edit
  // public class UsuarioEditViewModel { /* ... propiedades ... */ }
}
