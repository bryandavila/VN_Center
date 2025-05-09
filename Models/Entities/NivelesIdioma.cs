using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VN_Center.Models.Entities
{
  [Table("NivelesIdioma")] // Especifica el nombre exacto de la tabla en la BD
  public class NivelesIdioma
  {
    [Key] // Marca NivelIdiomaID como la clave primaria
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Indica que es autoincremental
    public int NivelIdiomaID { get; set; }

    [Required] // Indica que NombreNivel no puede ser nulo
    [StringLength(50)] // Coincide con NVARCHAR(50) y UNIQUE en tu script SQL
    public string NombreNivel { get; set; } = null!; // Inicializar a null! para C# nullable reference types

    // Propiedad de navegación: Un NivelIdioma puede estar asociado a muchas Solicitudes
    // Esto es opcional si no necesitas navegar desde NivelIdioma a Solicitudes directamente,
    // pero es útil para definir la relación.
    public virtual ICollection<Solicitudes> Solicitudes { get; set; } = new List<Solicitudes>();
  }
}
