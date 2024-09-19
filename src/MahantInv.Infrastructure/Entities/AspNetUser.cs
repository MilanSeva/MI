using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MahantInv.Infrastructure.Entities;

[Index("NormalizedEmail", Name = "EmailIndex")]
[Index("NormalizedUserName", Name = "UserNameIndex", IsUnique = true)]
public partial class AspNetUser
{
    [Key]
    [Column(TypeName = "VARCHAR (450)")]
    public string Id { get; set; }

    [Column(TypeName = "VARCHAR (256)")]
    public string UserName { get; set; }

    [Column(TypeName = "VARCHAR (256)")]
    public string NormalizedUserName { get; set; }

    [Column(TypeName = "VARCHAR (256)")]
    public string Email { get; set; }

    [Column(TypeName = "VARCHAR (256)")]
    public string NormalizedEmail { get; set; }

    [Column(TypeName = "BOOL")]
    public bool EmailConfirmed { get; set; }

    public string PasswordHash { get; set; }

    public string SecurityStamp { get; set; }

    public string ConcurrencyStamp { get; set; }

    public string PhoneNumber { get; set; }

    [Column(TypeName = "BOOL")]
    public bool PhoneNumberConfirmed { get; set; }

    [Column(TypeName = "BOOL")]
    public bool TwoFactorEnabled { get; set; }

    [Column(TypeName = "TIMESTAMP")]
    public DateTime? LockoutEnd { get; set; }

    [Column(TypeName = "BOOL")]
    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; } = new List<AspNetUserClaim>();

    [InverseProperty("User")]
    public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; } = new List<AspNetUserLogin>();

    [InverseProperty("User")]
    public virtual ICollection<AspNetUserToken> AspNetUserTokens { get; set; } = new List<AspNetUserToken>();

    [InverseProperty("LastModifiedBy")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty("LastModifiedBy")]
    public virtual ICollection<Party> Parties { get; set; } = new List<Party>();

    [InverseProperty("LastModifiedBy")]
    public virtual ICollection<ProductInventory> ProductInventories { get; set; } = new List<ProductInventory>();

    [InverseProperty("LastModifiedBy")]
    public virtual ICollection<ProductInventoryHistory> ProductInventoryHistories { get; set; } = new List<ProductInventoryHistory>();

    [InverseProperty("LastModifiedBy")]
    public virtual ICollection<ProductUsage> ProductUsages { get; set; } = new List<ProductUsage>();

    [InverseProperty("LastModifiedBy")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    [ForeignKey("UserId")]
    [InverseProperty("Users")]
    public virtual ICollection<AspNetRole> Roles { get; set; } = new List<AspNetRole>();
}
