using MahantInv.Web.Infrastructure;
using MahantInv.Web.Infrastructure.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahantInv.Web.Infrastructure.Entities
{
    [Table("ProductStorages")]
    public class ProductStorage : BaseEntity, IAggregateRoot
    {

        //[NotMapped]
        //public virtual string? StorageName { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        public int ProductId { get; set; }

        [ForeignKey("StorageId")]
        public virtual Storage Storage { get; set; }
        public int StorageId { get; set; }
    }
}
