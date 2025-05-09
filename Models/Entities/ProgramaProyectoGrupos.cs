using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VN_Center.Models.Entities
{
  [Table("ProgramaProyectoGrupos")]
  public class ProgramaProyectoGrupos
  {
    // Clave primaria compuesta
    [Key]
    [Column(Order = 0)]
    public int ProgramaProyectoID { get; set; }

    [Key]
    [Column(Order = 1)]
    public int GrupoID { get; set; }

    // --- Propiedades de Navegación ---
    [ForeignKey("ProgramaProyectoID")]
    public virtual ProgramasProyectosONG ProgramaProyecto { get; set; } = null!;

    [ForeignKey("GrupoID")]
    public virtual GruposComunitarios GrupoComunitario { get; set; } = null!;
  }
}
