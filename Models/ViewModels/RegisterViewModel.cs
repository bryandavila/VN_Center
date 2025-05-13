using System.ComponentModel.DataAnnotations;

namespace VN_Center.Models.ViewModels
{
  public class RegisterViewModel
  {
    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [Display(Name = "Nombre de Usuario")]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
    [Display(Name = "Correo Electrónico")]
    public string Email { get; set; } = null!;

    // Añadimos Nombres y Apellidos que son propiedades personalizadas de tu UsuariosSistema
    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(100)]
    [Display(Name = "Nombres")]
    public string Nombres { get; set; } = null!;

    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(100)]
    [Display(Name = "Apellidos")]
    public string Apellidos { get; set; } = null!;

    [Phone(ErrorMessage = "El formato del número de teléfono no es válido.")]
    [Display(Name = "Número de Teléfono (Opcional)")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y como máximo {1} caracteres.", MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = null!;

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar Contraseña")]
    [Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden.")]
    public string ConfirmPassword { get; set; } = null!;

    // Podrías añadir un campo para aceptar términos y condiciones si es necesario
    // [Range(typeof(bool), "true", "true", ErrorMessage = "Debe aceptar los términos y condiciones.")]
    // public bool AgreeToTerms { get; set; }
  }
}
