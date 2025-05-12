using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity; // Necesario para IdentityRole

namespace VN_Center.Models.Entities
{
  // Ya no necesitamos [Table("RolesSistema")] porque Identity lo manejará como AspNetRoles
  // Heredamos de IdentityRole<int> para usar int como tipo de clave primaria
  public class RolesSistema : IdentityRole<int>
  {
    // Propiedades que ya vienen con IdentityRole<int>:
    // public int Id { get; set; } // Ya existe como RolUsuarioID (PK) - Identity lo llama Id
    // public string Name { get; set; } // Ya existe como NombreRol - Identity lo llama Name
    // public string NormalizedName { get; set; } // Versión normalizada de Name
    // public string ConcurrencyStamp { get; set; } // Para control de concurrencia

    // --- Nuestra Propiedad Personalizada ---
    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Descripción del Rol")]
    [DataType(DataType.MultilineText)]
    public string? DescripcionRol { get; set; }

    // La colección de navegación UsuariosSistema ya no es necesaria aquí.
    // Identity maneja la relación Usuario-Rol.
    // public virtual ICollection<UsuariosSistema> UsuariosSistema { get; set; } = new List<UsuariosSistema>();

    // La colección de navegación RolPermisos se mantiene, ya que es una relación personalizada.
    public virtual ICollection<RolPermisos> RolPermisos { get; set; } = new List<RolPermisos>();

    // Constructor por defecto
    public RolesSistema() : base() { }

    // Constructor para crear un rol con nombre (útil)
    public RolesSistema(string roleName) : base(roleName) { }
  }
}
