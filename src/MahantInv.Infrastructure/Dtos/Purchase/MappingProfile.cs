﻿using AutoMapper;
using MahantInv.Infrastructure.Entities;
using System.Linq;

namespace MahantInv.Infrastructure.Dtos.Purchase
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<OrderCreateDto, Order>()
                .ForMember(d => d.OrderTransactions, o => o.MapFrom(s => s.OrderTransactions))
                .ForMember(d => d.Status, o => o.Ignore());
            CreateMap<Order, OrderCreateDto>()
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.Title))
                .ForMember(d => d.ProductExpiries, o => o.MapFrom(s => s.ProductExpiries.Select(p => p.ExpiryDate)))
                .ForMember(d => d.OrderTransactions, m => m.MapFrom(s => s.OrderTransactions));
            CreateMap<OrderTransactionCreateDto, OrderTransaction>()
                .ForMember(d => d.Party, o => o.Ignore())
                .ForMember(d => d.PaymentType, o => o.Ignore());
            CreateMap<OrderTransaction, OrderTransactionCreateDto>()
                .ForMember(d => d.Party, o => o.MapFrom(s => s.Party.Name))
                .ForMember(d => d.PaymentType, o => o.MapFrom(s => s.PaymentType.Title));
            CreateMap<ProductInventory, ProductInventoryHistory>();

            //CreateMap<Order, OrderVM>()
            //    .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name))
            //    .ForMember(d => d.Company, o => o.MapFrom(s => s.Product.Company))
            //    .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.Title))
            //    .ForMember(d => d.OrderTransactions, o => o.MapFrom(s => s.OrderTransactions));
            //CreateMap<OrderTransaction, OrderTransactionVM>();

            CreateMap<Order, OrderListDto>()
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.Title))
                .ForMember(d => d.OrderBulkName, o => o.MapFrom(s => s.Product.OrderBulkName == null ? null : $"{s.Product.OrderBulkQuantity} {s.Product.OrderBulkName}".Trim()))
                .ForMember(d => d.PaymentStatus, o => o.MapFrom(s => s.PaymentStatus))
                .ForMember(d => d.Product, o => o.MapFrom(s => s.Product.Name))
                .ForMember(d => d.Seller, o => o.MapFrom(s => s.Seller == null ? null : s.Seller.Name))
                .ForMember(d => d.LastModifiedBy, o => o.MapFrom(s => s.LastModifiedBy.UserName))
                .ForMember(d => d.OrderDate, o => o.MapFrom(s => s.OrderDate == null ? null : s.OrderDate.Value.ToString("dd/MM/yy")))
                .ForMember(d => d.ModifiedAt, o => o.MapFrom(s => s.ModifiedAt == null ? null : s.ModifiedAt.Value.ToString("dd/MM/yy")))
                .ForMember(d => d.ReceivedDate, o => o.MapFrom(s => s.ReceivedDate == null ? null : s.ReceivedDate.Value.ToString("dd/MM/yy")))
                .ForMember(d => d.OrderTransactions, o => o.MapFrom(s => s.OrderTransactions));
            CreateMap<OrderTransaction, OrderListTransactionDto>()
                .ForMember(d => d.Party, o => o.MapFrom(s => s.Party.Name))
                .ForMember(d => d.PaymentType, o => o.MapFrom(s => s.PaymentType.Title))
                .ForMember(d => d.PaymentDate, o => o.MapFrom(s => s.PaymentDate == null ? null : s.PaymentDate.Value.ToString("dd/MM/yy")));

        }
    }
}
