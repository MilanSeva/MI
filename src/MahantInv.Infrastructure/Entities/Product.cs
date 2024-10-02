using AutoMapper;
using MahantInv.Infrastructure.Dtos.Product;
using MahantInv.Infrastructure.Identity;
using MahantInv.SharedKernel;
using MahantInv.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahantInv.Infrastructure.Entities
{
    [AutoMap(typeof(ProductCreateDto))]
    [Table("Products")]
    public partial class Product : BaseEntity, IAggregateRoot
    {
        public string? PicturePath { get; set; }
        [Required(ErrorMessage = "Product Name field is required"), Display(Name = "Product Name")]
        public string Name { get; set; }
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
        [Required(ErrorMessage = "Storage field is required"), Display(Name = "Storage")]
        public bool Enabled { get; set; }
        public string LastModifiedById { get; set; }
        public DateTime? ModifiedAt { get; set; }
        [Dapper.Contrib.Extensions.Write(false)]
        public List<ProductStorage> ProductStorages { get; set; }

        [ForeignKey("LastModifiedById")]
        [InverseProperty("Products")]
        [Dapper.Contrib.Extensions.Write(false)]
        public virtual MIIdentityUser LastModifiedBy { get; set; }

        [InverseProperty("Product")]
        [Dapper.Contrib.Extensions.Write(false)]
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        [InverseProperty("Product")]
        [Dapper.Contrib.Extensions.Write(false)]
        public virtual ProductInventory ProductInventory { get; set; } = new ProductInventory();

        [InverseProperty("Product")]
        [Dapper.Contrib.Extensions.Write(false)]
        public virtual ICollection<ProductInventoryHistory> ProductInventoryHistories { get; set; } = new List<ProductInventoryHistory>();

        [InverseProperty("Product")]
        [Dapper.Contrib.Extensions.Write(false)]
        public virtual ICollection<ProductUsage> ProductUsages { get; set; } = new List<ProductUsage>();

        [ForeignKey("UnitTypeCode")]
        [InverseProperty("Products")]
        [Dapper.Contrib.Extensions.Write(false)]
        public virtual UnitType UnitTypeCodeNavigation { get; set; }

        [InverseProperty("Product")]
        [Dapper.Contrib.Extensions.Write(false)]
        public virtual ICollection<ProductExpiry> ProductExpiries { get; set; } = new List<ProductExpiry>();
    }

}
