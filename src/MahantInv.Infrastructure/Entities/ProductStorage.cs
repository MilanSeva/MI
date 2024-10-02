using MahantInv.SharedKernel;
using MahantInv.SharedKernel.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahantInv.Infrastructure.Entities
{
    [Table("ProductStorages")]
    public class ProductStorage : BaseEntity, IAggregateRoot
    {

        //[NotMapped]
        //[Dapper.Contrib.Extensions.Write(false)]
        //public virtual string? StorageName { get; set; }

        [Dapper.Contrib.Extensions.Write(false)]
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        public int ProductId { get; set; }

        [Dapper.Contrib.Extensions.Write(false)]
        [ForeignKey("StorageId")]
        public virtual Storage Storage { get; set; }
        public int StorageId { get; set; }
    }
}
