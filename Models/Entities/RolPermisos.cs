using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VN_Center.Models.Entities
{
  [Table("RolPermisos")]
  public class RolPermisos
  {
    // Clave primaria compuesta
    [Key]
    [Column(Order = 0)]
    [Display(Name = "Rol del Sistema")] // Para el dropdown
    public int RolUsuarioID { get; set; }

    [Key]
    [Column(Order = 1)]
    [Display(Name = "Permiso Asignado")] // Para el dropdown
    public int PermisoID { get; set; }

    // --- Propiedades de Navegación ---
    [ForeignKey("RolUsuarioID")]
    public virtual RolesSistema RolesSistema { get; set; } = null!;

    [ForeignKey("PermisoID")]
    public virtual Permisos Permisos { get; set; } = null!;
  }
}
