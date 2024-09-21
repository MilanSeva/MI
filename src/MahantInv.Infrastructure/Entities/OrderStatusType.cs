using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MahantInv.Infrastructure.Entities;

public partial class OrderStatusType
{
    [Key]
    [Column(TypeName = "VARCHAR (50)")]
    public string Id { get; set; }

    [Required]
    [Column(TypeName = "VARCHAR (128)")]
    public string Title { get; set; }

    [InverseProperty("Status")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
