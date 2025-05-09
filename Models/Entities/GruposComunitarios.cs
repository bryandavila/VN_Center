using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VN_Center.Models.Entities
{
  [Table("GruposComunitarios")]
  public class GruposComunitarios
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int GrupoID { get; set; }

    [Required]
    [StringLength(200)]
    public string NombreGrupo { get; set; } = null!;

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? DescripcionGrupo { get; set; }

    public int? ComunidadID { get; set; } // FK, puede ser nulo

    [StringLength(100)]
    public string? TipoGrupo { get; set; }

    [StringLength(200)]
    public string? PersonaContactoPrincipal { get; set; }

    [StringLength(30)]
    public string? TelefonoContactoGrupo { get; set; }

    [StringLength(254)]
    public string? EmailContactoGrupo { get; set; }

    // --- Propiedades de Navegación ---
    [ForeignKey("ComunidadID")]
    public virtual Comunidades? Comunidad { get; set; } // Puede no pertenecer a una comunidad específica

    // Relación muchos-a-muchos con Beneficiarios (a través de BeneficiarioGrupos)
    public virtual ICollection<BeneficiarioGrupos> BeneficiarioGrupos { get; set; } = new List<BeneficiarioGrupos>();

    // Relación muchos-a-muchos con ProgramasProyectosONG (a través de ProgramaProyectoGrupos)
    public virtual ICollection<ProgramaProyectoGrupos> ProgramaProyectoGrupos { get; set; } = new List<ProgramaProyectoGrupos>();
  }
}
