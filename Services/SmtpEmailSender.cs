using Microsoft.Extensions.Configuration; // Para IConfiguration
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace VN_Center.Services // Asegúrate que el namespace coincida
{
  public class SmtpEmailSender : IEmailSender
  {
    private readonly ILogger<SmtpEmailSender> _logger;
    private readonly string _host;
    private readonly int _port;
    private readonly bool _enableSSL;
    private readonly string _userName;
    private readonly string _password;
    private readonly string _senderEmail;
    private readonly string _senderName;

    public SmtpEmailSender(IConfiguration configuration, ILogger<SmtpEmailSender> logger)
    {
      _logger = logger;
      // Leer configuración desde appsettings.json o User Secrets
      _host = configuration["EmailSender:Smtp:Host"];
      _port = configuration.GetValue<int>("EmailSender:Smtp:Port");
      _enableSSL = configuration.GetValue<bool>("EmailSender:Smtp:EnableSSL");
      _userName = configuration["EmailSender:Smtp:UserName"]; // Tu email de Gmail/Outlook
      _password = configuration["EmailSender:Smtp:Password"]; // Tu contraseña de aplicación (Gmail) o contraseña normal
      _senderEmail = configuration["EmailSender:Smtp:SenderEmail"]; // El email que aparecerá como remitente
      _senderName = configuration["EmailSender:Smtp:SenderName"];   // El nombre que aparecerá como remitente

      if (string.IsNullOrEmpty(_host) || _port == 0 || string.IsNullOrEmpty(_userName) || string.IsNullOrEmpty(_password) || string.IsNullOrEmpty(_senderEmail))
      {
        _logger.LogError("La configuración SMTP para EmailSender no está completa. El envío de correos podría fallar.");
      }
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
      if (string.IsNullOrEmpty(_host) || _port == 0 || string.IsNullOrEmpty(_userName) || string.IsNullOrEmpty(_password) || string.IsNullOrEmpty(_senderEmail))
      {
        _logger.LogError("Configuración SMTP incompleta. No se puede enviar el correo.");
        // Podrías lanzar una excepción o simplemente no hacer nada.
        // Por ahora, si no está configurado, no intentará enviar.
        return;
      }

      try
      {
        using (var client = new SmtpClient(_host, _port))
        {
          client.EnableSsl = _enableSSL;
          client.UseDefaultCredentials = false;
          client.Credentials = new NetworkCredential(_userName, _password);

          var mailMessage = new MailMessage
          {
            From = new MailAddress(_senderEmail, _senderName ?? _senderEmail),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true,
          };
          mailMessage.To.Add(email);

          await client.SendMailAsync(mailMessage);
          _logger.LogInformation($"Correo enviado a {email} con asunto '{subject}' usando SMTP ({_host}).");
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Excepción al enviar correo a {email} usando SMTP ({_host}).");
        // Considera re-lanzar la excepción o manejarla de otra forma si el envío de correo es crítico.
      }
    }
  }
}
