using MahantInv.Web.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahantInv.Web.Infrastructure.Entities
{
    [Table("Categories")]
    public class Category : BaseEntity, IAggregateRoot
    {
        [Required, Display(Name = "Category Name")]
        public string Name { get; set; }

        [InverseProperty("Category")]
        public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
    }
}
