// VN_Center/Models/ViewModels/RegistroAuditoriaViewModel.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace VN_Center.Models.ViewModels
{
  public class RegistroAuditoriaViewModel
  {
    public int AuditoriaID { get; set; }

    [Display(Name = "Fecha y Hora")]
    public DateTime FechaHoraEvento { get; set; }

    [Display(Name = "ID Usuario Ejecutor")]
    public string? UsuarioEjecutorId { get; set; }

    [Display(Name = "Usuario Ejecutor")] // Nombre o Email del que hizo la acción
    public string? NombreUsuarioEjecutor { get; set; }

    [Display(Name = "Tipo de Evento")]
    public string TipoEvento { get; set; } = string.Empty;

    [Display(Name = "Entidad Afectada")]
    public string? EntidadAfectada { get; set; }

    [Display(Name = "ID Entidad Afectada")]
    public string? IdEntidadAfectada { get; set; }

    [Display(Name = "Nombre/Detalle Entidad Afectada")] // Nombre o Email del usuario afectado, o detalle de otra entidad
    public string? NombreDetalleEntidadAfectada { get; set; }

    [Display(Name = "Detalles del Cambio")]
    public string? DetallesCambio { get; set; }

    [Display(Name = "Dirección IP")]
    public string? DireccionIp { get; set; }
  }
}
