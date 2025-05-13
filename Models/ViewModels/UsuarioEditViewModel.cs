using System.ComponentModel.DataAnnotations; // Necesario para DataAnnotations

namespace VN_Center.Models.ViewModels
{
  // ViewModel específico para la vista de edición de usuarios
  public class UsuarioEditViewModel
  {
    // Necesitamos el Id para saber qué usuario estamos editando
    public int Id { get; set; } // Coincide con IdentityUser.Id

    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [Display(Name = "Nombre de Usuario")]
    public string UserName { get; set; } = null!; // Coincide con IdentityUser.UserName

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
    [Display(Name = "Correo Electrónico")]
    public string Email { get; set; } = null!; // Coincide con IdentityUser.Email

    // No incluimos campos de contraseña aquí, la edición de contraseña
    // debe ser un proceso separado por seguridad.

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    [Display(Name = "Nombres")]
    public string Nombres { get; set; } = null!; // Propiedad personalizada de UsuariosSistema

    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(100)]
    [Display(Name = "Apellidos")]
    public string Apellidos { get; set; } = null!; // Propiedad personalizada de UsuariosSistema

    [Display(Name = "Usuario Activo")]
    public bool Activo { get; set; } = true; // Propiedad personalizada de UsuariosSistema

    [Phone(ErrorMessage = "El formato del número de teléfono no es válido.")]
    [Display(Name = "Número de Teléfono")]
    public string? PhoneNumber { get; set; } // Coincide con IdentityUser.PhoneNumber (nullable)

    [Display(Name = "Rol")]
    public string? SelectedRoleName { get; set; } // Para recibir/mostrar el nombre del rol seleccionado en el dropdown
  }
}
