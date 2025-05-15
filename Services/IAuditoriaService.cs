// VN_Center/Services/IAuditoriaService.cs
using System.Threading.Tasks;
using VN_Center.Models.Entities; // Para la entidad UsuariosSistema

namespace VN_Center.Services
{
  public interface IAuditoriaService
  {
    /// <summary>
    /// Registra un evento de auditoría en la base de datos.
    /// </summary>
    /// <param name="usuarioEjecutor">El usuario del sistema que realiza la acción (puede ser null si es una acción del sistema).</param>
    /// <param name="tipoEvento">Una cadena que describe el tipo de evento (ej. "CreacionUsuario", "ActualizacionRol").</param>
    /// <param name="entidadAfectada">El nombre de la entidad principal afectada (ej. "UsuariosSistema").</param>
    /// <param name="idEntidadAfectada">El ID (como string) de la instancia específica de la entidad afectada.</param>
    /// <param name="detallesCambio">Una descripción de los cambios realizados. Puede ser texto simple o JSON.</param>
    /// <param name="direccionIp">La dirección IP desde donde se originó la solicitud (opcional).</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    Task RegistrarEventoAuditoriaAsync(
        UsuariosSistema? usuarioEjecutor,
        string tipoEvento,
        string? entidadAfectada,
        string? idEntidadAfectada,
        string? detallesCambio,
        string? direccionIp = null);

    /// <summary>
    /// Sobrecarga para registrar un evento de auditoría cuando el ID del usuario ejecutor se conoce directamente.
    /// </summary>
    Task RegistrarEventoAuditoriaAsync(
        string? usuarioEjecutorId,
        string? nombreUsuarioEjecutor,
        string tipoEvento,
        string? entidadAfectada,
        string? idEntidadAfectada,
        string? detallesCambio,
        string? direccionIp = null);
  }
}
