using MahantInv.Web.Infrastructure.Entities;
using MahantInv.Web.Infrastructure.Interfaces;

namespace MahantInv.Web.Infrastructure.Data
{
    public class BuyersRepository : EfRepository<Buyer>, IBuyersRepository
    {
        public BuyersRepository(MIDbContext context) : base(context)
        {
        }
    }
}
