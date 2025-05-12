using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity; // Necesario para IdentityUser

namespace VN_Center.Models.Entities
{
  // Ya no necesitamos [Table("UsuariosSistema")] porque Identity lo manejará como AspNetUsers
  // Heredamos de IdentityUser<int> para usar int como tipo de clave primaria
  public class UsuariosSistema : IdentityUser<int>
  {
    // Propiedades que ya vienen con IdentityUser<int> y que podemos omitir o mapear:
    // public int Id { get; set; } // Ya existe como UsuarioID (PK) - Identity lo llama Id
    // public string UserName { get; set; } // Ya existe como NombreUsuario - Identity lo llama UserName
    // public string Email { get; set; } // Ya existe - Identity lo llama Email
    // public string PasswordHash { get; set; } // Ya existe como HashContrasena - Identity lo llama PasswordHash
    // public string PhoneNumber { get; set; } // Ya existe (si lo usaras para el teléfono principal)
    // public bool EmailConfirmed { get; set; }
    // public bool PhoneNumberConfirmed { get; set; }
    // public bool TwoFactorEnabled { get; set; }
    // public DateTimeOffset? LockoutEnd { get; set; }
    // public bool LockoutEnabled { get; set; } // Podríamos usar esto en lugar de nuestro 'Activo'
    // public int AccessFailedCount { get; set; }

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
    // Si quieres mantenerlo separado, está bien.
    [Display(Name = "Usuario Activo")]
    public bool Activo { get; set; } = true;

    [Display(Name = "Último Acceso")]
    [DataType(DataType.DateTime)]
    public DateTime? FechaUltimoAcceso { get; set; }

    // La propiedad RolUsuarioID y la navegación RolesSistema ya no son necesarias aquí.
    // Identity maneja la relación Usuario-Rol a través de la tabla AspNetUserRoles.
    // public int RolUsuarioID { get; set; } 
    // public virtual RolesSistema RolesSistema { get; set; } = null!;


    // --- Propiedades de Navegación Personalizadas (si las tuvieras además de roles) ---
    public virtual ICollection<ProgramasProyectosONG> ProgramasProyectosONGResponsable { get; set; } = new List<ProgramasProyectosONG>();
    public virtual ICollection<SolicitudesInformacionGeneral> SolicitudesInformacionGeneralAsignadas { get; set; } = new List<SolicitudesInformacionGeneral>();

    [NotMapped]
    [Display(Name = "Nombre Completo")]
    public string NombreCompleto
    {
      get { return $"{Nombres} {Apellidos}".Trim(); }
    }

    // Constructor por defecto (puede ser necesario para EF Core o Identity)
    public UsuariosSistema() : base() { }
  }
}
