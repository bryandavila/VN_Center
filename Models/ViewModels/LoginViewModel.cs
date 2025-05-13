using System.ComponentModel.DataAnnotations;

namespace VN_Center.Models.ViewModels
{
  public class LoginViewModel
  {
    [Required(ErrorMessage = "El correo electrónico o nombre de usuario es obligatorio.")]
    [Display(Name = "Correo Electrónico o Nombre de Usuario")]
    public string EmailOrUserName { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = null!;

    [Display(Name = "Recordarme")]
    public bool RememberMe { get; set; }
  }
}
