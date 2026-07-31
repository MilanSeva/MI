using MahantInv.Web.Infrastructure.Identity;
using MahantInv.Web.Infrastructure;
using MahantInv.Web.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahantInv.Web.Infrastructure.Entities
{
    [Table("Parties")]
    public class Party : BaseEntity, IAggregateRoot
    {
        [Required, Display(Name = "Party Name")]
        public string Name { get; set; }
        [Required, Display(Name = "Party Type")]
        public string Type { get; set; }
        [Required, Display(Name = "Category")]
        public int? CategoryId { get; set; }
        [Display(Name = "Primary Contact")]
        public string PrimaryContact { get; set; }
        [Display(Name = "City")]
        public string City { get; set; }
        public string Country { get; set; }
        public string LastModifiedById { get; set; }
        public DateTime? ModifiedAt { get; set; }

        [ForeignKey("CategoryId")]
        [InverseProperty("Parties")]
        public virtual PartyCategory Category { get; set; }

        [ForeignKey("LastModifiedById")]
        [InverseProperty("Parties")]
        public virtual MIIdentityUser LastModifiedBy { get; set; }

        [InverseProperty("Party")]
        public virtual ICollection<OrderTransaction> OrderTransactions { get; set; } = new List<OrderTransaction>();

        [InverseProperty("Seller")]
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
