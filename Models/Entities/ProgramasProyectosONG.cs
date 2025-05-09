using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VN_Center.Models.Entities
{
  [Table("ProgramasProyectosONG")]
  public class ProgramasProyectosONG
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProgramaProyectoID { get; set; }

    [Required]
    [StringLength(255)]
    public string NombreProgramaProyecto { get; set; } = null!;

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? Descripcion { get; set; }

    [StringLength(50)]
    public string? TipoIniciativa { get; set; }

    [Column(TypeName = "DATE")]
    public DateTime? FechaInicioEstimada { get; set; }

    [Column(TypeName = "DATE")]
    public DateTime? FechaFinEstimada { get; set; }

    [Column(TypeName = "DATE")]
    public DateTime? FechaInicioReal { get; set; }

    [Column(TypeName = "DATE")]
    public DateTime? FechaFinReal { get; set; }

    [StringLength(50)]
    public string? EstadoProgramaProyecto { get; set; }

    public int? ResponsablePrincipalONGUsuarioID { get; set; } // FK, puede ser nulo

    // --- Propiedades de Navegación ---
    [ForeignKey("ResponsablePrincipalONGUsuarioID")]
    public virtual UsuariosSistema? ResponsablePrincipalONG { get; set; }

    // Relación muchos-a-muchos con Comunidades (a través de ProgramaProyectoComunidades)
    public virtual ICollection<ProgramaProyectoComunidades> ProgramaProyectoComunidades { get; set; } = new List<ProgramaProyectoComunidades>();

    // Relación muchos-a-muchos con GruposComunitarios (a través de ProgramaProyectoGrupos)
    public virtual ICollection<ProgramaProyectoGrupos> ProgramaProyectoGrupos { get; set; } = new List<ProgramaProyectoGrupos>();

    // Relación uno-a-muchos con ParticipacionesActivas
    public virtual ICollection<ParticipacionesActivas> ParticipacionesActivas { get; set; } = new List<ParticipacionesActivas>();

    // Relación muchos-a-muchos con Beneficiarios (a través de BeneficiariosProgramasProyectos)
    public virtual ICollection<BeneficiariosProgramasProyectos> BeneficiariosProgramasProyectos { get; set; } = new List<BeneficiariosProgramasProyectos>();
  }
}
