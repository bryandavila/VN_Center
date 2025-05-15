// VN_Center/Models/ViewModels/UsuarioSistemaViewModel.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Para DisplayName si lo necesitas aquí

namespace VN_Center.Models.ViewModels
{
  public class UsuarioSistemaViewModel
  {
    public string Id { get; set; } = string.Empty; // Identity IDs son strings, aunque tu PK sea int

    [Display(Name = "Nombre de Usuario")]
    public string? UserName { get; set; }

    [Display(Name = "Nombres")]
    public string Nombres { get; set; } = string.Empty;

    [Display(Name = "Apellidos")]
    public string Apellidos { get; set; } = string.Empty;

    [Display(Name = "Nombre Completo")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Display(Name = "Correo Electrónico")]
    public string? Email { get; set; }

    [Display(Name = "Correo Confirmado")]
    public bool EmailConfirmed { get; set; }

    [Display(Name = "Activo (Personalizado)")] // Tu propiedad personalizada
    public bool Activo { get; set; }

    [Display(Name = "Cuenta Bloqueada (Identity)")] // De LockoutEnd
    public bool IsLockedOut { get; set; }

    public DateTimeOffset? LockoutEndDateUtc { get; set; } // Para referencia

    public IList<string> Roles { get; set; } = new List<string>();
  }
}
