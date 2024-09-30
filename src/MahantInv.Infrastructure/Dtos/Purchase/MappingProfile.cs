using AutoMapper;
using MahantInv.Infrastructure.Entities;
using MahantInv.Infrastructure.ViewModels;
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
            CreateMap<Order, OrderCreateDto>()
                .ForMember(d => d.ProductExpiries, o => o.MapFrom(s => s.ProductExpiries.Select(p => p.ExpiryDate)))
                .ForMember(d => d.OrderTransactions, m => m.MapFrom(s => s.OrderTransactions));
            CreateMap<OrderTransactionCreateDto, OrderTransaction>();
            CreateMap<ProductInventory, ProductInventoryHistory>();

            CreateMap<Order, OrderVM>()
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name))
                .ForMember(d => d.Company, o => o.MapFrom(s => s.Product.Company))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.Title))
                .ForMember(d => d.OrderTransactions, o => o.MapFrom(s => s.OrderTransactions));
            CreateMap<OrderTransaction, OrderTransactionVM>();

            CreateMap<Order, OrderListDto>()
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.Title))
                .ForMember(d => d.OrderBulkName, o => o.MapFrom(s => s.Product.OrderBulkName == null ? null : $"{s.Product.OrderBulkQuantity} {s.Product.OrderBulkName}".Trim()))
                .ForMember(d => d.PaymentStatus, o => o.MapFrom(s => s.PaymentStatus))
                .ForMember(d => d.Product, o => o.MapFrom(s => s.Product.Name))
                .ForMember(d => d.Seller, o => o.MapFrom(s => s.Seller == null ? null : s.Seller.Name))
                .ForMember(d => d.LastModifiedBy, o => o.MapFrom(s => s.LastModifiedBy.UserName))
                .ForMember(d => d.OrderDate, o => o.MapFrom(s => s.OrderDate == null ? null : s.OrderDate.Value.ToString("dd/MM/yy")))
                .ForMember(d => d.ModifiedAt, o => o.MapFrom(s => s.ModifiedAt == null ? null : s.ModifiedAt.Value.ToString("dd/MM/yy")))
                .ForMember(d => d.ReceivedDate, o => o.MapFrom(s => s.ReceivedDate == null ? null : s.ReceivedDate.Value.ToString("dd/MM/yy")));
        }
    }
}
