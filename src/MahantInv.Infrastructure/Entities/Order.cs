using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MahantInv.Infrastructure.Entities;

public partial class Order
{
    [Key]
    public int Id { get; set; }

    public int ProductId { get; set; }

    [Column(TypeName = "NUMERIC (10, 2)")]
    public int Quantity { get; set; }

    [Required]
    [Column(TypeName = "VARCHAR (50)")]
    public string RefNo { get; set; }

    [Required]
    [Column(TypeName = "VARCHAR (50)")]
    public string StatusId { get; set; }

    public int? SellerId { get; set; }

    [Column(TypeName = "DATE")]
    public DateTime OrderDate { get; set; }

    [Column(TypeName = "NUMERIC (10, 2)")]
    public int? PricePerItem { get; set; }

    [Column(TypeName = "NUMERIC (7, 2)")]
    public int? Discount { get; set; }

    [Column(TypeName = "NUMERIC (7, 2)")]
    public double? Tax { get; set; }

    [Column(TypeName = "NUMERIC (10, 2)")]
    public int? DiscountAmount { get; set; }

    [Column(TypeName = "NUMERIC (10, 2)")]
    public double? NetAmount { get; set; }

    [Column(TypeName = "TEXT (900)")]
    public string Remark { get; set; }

    [Required]
    [Column(TypeName = "VARCHAR (450)")]
    public string LastModifiedById { get; set; }

    [Column(TypeName = "DATETIME")]
    public DateTime ModifiedAt { get; set; }

    [ForeignKey("LastModifiedById")]
    [InverseProperty("Orders")]
    public virtual AspNetUser LastModifiedBy { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<OrderTransaction> OrderTransactions { get; set; } = new List<OrderTransaction>();

    [ForeignKey("ProductId")]
    [InverseProperty("Orders")]
    public virtual Product Product { get; set; }

    [ForeignKey("SellerId")]
    [InverseProperty("Orders")]
    public virtual Party Seller { get; set; }

    [ForeignKey("StatusId")]
    [InverseProperty("Orders")]
    public virtual OrderStatusType Status { get; set; }
}
