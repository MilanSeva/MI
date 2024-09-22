using MahantInv.Infrastructure.Entities;
using MahantInv.Infrastructure.Interfaces;

namespace MahantInv.Infrastructure.Data
{
    public class BuyersRepository : DapperRepository<Buyer>, IBuyersRepository
    {
        public BuyersRepository(IDapperUnitOfWork uow) : base(uow)
        {
        }
    }
}
