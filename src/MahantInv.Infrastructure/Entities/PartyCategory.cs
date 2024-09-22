using MahantInv.SharedKernel;
using MahantInv.SharedKernel.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahantInv.Infrastructure.Entities
{
    [Table("PartyCategories")]
    public class PartyCategory : BaseEntity, IAggregateRoot
    {
        public string Name { get; set; }

        [Dapper.Contrib.Extensions.Write(false)]
        [InverseProperty("Category")]
        public virtual ICollection<Party> Parties { get; set; } = new List<Party>();
    }
}
