using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VN_Center.Models.Entities
{
  [Table("RolPermisos")]
  public class RolPermisos
  {
    // Clave primaria compuesta
    [Key]
    [Column(Order = 0)] // Define el orden de las columnas en la clave compuesta
    public int RolUsuarioID { get; set; }

    [Key]
    [Column(Order = 1)] // Define el orden de las columnas en la clave compuesta
    public int PermisoID { get; set; }

    // Propiedades de navegación a las entidades principales
    [ForeignKey("RolUsuarioID")]
    public virtual RolesSistema RolesSistema { get; set; } = null!;

    [ForeignKey("PermisoID")]
    public virtual Permisos Permisos { get; set; } = null!;
  }
}
