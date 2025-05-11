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

    [Required(ErrorMessage = "Debe seleccionar la participación a evaluar.")]
    [Display(Name = "Participación Evaluada")]
    public int ParticipacionID { get; set; } // FK

    [Display(Name = "Fecha de Evaluación")]
    [DataType(DataType.DateTime)]
    public DateTime FechaEvaluacion { get; set; } = DateTime.UtcNow;

    [StringLength(255)]
    [Display(Name = "Nombre del Programa/Universidad del Evaluador")]
    public string? NombreProgramaUniversidadEvaluador { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Parte Más Gratificante del Programa", Prompt = "Describe qué fue lo más gratificante para ti.")]
    [DataType(DataType.MultilineText)]
    public string? ParteMasGratificante { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Parte Más Difícil del Programa", Prompt = "Describe qué fue lo más desafiante.")]
    [DataType(DataType.MultilineText)]
    public string? ParteMasDificil { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Razones Originales para Participar", Prompt = "Brevemente, ¿por qué decidiste participar?")]
    [DataType(DataType.MultilineText)]
    public string? RazonesOriginalesParticipacion { get; set; }

    [StringLength(50)]
    [Display(Name = "¿Se Cumplieron tus Expectativas Originales?")]
    public string? ExpectativasOriginalesCumplidas { get; set; } // Podría ser un dropdown (Sí, No, Parcialmente)

    [Display(Name = "Utilidad de Información Previa (1-5)", Prompt = "1=Poco útil, 5=Muy útil")]
    [Range(1, 5, ErrorMessage = "El valor debe estar entre 1 y 5.")]
    public int? InformacionPreviaUtil { get; set; }

    [StringLength(50)]
    [Display(Name = "Esfuerzo de Integración en Comunidades")]
    public string? EsfuerzoIntegracionComunidades { get; set; } // Podría ser un dropdown (Excelente, Bueno, Regular, Pobre)

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Comentarios sobre Alojamiento/Hotel", Prompt = "¿Qué te gustó más de cada ubicación? ¿Qué fue desafiante?")]
    [DataType(DataType.MultilineText)]
    public string? ComentariosAlojamientoHotel { get; set; }

    [Display(Name = "Inmersión Cultural y Humildad (1-5)", Prompt = "1=Totalmente en desacuerdo, 5=Totalmente de acuerdo")]
    [Range(1, 5, ErrorMessage = "El valor debe estar entre 1 y 5.")]
    public int? ProgramaInmersionCulturalAyudoHumildad { get; set; }

    [StringLength(20)]
    [Display(Name = "Actividades Recreativas/Culturales Interesantes")]
    public string? ActividadesRecreativasCulturalesInteresantes { get; set; } // Podría ser dropdown (De acuerdo, En desacuerdo)

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Visita de Sitio/Comunidad Favorita y Por Qué", Prompt = "Describe tu visita favorita.")]
    [DataType(DataType.MultilineText)]
    public string? VisitaSitioComunidadFavoritaYPorQue { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Aspecto Más Valioso de la Experiencia", Prompt = "¿Qué fue lo más valioso para ti?")]
    [DataType(DataType.MultilineText)]
    public string? AspectoMasValiosoExperiencia { get; set; }

    [Display(Name = "Aplicaré lo Aprendido (1-5)", Prompt = "1=Totalmente en desacuerdo, 5=Totalmente de acuerdo")]
    [Range(1, 5, ErrorMessage = "El valor debe estar entre 1 y 5.")]
    public int? AplicaraLoAprendidoFuturo { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Tres Cosas que Aprendí Sobre Mí Mismo", Prompt = "Lista tres cosas.")]
    [DataType(DataType.MultilineText)]
    public string? TresCosasAprendidasSobreSiMismo { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "¿Cómo Compartiré lo Aprendido en mi Universidad/Comunidad?", Prompt = "Describe cómo planeas compartir tu experiencia.")]
    [DataType(DataType.MultilineText)]
    public string? ComoCompartiraAprendidoUniversidad { get; set; }

    [StringLength(10)]
    [Display(Name = "¿Recomendarías este Programa a Otros?")]
    public string? RecomendariaProgramaOtros { get; set; } // Podría ser dropdown (Sí, No, Tal vez)

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "¿Qué Dirías a Otros sobre el Programa?", Prompt = "Tus comentarios sobre el programa.")]
    [DataType(DataType.MultilineText)]
    public string? QueDiraOtrosSobrePrograma { get; set; }

    [Display(Name = "¿Podemos Usarte como Referencia?")]
    public bool? PermiteSerUsadoComoReferencia { get; set; } // Sí/No

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Comentarios Adicionales", Prompt = "Cualquier otro comentario o sugerencia.")]
    [DataType(DataType.MultilineText)]
    public string? ComentariosAdicionalesEvaluacion { get; set; }

    // --- Propiedad de Navegación ---
    [ForeignKey("ParticipacionID")]
    public virtual ParticipacionesActivas ParticipacionActiva { get; set; } = null!;
  }
}
