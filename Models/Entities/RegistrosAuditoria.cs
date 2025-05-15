// VN_Center/Models/Entities/RegistrosAuditoria.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity; // Para IdentityUser si es necesario referenciarlo

namespace VN_Center.Models.Entities
{
  [Table("RegistrosAuditoria")]
  public class RegistrosAuditoria
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AuditoriaID { get; set; }

    [Required]
    [Display(Name = "Fecha y Hora")]
    public DateTime FechaHoraEvento { get; set; } = DateTime.UtcNow;

    // Quién realizó la acción (el usuario administrador o el sistema)
    // Podría ser el ID del usuario que realiza la acción.
    // Si es una acción del sistema (ej. auto-bloqueo), podría ser un valor especial o nulo.
    [StringLength(450)] // Longitud típica para IDs de Identity
    [Display(Name = "Usuario Ejecutor ID")]
    public string? UsuarioEjecutorId { get; set; }

    [StringLength(256)] // Basado en la longitud de UserName en Identity
    [Display(Name = "Nombre Usuario Ejecutor")]
    public string? NombreUsuarioEjecutor { get; set; } // Para facilitar la visualización

    [Required]
    [StringLength(100)]
    [Display(Name = "Tipo de Evento")]
    public string TipoEvento { get; set; } = null!;
    // Ejemplos: "CreacionUsuario", "ActualizacionUsuario", "CambioRol", "RestablecimientoContrasenaAdmin"

    [StringLength(100)]
    [Display(Name = "Entidad Afectada")]
    public string? EntidadAfectada { get; set; } // Ej: "UsuariosSistema", "RolesSistema"

    [StringLength(450)] // ID de la entidad afectada (ej. ID del usuario modificado)
    [Display(Name = "ID Entidad Afectada")]
    public string? IdEntidadAfectada { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Detalles del Cambio")]
    public string? DetallesCambio { get; set; }
    // Podría ser un JSON o texto formateado describiendo qué cambió.
    // Ej: "Rol 'Editor' añadido.", "Campo 'Apellidos' cambiado de 'Pérez' a 'Pérez Solano'."

    [StringLength(45)] // Para la dirección IP
    [Display(Name = "Dirección IP")]
    public string? DireccionIp { get; set; }

    public RegistrosAuditoria()
    {
      FechaHoraEvento = DateTime.UtcNow;
    }
  }
}
