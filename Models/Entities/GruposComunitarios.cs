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

    [Required(ErrorMessage = "El nombre del grupo es obligatorio.")]
    [StringLength(200)]
    [Display(Name = "Nombre del Grupo")]
    public string NombreGrupo { get; set; } = null!;

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Descripción del Grupo")]
    [DataType(DataType.MultilineText)]
    public string? DescripcionGrupo { get; set; }

    [Display(Name = "Comunidad Asociada")]
    public int? ComunidadID { get; set; } // FK, puede ser nulo si el grupo es inter-comunitario

    [StringLength(100)]
    [Display(Name = "Tipo de Grupo")]
    public string? TipoGrupo { get; set; } // Ej: Mujeres, Jóvenes, Agricultores, etc.

    [StringLength(200)]
    [Display(Name = "Persona de Contacto Principal")]
    public string? PersonaContactoPrincipal { get; set; }

    [StringLength(30)]
    [Display(Name = "Teléfono de Contacto del Grupo")]
    public string? TelefonoContactoGrupo { get; set; }

    [EmailAddress(ErrorMessage = "Formato de correo no válido.")]
    [StringLength(254)]
    [Display(Name = "Email de Contacto del Grupo")]
    public string? EmailContactoGrupo { get; set; }

    // --- Propiedades de Navegación ---
    [ForeignKey("ComunidadID")]
    [Display(Name = "Comunidad")]
    public virtual Comunidades? Comunidad { get; set; }

    public virtual ICollection<BeneficiarioGrupos> BeneficiarioGrupos { get; set; } = new List<BeneficiarioGrupos>();
    public virtual ICollection<ProgramaProyectoGrupos> ProgramaProyectoGrupos { get; set; } = new List<ProgramaProyectoGrupos>();
  }
}
