using MahantInv.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace MahantInv.Infrastructure.Dtos.Product
{
    public class ProductCreateDto : BaseEntity
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
        [Display(Name = "Order Bulk Name")]
        public string? OrderBulkName { get; set; }
        [Display(Name = "Order Bulk Quantity")]
        public int? OrderBulkQuantity { get; set; }

        [Display(Name = "Is Disposable?")]
        public bool IsDisposable { get; set; }
        public string Company { get; set; }
        [Required(ErrorMessage = "Storage field is required"), Display(Name = "Storage")]
        //public int? StorageId { get; set; }
        [Dapper.Contrib.Extensions.Write(false)]
        public string StorageNames { get; set; }

    }
}
