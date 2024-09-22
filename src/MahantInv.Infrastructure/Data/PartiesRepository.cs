using Dapper;
using MahantInv.Infrastructure.Entities;
using MahantInv.Infrastructure.Interfaces;
using MahantInv.Infrastructure.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.Data
{
    public class PartiesRepository : DapperRepository<Party>, IPartiesRepository
    {
        public PartiesRepository(IDapperUnitOfWork uow) : base(uow)
        {
        }

        public Task<PartyVM> GetPartyById(int partyId)
        {
            return db.QuerySingleAsync<PartyVM>(@"select p.*,u.UserName LastModifiedBy,pc.Name as Category from Parties p 
                    inner join AspNetUsers u on p.LastModifiedById = u.Id
                    left outer join PartyCategories pc on p.CategoryId = pc.Id
                    where p.Id= @partyId", new { partyId }, transaction: t);
        }

        public Task<IEnumerable<PartyVM>> GetParties()
        {
            return db.QueryAsync<PartyVM>(@"select p.*,u.UserName LastModifiedBy,pc.Name as Category from Parties p 
                    inner join AspNetUsers u on p.LastModifiedById = u.Id
                    left outer join PartyCategories pc on p.CategoryId = pc.Id", transaction: t);
        }
    }
}
