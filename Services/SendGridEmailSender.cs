using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace VN_Center.Services
{
  public class SendGridEmailSender : IEmailSender
  {
    private readonly ILogger<SendGridEmailSender> _logger;
    private readonly string _sendGridKey;
    private readonly string _senderEmail;
    private readonly string _senderName;
    private readonly string _resetPasswordTemplateId;
    private readonly string _appName; // Para el nombre de la aplicación

    public SendGridEmailSender(IConfiguration configuration, ILogger<SendGridEmailSender> logger)
    {
      _logger = logger;
      _sendGridKey = configuration["EmailSender:SendGridKey"];
      _senderEmail = configuration["EmailSender:SenderEmail"];
      _senderName = configuration["EmailSender:SenderName"];
      _resetPasswordTemplateId = configuration["EmailSender:ResetPasswordTemplateId"];
      _appName = configuration["AppSettings:AppName"] ?? "VN Center"; // Obtener de AppSettings o usar valor por defecto

      if (string.IsNullOrEmpty(_sendGridKey)) _logger.LogError("SendGridKey no está configurada en EmailSender:SendGridKey.");
      if (string.IsNullOrEmpty(_senderEmail)) _logger.LogError("SenderEmail no está configurado en EmailSender:SenderEmail.");
      if (string.IsNullOrEmpty(_resetPasswordTemplateId)) _logger.LogError("ResetPasswordTemplateId no está configurado para SendGrid en EmailSender:ResetPasswordTemplateId.");
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessageAsResetLink)
    {
      if (string.IsNullOrEmpty(_sendGridKey) || string.IsNullOrEmpty(_senderEmail) || string.IsNullOrEmpty(_resetPasswordTemplateId))
      {
        _logger.LogError("El servicio de correo SendGrid (plantilla dinámica) no está completamente configurado. No se enviará el correo.");
        return;
      }

      var client = new SendGridClient(_sendGridKey);
      var from = new EmailAddress(_senderEmail, _senderName ?? _appName); // Usar appName si senderName es nulo
      var to = new EmailAddress(email);

      // El nombre del usuario podría obtenerse si tuvieras el objeto User aquí, o pasarlo desde AuthController
      // Por ahora, usaremos el email como un placeholder para user_name si no tenemos el nombre real.
      string userNameForEmail = email; // Placeholder, idealmente sería el nombre real del usuario.

      var dynamicTemplateData = new Dictionary<string, object> // Cambiado a object para permitir diferentes tipos
            {
                { "subject", subject },
                { "reset_link", htmlMessageAsResetLink },
                { "user_name", userNameForEmail }, // Placeholder para el nombre del usuario
                { "appName", _appName },
                { "current_year", DateTime.Now.Year.ToString() }
            };

      var msg = MailHelper.CreateSingleTemplateEmail(from, to, _resetPasswordTemplateId, dynamicTemplateData);

      try
      {
        var response = await client.SendEmailAsync(msg);
        if (response.IsSuccessStatusCode)
        {
          _logger.LogInformation($"Correo (plantilla dinámica {_resetPasswordTemplateId}) enviado a {email}. Código de estado: {response.StatusCode}");
        }
        else
        {
          string responseBody = await response.Body.ReadAsStringAsync();
          _logger.LogError($"Error al enviar correo (plantilla {_resetPasswordTemplateId}) a {email} usando SendGrid. Código: {response.StatusCode}. Cuerpo: {responseBody}");
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Excepción al enviar correo (plantilla {_resetPasswordTemplateId}) a {email} usando SendGrid.");
      }
    }
  }
}
