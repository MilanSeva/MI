﻿using MahantInv.Infrastructure.Identity;
using MahantInv.SharedKernel;
using MahantInv.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahantInv.Infrastructure.Entities
{
    [Table("Orders")]
    public class Order : BaseEntity, IAggregateRoot
    {
        [Required, Display(Name = "Product")]
        public int? ProductId { get; set; }
        [Required, Display(Name = "Quantity")]
        public double? Quantity { get; set; }
        [Display(Name = "Received Quantity")]
        public double? ReceivedQuantity { get; set; }
        public string RefNo { get; set; }
        [Display(Name = "Status")]
        public string StatusId { get; set; }
        public string PaymentStatus { get; set; }
        [Display(Name = "Seller")]
        public int? SellerId { get; set; }
        [Required, Display(Name = "Order Date")]
        public DateOnly? OrderDate { get; set; }
        [Display(Name = "Received Date")]
        public DateOnly? ReceivedDate { get; set; }
        [Display(Name = "Price Per Item")]
        public double? PricePerItem { get; set; }
        [Display(Name = "Discount(%)")]
        public double? Discount { get; set; }
        [Display(Name = "Tax(%)")]
        public double? Tax { get; set; }
        [Display(Name = "Discount Amount")]
        public double? DiscountAmount { get; set; }
        [Display(Name = "Net Amount")]
        public double? NetAmount { get; set; }
        public string Remark { get; set; }
        [Display(Name = "Last Modified By")]
        public string LastModifiedById { get; set; }
        [Display(Name = "Modified At")]
        public DateTime? ModifiedAt { get; set; }

        [Dapper.Contrib.Extensions.Write(false)]
        [ForeignKey("LastModifiedById")]
        [InverseProperty("Orders")]
        public virtual MIIdentityUser LastModifiedBy { get; set; }

        [Dapper.Contrib.Extensions.Write(false)]
        [InverseProperty("Order")]
        public virtual List<OrderTransaction> OrderTransactions { get; set; } = new List<OrderTransaction>();

        [Dapper.Contrib.Extensions.Write(false)]
        [ForeignKey("ProductId")]
        [InverseProperty("Orders")]
        public virtual Product Product { get; set; }

        [Dapper.Contrib.Extensions.Write(false)]
        [ForeignKey("SellerId")]
        [InverseProperty("Orders")]
        public virtual Party Seller { get; set; }

        [Dapper.Contrib.Extensions.Write(false)]
        [ForeignKey("StatusId")]
        [InverseProperty("Orders")]
        public virtual OrderStatusType Status { get; set; }
        [InverseProperty("Order")]
        public virtual List<ProductExpiry> ProductExpiries { get; set; } = new List<ProductExpiry>();
        [InverseProperty("Order")]
        public virtual List<OrderDocument> OrderDocuments { get; set; } = new List<OrderDocument>();
    }
}
