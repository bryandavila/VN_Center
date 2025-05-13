using System.Threading.Tasks;

namespace VN_Center.Services
{
  public interface IEmailSender
  {
    Task SendEmailAsync(string email, string subject, string htmlMessage);
  }
}
