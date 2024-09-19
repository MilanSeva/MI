using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MahantInv.Infrastructure.Entities;

public partial class OrderTransaction
{
    [Key]
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int PartyId { get; set; }

    [Required]
    [Column(TypeName = "VARCHAR (20)")]
    public string PaymentTypeId { get; set; }

    [Column(TypeName = "NUMERIC (10, 2)")]
    public double Amount { get; set; }

    [Column(TypeName = "DATE")]
    public DateTime? PaymentDate { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("OrderTransactions")]
    public virtual Order Order { get; set; }

    [ForeignKey("PartyId")]
    [InverseProperty("OrderTransactions")]
    public virtual Party Party { get; set; }

    [ForeignKey("PaymentTypeId")]
    [InverseProperty("OrderTransactions")]
    public virtual PaymentType PaymentType { get; set; }
}
