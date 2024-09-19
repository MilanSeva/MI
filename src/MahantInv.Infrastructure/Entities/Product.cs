using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MahantInv.Infrastructure.Entities;

[Index("Name", "Size", "UnitTypeCode", Name = "CUK_Name_UnitTypeCode", IsUnique = true)]
public partial class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "VARCHAR (255)")]
    public string Name { get; set; }

    [Column(TypeName = "NUMERIC (10, 2)")]
    public int? Size { get; set; }

    [Column(TypeName = "VARCHAR (900)")]
    public string Description { get; set; }

    [Column(TypeName = "VARCHAR (12)")]
    public string UnitTypeCode { get; set; }

    [Column(TypeName = "NUMERIC (10, 2)")]
    public int ReorderLevel { get; set; }

    [Column(TypeName = "BOOL")]
    public bool IsDisposable { get; set; }

    [Column(TypeName = "VARCHAR (256)")]
    public string Company { get; set; }

    [Column(TypeName = "BOOL")]
    public bool Enabled { get; set; }

    [Required]
    [Column(TypeName = "VARCHAR (450)")]
    public string LastModifiedById { get; set; }

    [Column(TypeName = "DATETIME")]
    public DateTime ModifiedAt { get; set; }

    [ForeignKey("LastModifiedById")]
    [InverseProperty("Products")]
    public virtual AspNetUser LastModifiedBy { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty("Product")]
    public virtual ICollection<ProductInventory> ProductInventories { get; set; } = new List<ProductInventory>();

    [InverseProperty("Product")]
    public virtual ICollection<ProductInventoryHistory> ProductInventoryHistories { get; set; } = new List<ProductInventoryHistory>();

    [InverseProperty("Product")]
    public virtual ICollection<ProductUsage> ProductUsages { get; set; } = new List<ProductUsage>();

    [ForeignKey("UnitTypeCode")]
    [InverseProperty("Products")]
    public virtual UnitType UnitTypeCodeNavigation { get; set; }
}
