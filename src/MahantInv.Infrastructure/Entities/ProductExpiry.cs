using MahantInv.SharedKernel;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahantInv.Infrastructure.Entities
{
    [Table("ProductExpiries")]
    public class ProductExpiry : BaseEntity
    {
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        [Dapper.Contrib.Extensions.Write(false)]
        public virtual Product Product { get; set; }

        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        [Dapper.Contrib.Extensions.Write(false)]
        public virtual Order Order { get; set; }

        public bool IsArchive { get; set; }

    }
}
