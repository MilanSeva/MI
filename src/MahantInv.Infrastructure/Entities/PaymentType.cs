using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MahantInv.Infrastructure.Entities;

public partial class PaymentType
{
    [Key]
    [Column(TypeName = "VARCHAR (20)")]
    public string Id { get; set; }

    [Column(TypeName = "VARCHAR (256)")]
    public string Title { get; set; }

    [InverseProperty("PaymentType")]
    public virtual ICollection<OrderTransaction> OrderTransactions { get; set; } = new List<OrderTransaction>();
}
