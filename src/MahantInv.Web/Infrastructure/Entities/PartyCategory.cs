using MahantInv.Web.Infrastructure;
using MahantInv.Web.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahantInv.Web.Infrastructure.Entities
{
    [Table("PartyCategories")]
    public class PartyCategory : BaseEntity, IAggregateRoot
    {
        public string Name { get; set; }

        [InverseProperty("Category")]
        public virtual ICollection<Party> Parties { get; set; } = new List<Party>();
    }
}
