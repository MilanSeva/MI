using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.Dtos.Product
{
    public class ProductSearchDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? PicturePath { get; set; }
        public decimal? Size { get; set; }
        public string Description { get; set; } 
        public string UnitTypeCode { get; set; }
        public string Company { get; set; } 
        public string Storage { get; set; }
        public string? OrderBulkName { get; set; }
        public int? OrderBulkQuantity { get; set; }
    }
}
