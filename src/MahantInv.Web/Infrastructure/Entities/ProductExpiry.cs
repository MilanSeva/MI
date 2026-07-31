using MahantInv.Web.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahantInv.Web.Infrastructure.Entities
{
    [Table("ProductExpiries")]
    public class ProductExpiry : BaseEntity
    {
        public DateOnly ExpiryDate { get; set; }
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        public bool IsArchive { get; set; }

    }
}
