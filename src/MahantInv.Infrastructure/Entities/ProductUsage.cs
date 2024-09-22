using MahantInv.Infrastructure.Identity;
using MahantInv.SharedKernel;
using MahantInv.SharedKernel.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahantInv.Infrastructure.Entities
{
    [Table("ProductUsages")]
    public class ProductUsage : BaseEntity, IAggregateRoot
    {
        public int? ProductId { get; set; }
        public double? Quantity { get; set; }
        public string RefNo { get; set; }
        public string LastModifiedById { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string Buyer { get; set; }
        public DateTime? UsageDate { get; set; }

        [Dapper.Contrib.Extensions.Write(false)]
        [ForeignKey("LastModifiedById")]
        [InverseProperty("ProductUsages")]
        public virtual MIIdentityUser LastModifiedBy { get; set; }

        [Dapper.Contrib.Extensions.Write(false)]
        [ForeignKey("ProductId")]
        [InverseProperty("ProductUsages")]
        public virtual Product Product { get; set; }
    }
}
