using MahantInv.Infrastructure.Entities;
using MahantInv.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.Data
{
    public class BuyersRepository : DapperRepository<Buyer>, IBuyersRepository
    {
        public BuyersRepository(IDapperUnitOfWork uow) : base(uow)
        {
        }
    }
}
