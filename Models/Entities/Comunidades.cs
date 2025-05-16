// VN_Center/Models/Entities/Comunidades.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VN_Center.Models.Entities
{
  [Table("Comunidades")]
  public class Comunidades
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ComunidadID { get; set; }

    [Required(ErrorMessage = "El nombre de la comunidad es obligatorio.")]
    [StringLength(150)]
    [Display(Name = "Nombre de la Comunidad")]
    public string NombreComunidad { get; set; } = null!;

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Ubicación Detallada")]
    public string? UbicacionDetallada { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Notas Adicionales")]
    public string? NotasComunidad { get; set; }

    // --- NUEVA PROPIEDAD PARA AUDITORÍA Y FILTRADO ---
    [StringLength(450)]
    [Display(Name = "Usuario Creador ID")]
    public string? UsuarioCreadorId { get; set; }
    // --- FIN DE NUEVA PROPIEDAD ---

    // Propiedades de navegación
    public virtual ICollection<Beneficiarios> Beneficiarios { get; set; } = new List<Beneficiarios>();
    public virtual ICollection<GruposComunitarios> GruposComunitarios { get; set; } = new List<GruposComunitarios>();
    public virtual ICollection<ProgramaProyectoComunidades> ProgramaProyectoComunidades { get; set; } = new List<ProgramaProyectoComunidades>();
  }
}
