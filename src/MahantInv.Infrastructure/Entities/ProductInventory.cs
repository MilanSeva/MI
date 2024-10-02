using MahantInv.Infrastructure.Identity;
using MahantInv.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahantInv.Infrastructure.Entities
{
    [Table("ProductInventory")]
    [Index(nameof(ProductId), IsUnique = true)]
    public class ProductInventory : IAggregateRoot
    {
        [Key]
        public int ProductId { get; set; }
        public double? Quantity { get; set; }
        public string RefNo { get; set; }
        public string LastModifiedById { get; set; }
        public DateTime? ModifiedAt { get; set; }

        [Dapper.Contrib.Extensions.Write(false)]
        [ForeignKey("LastModifiedById")]
        [InverseProperty("ProductInventories")]
        public virtual MIIdentityUser LastModifiedBy { get; set; }

        [Dapper.Contrib.Extensions.Write(false)]
        [ForeignKey("ProductId")]
        [InverseProperty("ProductInventory")]
        public virtual Product Product { get; set; }
    }
}

