using MahantInv.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.ViewModels
{
    public class ProductUsageVM: ProductUsage
    {
        public string ProductName { get; set; }
        public string LastModifiedBy { get; set; }
        public string UsageDateFormat
        {
            get
            {
                return UsageDate.HasValue ? $"{UsageDate:dd/MM/yyyy}" : null;
            }
        }
    }
}
