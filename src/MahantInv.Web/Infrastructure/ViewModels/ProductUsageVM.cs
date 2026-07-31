using MahantInv.Web.Infrastructure.Entities;

namespace MahantInv.Web.Infrastructure.ViewModels
{
    public class ProductUsageVM : ProductUsage
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
