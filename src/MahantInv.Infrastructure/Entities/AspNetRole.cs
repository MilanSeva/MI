using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MahantInv.Infrastructure.Entities;

[Index("NormalizedName", Name = "RoleNameIndex", IsUnique = true)]
public partial class AspNetRole
{
    [Key]
    [Column(TypeName = "VARCHAR (450)")]
    public string Id { get; set; }

    [Column(TypeName = "VARCHAR (256)")]
    public string Name { get; set; }

    [Column(TypeName = "VARCHAR (256)")]
    public string NormalizedName { get; set; }

    public string ConcurrencyStamp { get; set; }

    [InverseProperty("Role")]
    public virtual ICollection<AspNetRoleClaim> AspNetRoleClaims { get; set; } = new List<AspNetRoleClaim>();

    [ForeignKey("RoleId")]
    [InverseProperty("Roles")]
    public virtual ICollection<AspNetUser> Users { get; set; } = new List<AspNetUser>();
}
