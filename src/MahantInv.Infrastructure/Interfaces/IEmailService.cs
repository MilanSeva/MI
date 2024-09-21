using MahantInv.Infrastructure.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(Email email);
    }
}
