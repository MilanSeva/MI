using MahantInv.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MahantInv.Infrastructure.Dtos.Purchase
{
    public class OrderCreateDto
    {
        public int Id { get; set; }
        [Required, Display(Name = "Product")]
        public int? ProductId { get; set; }
        [Required, Display(Name = "Quantity")]
        public double? Quantity { get; set; }
        [Display(Name = "Received Quantity")]
        public double? ReceivedQuantity { get; set; }
        public string RefNo { get; set; }
        [Display(Name = "Seller")]
        public int? SellerId { get; set; }
        [Required, Display(Name = "Order Date")]
        public DateTime? OrderDate { get; set; }
        [Display(Name = "Received Date")]
        public DateTime? ReceivedDate { get; set; }
        [Display(Name = "Price Per Item")]
        public double? PricePerItem { get; set; }
        [Display(Name = "Discount")]
        public double? Discount { get; set; }
        [Display(Name = "GST(%)")]
        public double? Tax { get; set; }
        [Display(Name = "Discount Amount")]
        public double? DiscountAmount { get; set; }
        [Display(Name = "Net Amount")]
        public double? NetAmount { get; set; }
        public string Remark { get; set; }
        
        public List<OrderTransaction> OrderTransactions { get; set; } = new List<OrderTransaction>();
        public List<DateTime> ProductExpiries { get; set; } = new List<DateTime>();
        //public virtual ICollection<OrderDocument> OrderDocuments { get; set; } = new List<OrderDocument>();
    }
    public class OrderTransactionCreateDto
    {
        public int PartyId { get; set; }
        public string PaymentTypeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}
