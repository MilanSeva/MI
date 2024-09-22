using MahantInv.SharedKernel;
using MahantInv.SharedKernel.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahantInv.Infrastructure.Entities
{
    [Table("OrderDocuments")]
    public class OrderDocument : BaseEntity, IAggregateRoot
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Extension { get; set; }
        [Required]
        public string Path { get; set; }
        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        [Dapper.Contrib.Extensions.Write(false)]
        public virtual Order Order { get; set; }
    }
}
