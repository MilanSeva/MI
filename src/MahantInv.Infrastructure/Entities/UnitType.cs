using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MahantInv.Infrastructure.Entities;

public partial class UnitType
{
    [Key]
    [Column(TypeName = "VARCHAR (12)")]
    public string Code { get; set; }

    [Required]
    [Column(TypeName = "VARCHAR (128)")]
    public string Name { get; set; }

    [InverseProperty("UnitTypeCodeNavigation")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
