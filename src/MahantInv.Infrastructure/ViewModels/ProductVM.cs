using MahantInv.Infrastructure.Entities;
using MahantInv.Infrastructure.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace MahantInv.Infrastructure.ViewModels
{
    public class ProductVM 
    {
        public string Id { get; set; }
        public string? PicturePath { get; set; }
        public string Name { get; set; }
        public string GujaratiName { get; set; }
        public decimal? Size { get; set; }
        public string Description { get; set; }
        public string UnitTypeCode { get; set; }
        public decimal? ReorderLevel { get; set; }
        public string? OrderBulkName { get; set; }
        public int? OrderBulkQuantity { get; set; }

        public bool IsDisposable { get; set; }
        public string Company { get; set; }
        public bool Enabled { get; set; }
        public string LastModifiedById { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public List<ProductStorage> ProductStorages { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        public virtual ProductInventory ProductInventory { get; set; } = new ProductInventory();

        public virtual ICollection<ProductInventoryHistory> ProductInventoryHistories { get; set; } = new List<ProductInventoryHistory>();

        public virtual ICollection<ProductUsage> ProductUsages { get; set; } = new List<ProductUsage>();

        public virtual UnitType UnitTypeCodeNavigation { get; set; }

        public virtual ICollection<ProductExpiry> ProductExpiries { get; set; } = new List<ProductExpiry>();
        public string? Storage { get; set; }
        public string? StorageIds { get; set; }
        public string LastModifiedBy { get; set; }
        public decimal CurrentStock { get; set; }
        public string UnitTypeName { get; set; }
        public string Disposable
        {
            get
            {
                return this.IsDisposable ? "Yes" : "No";
            }
        }
        public string FullName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Company))
                    return Name;
                return $"{Name} - {Company}";
            }
        }
        public string SizeUnitTypeCode
        {
            get
            {
                return $"{Size} {UnitTypeCode}";
            }
        }
    }
}
