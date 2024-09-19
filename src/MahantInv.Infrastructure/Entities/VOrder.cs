using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MahantInv.Infrastructure.Entities;

[Keyless]
public partial class VOrder
{
    public int? Id { get; set; }

    public int? ProductId { get; set; }

    public double? Quantity { get; set; }

    [Column(TypeName = "VARCHAR (50)")]
    public string RefNo { get; set; }

    [Column(TypeName = "VARCHAR (50)")]
    public string StatusId { get; set; }

    public int? SellerId { get; set; }

    [Column(TypeName = "DATE")]
    public DateTime? OrderDate { get; set; }

    public double? PricePerItem { get; set; }

    public double? Discount { get; set; }

    public double? Tax { get; set; }

    public double? DiscountAmount { get; set; }

    public double? NetAmount { get; set; }

    [Column(TypeName = "TEXT (900)")]
    public string Remark { get; set; }

    [Column(TypeName = "VARCHAR (450)")]
    public string LastModifiedById { get; set; }

    [Column(TypeName = "DATETIME")]
    public DateTime? ModifiedAt { get; set; }

    [Column(TypeName = "VARCHAR (255)")]
    public string ProductName { get; set; }

    [Column(TypeName = "VARCHAR (256)")]
    public string Company { get; set; }

    [Column(TypeName = "VARCHAR (128)")]
    public string Status { get; set; }

    [Column(TypeName = "VARCHAR (256)")]
    public string LastModifiedBy { get; set; }

    [Column(TypeName = "NUMERIC (10, 2)")]
    public int? CurrentStock { get; set; }

    [Column(TypeName = "NUMERIC (10, 2)")]
    public int? ReorderLevel { get; set; }

    [Column(TypeName = "VARCHAR (256)")]
    public string Seller { get; set; }

    public double? PaidAmount { get; set; }
}
