using MahantInv.Web.Infrastructure;
using MahantInv.Web.Infrastructure.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahantInv.Web.Infrastructure.Entities
{
    [Table("Storages")]
    public class Storage : BaseEntity, IAggregateRoot
    {
        [Required, Display(Name = "Storage Name")]
        public string Name { get; set; }
        public bool Enabled { get; set; }

    }
}
