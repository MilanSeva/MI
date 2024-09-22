using AutoMapper;
using MahantInv.Infrastructure.ViewModels;

namespace MahantInv.Web.Utility
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //CreateMap<destination, source>();
            CreateMap<OrderVM, OrdersGrid>();
            CreateMap<OrderTransactionVM, OrdersGrid>();
        }
    }
}
