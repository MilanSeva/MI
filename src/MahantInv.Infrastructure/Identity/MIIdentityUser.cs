﻿using MahantInv.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahantInv.Infrastructure.Identity
{
    public class MIIdentityUser : IdentityUser
    {
        public string AuthenticatorKey { get; set; }
        public bool IsMfaEnabled { get; set; }
        public bool IsActive { get; set; }

        [Dapper.Contrib.Extensions.Write(false)]
        [InverseProperty("LastModifiedBy")]
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        [Dapper.Contrib.Extensions.Write(false)]
        [InverseProperty("LastModifiedBy")]
        public virtual ICollection<Party> Parties { get; set; } = new List<Party>();

        [Dapper.Contrib.Extensions.Write(false)]
        [InverseProperty("LastModifiedBy")]
        public virtual ICollection<ProductInventory> ProductInventories { get; set; } = new List<ProductInventory>();

        [Dapper.Contrib.Extensions.Write(false)]
        [InverseProperty("LastModifiedBy")]
        public virtual ICollection<ProductInventoryHistory> ProductInventoryHistories { get; set; } = new List<ProductInventoryHistory>();

        [Dapper.Contrib.Extensions.Write(false)]
        [InverseProperty("LastModifiedBy")]
        public virtual ICollection<ProductUsage> ProductUsages { get; set; } = new List<ProductUsage>();

        [Dapper.Contrib.Extensions.Write(false)]
        [InverseProperty("LastModifiedBy")]
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

        [Dapper.Contrib.Extensions.Write(false)]
        //[InverseProperty("UserId")]
        public virtual ICollection<MIIdentityUserRole> UserRoles { get; set; } = new List<MIIdentityUserRole>();

    }
    public class MIIdentityRole : IdentityRole
    {
        public MIIdentityRole() { }
        public MIIdentityRole(string roleName)
        {
            Name = roleName;
        }
        public virtual ICollection<MIIdentityUserRole> UserRoles { get; set; }
    }

    public class MIIdentityUserRole : IdentityUserRole<string>
    {
        public virtual MIIdentityUser User { get; set; }
        public virtual MIIdentityRole Role { get; set; }
    }

}
