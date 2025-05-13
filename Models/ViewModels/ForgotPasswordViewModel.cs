using System.ComponentModel.DataAnnotations;

namespace VN_Center.Models.ViewModels
{
  public class ForgotPasswordViewModel
  {
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
    [Display(Name = "Correo Electrónico")]
    public string Email { get; set; } = null!;
  }
}
