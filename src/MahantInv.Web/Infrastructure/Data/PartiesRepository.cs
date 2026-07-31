using MahantInv.Web.Infrastructure.Entities;
using MahantInv.Web.Infrastructure.Interfaces;
using MahantInv.Web.Infrastructure.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MahantInv.Web.Infrastructure.Data
{
    public class PartiesRepository : EfRepository<Party>, IPartiesRepository
    {
        public PartiesRepository(MIDbContext context) : base(context)
        {
        }

        public Task<PartyVM> GetPartyById(int partyId)
        {
            return _context.Parties
                .Where(p => p.Id == partyId)
                .Select(p => new PartyVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Type = p.Type,
                    CategoryId = p.CategoryId,
                    PrimaryContact = p.PrimaryContact,
                    City = p.City,
                    Country = p.Country,
                    LastModifiedById = p.LastModifiedById,
                    ModifiedAt = p.ModifiedAt,
                    LastModifiedBy = p.LastModifiedBy.UserName,
                    Category = p.Category.Name
                })
                .SingleAsync();
        }

        public async Task<IEnumerable<PartyVM>> GetParties()
        {
            return await _context.Parties
                .Select(p => new PartyVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Type = p.Type,
                    CategoryId = p.CategoryId,
                    PrimaryContact = p.PrimaryContact,
                    City = p.City,
                    Country = p.Country,
                    LastModifiedById = p.LastModifiedById,
                    ModifiedAt = p.ModifiedAt,
                    LastModifiedBy = p.LastModifiedBy.UserName,
                    Category = p.Category.Name
                })
                .ToListAsync();
        }
    }
}
