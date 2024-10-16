using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MahantInv.Infrastructure.Dtos.Purchase
{
    public class OrderCreateDto
    {
        public int Id { get; set; }
        [Required, Display(Name = "Product *")]
        public int? ProductId { get; set; }
        [Required, Display(Name = "Quantity *")]
        public double? Quantity { get; set; }
        [Display(Name = "Received Quantity")]
        public double? ReceivedQuantity { get; set; }
        public string RefNo { get; set; }
        [Display(Name="Bulk Quantity")]
        public string BulkNameQuantity { get; set; }
        [Display(Name = "Seller")]
        public int? SellerId { get; set; }
        [Required, Display(Name = "Order Date *")]
        public DateOnly? OrderDate { get; set; }
        [Display(Name = "Received Date")]
        public DateOnly? ReceivedDate { get; set; }
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
        public string Status { get; set; }

        public List<OrderTransactionCreateDto> OrderTransactions { get; set; } = new List<OrderTransactionCreateDto>();
        public List<DateOnly> ProductExpiries { get; set; } = new List<DateOnly>();
        //public virtual ICollection<OrderDocument> OrderDocuments { get; set; } = new List<OrderDocument>();
    }
    public class OrderTransactionCreateDto
    {
        public int PartyId { get; set; }
        public string Party { get; set; }
        public string? PaymentTypeId { get; set; }
        public string? PaymentType { get; set; }
        public decimal Amount { get; set; }
        public DateOnly? PaymentDate { get; set; }
    }
}
