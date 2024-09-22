using MahantInv.SharedKernel.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahantInv.Infrastructure.Entities
{
    [Table("UnitTypes")]
    public class UnitType : IAggregateRoot
    {
        [Key]
        [Dapper.Contrib.Extensions.ExplicitKey, Required]
        public string Code { get; set; }
        [Required, Display(Name = "Unit Type Name")]
        public string Name { get; set; }
        //public List<BaseDomainEvent> Events = new();

        [Dapper.Contrib.Extensions.Write(false)]
        [InverseProperty("UnitTypeCodeNavigation")]
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
