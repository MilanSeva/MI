using MahantInv.SharedKernel;
using MahantInv.SharedKernel.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahantInv.Infrastructure.Entities
{
    [Table("Storages")]
    public class Storage : BaseEntity, IAggregateRoot
    {
        [Required, Display(Name = "Storage Name")]
        public string Name { get; set; }
        public bool Enabled { get; set; }

    }
}
