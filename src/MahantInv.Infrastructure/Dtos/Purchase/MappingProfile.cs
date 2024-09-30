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

            CreateMap<Order, OrderListDto>()
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.Title))
                .ForMember(d => d.OrderBulkName, o => o.MapFrom(s => s.Product.OrderBulkName == null ? null : $"{s.Product.OrderBulkQuantity} {s.Product.OrderBulkName}".Trim()))
                .ForMember(d => d.PaymentStatus, o => o.MapFrom(s => s.PaymentStatus))
                .ForMember(d => d.Product, o => o.MapFrom(s => s.Product.Name))
                .ForMember(d => d.Seller, o => o.MapFrom(s => s.Seller.Name))
                .ForMember(d => d.LastModifiedBy, o => o.MapFrom(s => s.LastModifiedBy.UserName))
                .ForMember(d => d.OrderDate, o => o.MapFrom(s => s.OrderDate == null ? null : s.OrderDate.Value.ToString("dd/MM/yy")))
                .ForMember(d => d.ModifiedAt, o => o.MapFrom(s => s.ModifiedAt == null ? null : s.ModifiedAt.Value.ToString("dd/MM/yy")))
                .ForMember(d => d.ReceivedDate, o => o.MapFrom(s => s.ReceivedDate == null ? null : s.ReceivedDate.Value.ToString("dd/MM/yy")));
        }
    }
}
