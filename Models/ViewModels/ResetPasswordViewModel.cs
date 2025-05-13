using System.ComponentModel.DataAnnotations;

namespace VN_Center.Models.ViewModels
{
  public class ResetPasswordViewModel
  {
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y como máximo {1} caracteres.", MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva Contraseña")]
    public string Password { get; set; } = null!;

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar Nueva Contraseña")]
    [Compare("Password", ErrorMessage = "La nueva contraseña y la contraseña de confirmación no coinciden.")]
    public string ConfirmPassword { get; set; } = null!;

    // Este campo es crucial y debe ser enviado desde el enlace del correo electrónico
    [Required]
    public string Token { get; set; } = null!;
  }
}
