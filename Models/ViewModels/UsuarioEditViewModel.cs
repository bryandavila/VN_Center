// VN_Center/Models/ViewModels/UsuarioEditViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace VN_Center.Models.ViewModels
{
  public class UsuarioEditViewModel
  {
    public int Id { get; set; } // El ID del usuario de tu entidad UsuariosSistema

    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [Display(Name = "Nombre de Usuario (Login)")]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
    [Display(Name = "Correo Electrónico")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    [Display(Name = "Nombres")]
    public string Nombres { get; set; } = null!;

    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(100)]
    [Display(Name = "Apellidos")]
    public string Apellidos { get; set; } = null!;

    [Display(Name = "Número de Teléfono")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Usuario Activo")]
    public bool Activo { get; set; }

    [Display(Name = "Rol Asignado")]
    public string? SelectedRoleName { get; set; } // Para el dropdown de roles

    // --- Campos para Restablecimiento de Contraseña por Administrador ---
    [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y como máximo {1} caracteres de longitud.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva Contraseña (Opcional)")]
    public string? NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar Nueva Contraseña")]
    [Compare("NewPassword", ErrorMessage = "La nueva contraseña y la contraseña de confirmación no coinciden.")]
    public string? ConfirmNewPassword { get; set; }
  }
}
