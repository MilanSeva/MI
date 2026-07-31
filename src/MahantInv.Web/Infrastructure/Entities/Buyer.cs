using MahantInv.Web.Infrastructure;
using MahantInv.Web.Infrastructure.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahantInv.Web.Infrastructure.Entities
{
    [Table("Buyers")]
    public class Buyer : BaseEntity, IAggregateRoot
    {
        [Required, MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(13)]
        public string Contact { get; set; }
    }
}
