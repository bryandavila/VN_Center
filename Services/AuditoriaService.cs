// VN_Center/Services/AuditoriaService.cs
using System;
using System.Threading.Tasks;
using VN_Center.Data; // Para VNCenterDbContext
using VN_Center.Models.Entities; // Para RegistrosAuditoria y UsuariosSistema

namespace VN_Center.Services
{
  public class AuditoriaService : IAuditoriaService
  {
    private readonly VNCenterDbContext _context;

    public AuditoriaService(VNCenterDbContext context)
    {
      _context = context;
    }

    public async Task RegistrarEventoAuditoriaAsync(
        UsuariosSistema? usuarioEjecutor,
        string tipoEvento,
        string? entidadAfectada,
        string? idEntidadAfectada,
        string? detallesCambio,
        string? direccionIp = null)
    {
      await RegistrarEventoAuditoriaAsync(
          usuarioEjecutor?.Id.ToString(), // El ID de UsuariosSistema es int, lo convertimos a string
          usuarioEjecutor?.UserName,      // O podrías usar usuarioEjecutor.NombreCompleto si prefieres
          tipoEvento,
          entidadAfectada,
          idEntidadAfectada,
          detallesCambio,
          direccionIp);
    }

    public async Task RegistrarEventoAuditoriaAsync(
        string? usuarioEjecutorId,
        string? nombreUsuarioEjecutor,
        string tipoEvento,
        string? entidadAfectada,
        string? idEntidadAfectada,
        string? detallesCambio,
        string? direccionIp = null)
    {
      if (string.IsNullOrWhiteSpace(tipoEvento))
      {
        throw new ArgumentNullException(nameof(tipoEvento), "El tipo de evento no puede ser nulo o vacío.");
      }

      var registro = new RegistrosAuditoria
      {
        FechaHoraEvento = DateTime.UtcNow, // Se asegura de usar la hora UTC
        UsuarioEjecutorId = usuarioEjecutorId,
        NombreUsuarioEjecutor = nombreUsuarioEjecutor,
        TipoEvento = tipoEvento,
        EntidadAfectada = entidadAfectada,
        IdEntidadAfectada = idEntidadAfectada,
        DetallesCambio = detallesCambio,
        DireccionIp = direccionIp
      };

      _context.RegistrosAuditoria.Add(registro);
      await _context.SaveChangesAsync();
    }
  }
}
