using MahantInv.Web.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahantInv.Web.Infrastructure.Entities
{
    [Table("UnitTypes")]
    public class UnitType : IAggregateRoot
    {
        [Key]
        [Required]
        public string Code { get; set; }
        [Required, Display(Name = "Unit Type Name")]
        public string Name { get; set; }
        //public List<BaseDomainEvent> Events = new();

        [InverseProperty("UnitTypeCodeNavigation")]
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
