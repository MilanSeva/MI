using MahantInv.Infrastructure.Entities;
using MahantInv.Infrastructure.ViewModels;
using MahantInv.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.Interfaces
{
    public interface IPartiesRepository : IAsyncRepository<Party>
    {
        Task<IEnumerable<PartyVM>> GetParties();
        Task<PartyVM> GetPartyById(int partyId);
    }
}
