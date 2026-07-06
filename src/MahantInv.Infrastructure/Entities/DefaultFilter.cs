using MahantInv.SharedKernel;
using MahantInv.SharedKernel.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahantInv.Infrastructure.Entities
{
    [Table("DefaultFilters")]
    public class DefaultFilter : BaseEntity, IAggregateRoot
    {
        [Required]
        [StringLength(20)]
        public string Code { get; set; }
        [Length(2, 50, ErrorMessage = "Minimum 2 & Maximum 50 characters allowed.")]
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string FilterData { get; set; }
    }
}
