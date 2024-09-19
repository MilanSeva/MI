using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MahantInv.Infrastructure.Entities;

[Keyless]
public partial class VOrderTransaction
{
    public int? Id { get; set; }

    public int? OrderId { get; set; }

    public int? PartyId { get; set; }

    [Column(TypeName = "VARCHAR (20)")]
    public string PaymentTypeId { get; set; }

    public double? Amount { get; set; }

    [Column(TypeName = "VARCHAR (256)")]
    public string Party { get; set; }

    [Column(TypeName = "VARCHAR (256)")]
    public string PaymentType { get; set; }

    [Column(TypeName = "DATE")]
    public DateTime? PaymentDate { get; set; }
}
