using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VN_Center.Models.Entities
{
  [Table("RolesSistema")]
  public class RolesSistema
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RolUsuarioID { get; set; }

    [Required]
    [StringLength(50)]
    public string NombreRol { get; set; } = null!;

    [Column(TypeName = "NVARCHAR(MAX)")] // Especificar tipo de dato para campos MAX
    public string? DescripcionRol { get; set; } // Puede ser nulo

    // Propiedad de navegación: Un rol puede tener muchos usuarios
    public virtual ICollection<UsuariosSistema> UsuariosSistema { get; set; } = new List<UsuariosSistema>();

    // Propiedad de navegación para la tabla de cruce RolPermisos
    public virtual ICollection<RolPermisos> RolPermisos { get; set; } = new List<RolPermisos>();
  }
}
