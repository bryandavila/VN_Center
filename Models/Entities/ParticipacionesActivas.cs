using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VN_Center.Models.Entities
{
  [Table("ParticipacionesActivas")]
  public class ParticipacionesActivas
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ParticipacionID { get; set; }

    public int SolicitudID { get; set; } // FK

    public int ProgramaProyectoID { get; set; } // FK

    [Column(TypeName = "DATE")]
    public DateTime FechaAsignacion { get; set; } = DateTime.Today; // Default a Today si es apropiado

    [Column(TypeName = "DATE")]
    public DateTime FechaInicioParticipacion { get; set; }

    [Column(TypeName = "DATE")]
    public DateTime? FechaFinParticipacion { get; set; }

    [StringLength(150)]
    public string? RolDesempenado { get; set; }

    public int? HorasTCUCompletadas { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? NotasSupervisor { get; set; }

    // --- Propiedades de Navegación ---
    [ForeignKey("SolicitudID")]
    public virtual Solicitudes Solicitud { get; set; } = null!;

    [ForeignKey("ProgramaProyectoID")]
    public virtual ProgramasProyectosONG ProgramaProyecto { get; set; } = null!;

    // Una ParticipacionActiva puede tener muchas EvaluacionesPrograma
    public virtual ICollection<EvaluacionesPrograma> EvaluacionesPrograma { get; set; } = new List<EvaluacionesPrograma>();
  }
}
