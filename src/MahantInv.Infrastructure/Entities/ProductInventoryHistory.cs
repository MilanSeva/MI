using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MahantInv.Infrastructure.Entities;

[Table("ProductInventoryHistory")]
public partial class ProductInventoryHistory
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

    [ForeignKey("LastModifiedById")]
    [InverseProperty("ProductInventoryHistories")]
    public virtual AspNetUser LastModifiedBy { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("ProductInventoryHistories")]
    public virtual Product Product { get; set; }
}
