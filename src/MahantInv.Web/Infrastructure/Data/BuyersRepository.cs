using MahantInv.Web.Infrastructure.Entities;
using MahantInv.Web.Infrastructure.Interfaces;

namespace MahantInv.Web.Infrastructure.Data
{
    public class BuyersRepository : DapperRepository<Buyer>, IBuyersRepository
    {
        public BuyersRepository(IDapperUnitOfWork uow) : base(uow)
        {
        }
    }
}
