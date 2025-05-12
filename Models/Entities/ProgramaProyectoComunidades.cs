using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VN_Center.Models.Entities
{
  [Table("ProgramaProyectoComunidades")]
  public class ProgramaProyectoComunidades
  {
    // Clave primaria compuesta
    [Key]
    [Column(Order = 0)]
    [Display(Name = "Programa/Proyecto")]
    public int ProgramaProyectoID { get; set; }

    [Key]
    [Column(Order = 1)]
    [Display(Name = "Comunidad")]
    public int ComunidadID { get; set; }

    // --- Propiedades de Navegación ---
    [ForeignKey("ProgramaProyectoID")]
    public virtual ProgramasProyectosONG ProgramaProyecto { get; set; } = null!;

    [ForeignKey("ComunidadID")]
    public virtual Comunidades Comunidad { get; set; } = null!;
  }
}
