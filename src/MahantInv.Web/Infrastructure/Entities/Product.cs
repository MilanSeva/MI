using AutoMapper;
using MahantInv.Web.Infrastructure.Dtos.Product;
using MahantInv.Web.Infrastructure.Identity;
using MahantInv.Web.Infrastructure;
using MahantInv.Web.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahantInv.Web.Infrastructure.Entities
{
    [AutoMap(typeof(ProductCreateDto))]
    [Table("Products")]
    public partial class Product : BaseEntity, IAggregateRoot
    {
        public string? PicturePath { get; set; }
        [Required(ErrorMessage = "Product Name field is required"), Display(Name = "Product Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Product Gujarati Name field is required"), Display(Name = "Product Gujarati Name")]
        public string GujaratiName { get; set; }
        [Required(ErrorMessage = "Size field is required"), Display(Name = "Size")]
        public decimal? Size { get; set; }
        public string Description { get; set; }
        [Display(Name = "Unit Type")]
        public string UnitTypeCode { get; set; }
        [Required(ErrorMessage = "Reorder Level field is required"), Display(Name = "Reorder Level")]
        public decimal? ReorderLevel { get; set; }
        public string? OrderBulkName { get; set; }
        public int? OrderBulkQuantity { get; set; }

        [Display(Name = "Is Disposable?")]
        public bool IsDisposable { get; set; }
        public string Company { get; set; }
        public bool Enabled { get; set; }
        public string LastModifiedById { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public List<ProductStorage> ProductStorages { get; set; }

        [ForeignKey("LastModifiedById")]
        [InverseProperty("Products")]
        public virtual MIIdentityUser LastModifiedBy { get; set; }

        [InverseProperty("Product")]
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        [InverseProperty("Product")]
        public virtual ProductInventory ProductInventory { get; set; } = new ProductInventory();

        [InverseProperty("Product")]
        public virtual ICollection<ProductInventoryHistory> ProductInventoryHistories { get; set; } = new List<ProductInventoryHistory>();

        [InverseProperty("Product")]
        public virtual ICollection<ProductUsage> ProductUsages { get; set; } = new List<ProductUsage>();

        [ForeignKey("UnitTypeCode")]
        [InverseProperty("Products")]
        public virtual UnitType UnitTypeCodeNavigation { get; set; }

        [InverseProperty("Product")]
        public virtual ICollection<ProductExpiry> ProductExpiries { get; set; } = new List<ProductExpiry>();
    }

}
