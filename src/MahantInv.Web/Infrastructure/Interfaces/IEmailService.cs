using MahantInv.Web.Infrastructure.ViewModels;
using System.Threading.Tasks;

namespace MahantInv.Web.Infrastructure.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(Email email);
    }
}
