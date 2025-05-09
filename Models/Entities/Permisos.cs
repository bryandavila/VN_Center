using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VN_Center.Models.Entities
{
  [Table("Permisos")]
  public class Permisos
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PermisoID { get; set; }

    [Required]
    [StringLength(100)]
    public string NombrePermiso { get; set; } = null!;

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? DescripcionPermiso { get; set; }

    // Propiedad de navegación para la tabla de cruce RolPermisos
    public virtual ICollection<RolPermisos> RolPermisos { get; set; } = new List<RolPermisos>();
  }
}
