using MahantInv.Web.Infrastructure;
using MahantInv.Web.Infrastructure.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahantInv.Web.Infrastructure.Entities
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
        public virtual Order Order { get; set; }
    }
}
