// VN_Center/Models/Entities/ProgramasProyectosONG.cs
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

    [Required(ErrorMessage = "El nombre del programa o proyecto es obligatorio.")]
    [StringLength(255)]
    [Display(Name = "Nombre del Programa/Proyecto")]
    public string NombreProgramaProyecto { get; set; } = null!;

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Descripción Detallada")]
    [DataType(DataType.MultilineText)]
    public string? Descripcion { get; set; }

    [StringLength(50)]
    [Display(Name = "Tipo de Iniciativa")]
    public string? TipoIniciativa { get; set; } // Ej: 'Programa Educativo', 'Proyecto de Salud'

    [Column(TypeName = "DATE")]
    [Display(Name = "Fecha de Inicio Estimada")]
    [DataType(DataType.Date)]
    public DateTime? FechaInicioEstimada { get; set; }

    [Column(TypeName = "DATE")]
    [Display(Name = "Fecha de Fin Estimada")]
    [DataType(DataType.Date)]
    public DateTime? FechaFinEstimada { get; set; }

    [Column(TypeName = "DATE")]
    [Display(Name = "Fecha de Inicio Real")]
    [DataType(DataType.Date)]
    public DateTime? FechaInicioReal { get; set; }

    [Column(TypeName = "DATE")]
    [Display(Name = "Fecha de Fin Real")]
    [DataType(DataType.Date)]
    public DateTime? FechaFinReal { get; set; }

    [StringLength(50)]
    [Display(Name = "Estado del Programa/Proyecto")]
    public string? EstadoProgramaProyecto { get; set; } // Ej: 'Planificación', 'En Curso', 'Completado'

    [Display(Name = "Responsable Principal (ONG)")]
    public int? ResponsablePrincipalONGUsuarioID { get; set; } // FK

    // --- NUEVA PROPIEDAD PARA AUDITORÍA Y FILTRADO ---
    [StringLength(450)]
    [Display(Name = "Usuario Creador ID")]
    public string? UsuarioCreadorId { get; set; }
    // --- FIN DE NUEVA PROPIEDAD ---

    // --- Propiedades de Navegación ---
    [ForeignKey("ResponsablePrincipalONGUsuarioID")]
    [Display(Name = "Responsable (ONG)")]
    public virtual UsuariosSistema? ResponsablePrincipalONG { get; set; }

    public virtual ICollection<ProgramaProyectoComunidades> ProgramaProyectoComunidades { get; set; } = new List<ProgramaProyectoComunidades>();
    public virtual ICollection<ProgramaProyectoGrupos> ProgramaProyectoGrupos { get; set; } = new List<ProgramaProyectoGrupos>();
    public virtual ICollection<ParticipacionesActivas> ParticipacionesActivas { get; set; } = new List<ParticipacionesActivas>();
    public virtual ICollection<BeneficiariosProgramasProyectos> BeneficiariosProgramasProyectos { get; set; } = new List<BeneficiariosProgramasProyectos>();
    // Si tienes una relación directa de EvaluacionesPrograma a ProgramasProyectosONG, deberías incluirla aquí.
    // public virtual ICollection<EvaluacionesPrograma> EvaluacionesPrograma { get; set; } = new List<EvaluacionesPrograma>();
  }
}
