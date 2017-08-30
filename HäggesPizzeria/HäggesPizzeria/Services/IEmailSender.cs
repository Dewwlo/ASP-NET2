using System.Threading.Tasks;

namespace HäggesPizzeria.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
