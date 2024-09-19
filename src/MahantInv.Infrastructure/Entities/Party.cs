using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MahantInv.Infrastructure.Entities;

public partial class Party
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "VARCHAR (256)")]
    public string Name { get; set; }

    public int CategoryId { get; set; }

    [Column(TypeName = "VARCHAR (15)")]
    public string PrimaryContact { get; set; }

    [Column(TypeName = "VARCHAR (15)")]
    public string SecondaryContact { get; set; }

    [Column(TypeName = "TEXT (255)")]
    public string Line1 { get; set; }

    [Column(TypeName = "TEXT (255)")]
    public string Line2 { get; set; }

    [Column(TypeName = "TEXT (255)")]
    public string Taluk { get; set; }

    [Column(TypeName = "TEXT (128)")]
    public string District { get; set; }

    [Column(TypeName = "TEXT (128)")]
    public string State { get; set; }

    [Column(TypeName = "TEXT (128)")]
    public string Country { get; set; }

    [Required]
    [Column(TypeName = "VARCHAR (7)")]
    public string Type { get; set; }

    [Required]
    [Column(TypeName = "VARCHAR (450)")]
    public string LastModifiedById { get; set; }

    [Column(TypeName = "DATETIME")]
    public DateTime ModifiedAt { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Parties")]
    public virtual PartyCategory Category { get; set; }

    [ForeignKey("LastModifiedById")]
    [InverseProperty("Parties")]
    public virtual AspNetUser LastModifiedBy { get; set; }

    [InverseProperty("Party")]
    public virtual ICollection<OrderTransaction> OrderTransactions { get; set; } = new List<OrderTransaction>();

    [InverseProperty("Seller")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
