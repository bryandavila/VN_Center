using Microsoft.Extensions.Logging; // Para ILogger
using System.Threading.Tasks;

namespace VN_Center.Services
{
  public class ConsoleEmailSender : IEmailSender
  {
    private readonly ILogger<ConsoleEmailSender> _logger;

    public ConsoleEmailSender(ILogger<ConsoleEmailSender> logger)
    {
      _logger = logger;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
      _logger.LogInformation("--- NUEVO CORREO (SIMULADO) ---");
      _logger.LogInformation($"Para: {email}");
      _logger.LogInformation($"Asunto: {subject}");
      _logger.LogInformation("Cuerpo (HTML):");
      _logger.LogInformation(htmlMessage);
      _logger.LogInformation("--- FIN DEL CORREO (SIMULADO) ---");

      return Task.CompletedTask;
    }
  }
}
