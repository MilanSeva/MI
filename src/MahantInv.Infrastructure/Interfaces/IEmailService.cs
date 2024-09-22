using MahantInv.Infrastructure.ViewModels;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(Email email);
    }
}
