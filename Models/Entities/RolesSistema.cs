using System.Collections.Generic;
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

    [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
    [StringLength(50)]
    [Display(Name = "Nombre del Rol")]
    public string NombreRol { get; set; } = null!;

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Descripción del Rol")]
    [DataType(DataType.MultilineText)]
    public string? DescripcionRol { get; set; }

    // Propiedad de navegación: Un rol puede tener muchos usuarios
    public virtual ICollection<UsuariosSistema> UsuariosSistema { get; set; } = new List<UsuariosSistema>();

    // Propiedad de navegación para la tabla de cruce RolPermisos
    public virtual ICollection<RolPermisos> RolPermisos { get; set; } = new List<RolPermisos>();
  }
}
