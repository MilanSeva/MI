using AutoMapper;
using MahantInv.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.Dtos.Purchase
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<OrderCreateDto, Order>();
            CreateMap<OrderTransactionCreateDto, OrderTransaction>();
            CreateMap<ProductInventory, ProductInventoryHistory>();
        }
    }
}
