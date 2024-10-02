using MahantInv.Infrastructure.Entities;

namespace MahantInv.Infrastructure.ViewModels
{
    public class ProductVM : Product
    {
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
