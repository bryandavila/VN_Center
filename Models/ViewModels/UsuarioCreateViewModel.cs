using System.ComponentModel.DataAnnotations; // Necesario para DataAnnotations como [Required], [EmailAddress], etc.

namespace VN_Center.Models.ViewModels
{
  // ViewModel específico para la vista de creación de usuarios
  public class UsuarioCreateViewModel
  {
    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [Display(Name = "Nombre de Usuario")]
    public string UserName { get; set; } = null!; // Coincide con IdentityUser.UserName

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
    [Display(Name = "Correo Electrónico")]
    public string Email { get; set; } = null!; // Coincide con IdentityUser.Email

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y como máximo {1} caracteres.", MinimumLength = 6)] // Requisito mínimo de Identity por defecto
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = null!; // Para recibir la contraseña en texto plano

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar Contraseña")]
    [Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden.")]
    public string ConfirmPassword { get; set; } = null!; // Para validar la confirmación

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
    public string? SelectedRoleName { get; set; } // Para recibir el nombre del rol seleccionado en el dropdown
  }
}
