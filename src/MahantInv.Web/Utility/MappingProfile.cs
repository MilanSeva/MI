using AutoMapper;
using MahantInv.Infrastructure.ViewModels;

namespace MahantInv.Web.Utility
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //CreateMap<destination, source>();
            CreateMap<OrderVM, OrdersGrid>();
            CreateMap<OrderTransactionVM, OrdersGrid>();
        }
    }
}
