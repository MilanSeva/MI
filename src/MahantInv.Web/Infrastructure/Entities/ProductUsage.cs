using MahantInv.Web.Infrastructure.Identity;
using MahantInv.Web.Infrastructure;
using MahantInv.Web.Infrastructure.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahantInv.Web.Infrastructure.Entities
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
        public string Note { get; set; }

        [ForeignKey("LastModifiedById")]
        [InverseProperty("ProductUsages")]
        public virtual MIIdentityUser LastModifiedBy { get; set; }

        [ForeignKey("ProductId")]
        [InverseProperty("ProductUsages")]
        public virtual Product Product { get; set; }
    }
}
