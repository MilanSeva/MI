using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MahantInv.Infrastructure.Entities;

public partial class ProductUsage
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
    [Column(TypeName = "VARCHAR (450)")]
    public string LastModifiedById { get; set; }

    [Column(TypeName = "DATETIME")]
    public DateTime ModifiedAt { get; set; }

    [Column(TypeName = "VARCHAR (255)")]
    public string Buyer { get; set; }

    [Column(TypeName = "DATE")]
    public DateTime? UsageDate { get; set; }

    [ForeignKey("LastModifiedById")]
    [InverseProperty("ProductUsages")]
    public virtual AspNetUser LastModifiedBy { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("ProductUsages")]
    public virtual Product Product { get; set; }
}
