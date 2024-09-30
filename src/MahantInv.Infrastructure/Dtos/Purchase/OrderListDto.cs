using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.Dtos.Purchase
{
    public class OrderListDto
    {
        public string Id { get; set; }
        public string Product { get; set; }
        public double? Quantity { get; set; }
        public double? ReceivedQuantity { get; set; }
        public string OrderBulkName { get; set; }
        public string Status { get; set; }
        public string? PaymentStatus { get; set; }
        public string? Seller { get; set; }
        public string? OrderDate { get; set; }
        public string? ReceivedDate { get; set; }
        public string Remark { get; set; }
        public string LastModifiedBy { get; set; }
        public string? ModifiedAt { get; set; }
    }
}
