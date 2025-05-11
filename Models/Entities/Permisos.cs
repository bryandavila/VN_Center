using System.Collections.Generic;
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

    [Required(ErrorMessage = "El nombre del permiso es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre del permiso no puede exceder los 100 caracteres.")]
    [Display(Name = "Nombre del Permiso (Clave)")]
    public string NombrePermiso { get; set; } = null!; // Ej: "Beneficiario_Crear", "Usuario_Listar"

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Descripción del Permiso")]
    [DataType(DataType.MultilineText)]
    public string? DescripcionPermiso { get; set; }

    // Propiedad de navegación para la tabla de cruce RolPermisos
    public virtual ICollection<RolPermisos> RolPermisos { get; set; } = new List<RolPermisos>();
  }
}
