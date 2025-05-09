using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VN_Center.Models.Entities
{
  [Table("EvaluacionesPrograma")]
  public class EvaluacionesPrograma
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int EvaluacionID { get; set; }

    public int ParticipacionID { get; set; } // FK

    public DateTime FechaEvaluacion { get; set; } = DateTime.UtcNow;

    [StringLength(255)]
    public string? NombreProgramaUniversidadEvaluador { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? ParteMasGratificante { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? ParteMasDificil { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? RazonesOriginalesParticipacion { get; set; }

    [StringLength(50)]
    public string? ExpectativasOriginalesCumplidas { get; set; } // Escala o Sí/No

    public int? InformacionPreviaUtil { get; set; } // Escala 1-5

    [StringLength(50)]
    public string? EsfuerzoIntegracionComunidades { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? ComentariosAlojamientoHotel { get; set; }

    public int? ProgramaInmersionCulturalAyudoHumildad { get; set; } // Escala 1-5

    [StringLength(20)]
    public string? ActividadesRecreativasCulturalesInteresantes { get; set; } // "Agree", "Disagree"

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? VisitaSitioComunidadFavoritaYPorQue { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? AspectoMasValiosoExperiencia { get; set; }

    public int? AplicaraLoAprendidoFuturo { get; set; } // Escala 1-5

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? TresCosasAprendidasSobreSiMismo { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? ComoCompartiraAprendidoUniversidad { get; set; }

    [StringLength(10)]
    public string? RecomendariaProgramaOtros { get; set; } // "Yes", "No", "Maybe"

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? QueDiraOtrosSobrePrograma { get; set; }

    public bool? PermiteSerUsadoComoReferencia { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? ComentariosAdicionalesEvaluacion { get; set; }

    // --- Propiedad de Navegación ---
    [ForeignKey("ParticipacionID")]
    public virtual ParticipacionesActivas ParticipacionActiva { get; set; } = null!;
  }
}
