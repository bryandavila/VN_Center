using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace VN_Center.Models.Entities
{
  // Heredamos de IdentityUser<int> para usar int como tipo de clave primaria
  public class UsuariosSistema : IdentityUser<int>
  {
    // --- Nuestras Propiedades Personalizadas ---
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    [ProtectedPersonalData] // Para GDPR si es necesario
    [Display(Name = "Nombres")]
    public string Nombres { get; set; } = null!;

    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(100)]
    [ProtectedPersonalData]
    [Display(Name = "Apellidos")]
    public string Apellidos { get; set; } = null!;

    // 'Activo' podría mapearse a !LockoutEnabled de IdentityUser.
    [Display(Name = "Usuario Activo")]
    public bool Activo { get; set; } = true;

    [Display(Name = "Último Acceso")]
    [DataType(DataType.DateTime)]
    public DateTime? FechaUltimoAcceso { get; set; }

    // Identity maneja la relación Usuario-Rol a través de la tabla AspNetUserRoles.
    // --- Propiedades de Navegación Personalizadas ---
    public virtual ICollection<ProgramasProyectosONG> ProgramasProyectosONGResponsable { get; set; } = new List<ProgramasProyectosONG>();
    public virtual ICollection<SolicitudesInformacionGeneral> SolicitudesInformacionGeneralAsignadas { get; set; } = new List<SolicitudesInformacionGeneral>();

    [NotMapped]
    [Display(Name = "Nombre Completo")]
    public string NombreCompleto
    {
      get { return $"{Nombres} {Apellidos}".Trim(); }
    }

    public UsuariosSistema() : base() { }
  }
}
