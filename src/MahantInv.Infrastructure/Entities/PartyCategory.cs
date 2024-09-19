using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MahantInv.Infrastructure.Entities;

[Index("Name", IsUnique = true)]
public partial class PartyCategory
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "VARCHAR (50)")]
    public string Name { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<Party> Parties { get; set; } = new List<Party>();
}
