using System.Threading.Tasks;

namespace MahantInv.Web.Infrastructure.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string from, string subject, string body);
    }
}
