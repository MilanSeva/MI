using MahantInv.Infrastructure.Entities;

namespace MahantInv.Infrastructure.ViewModels
{
    public class StorageVM : Storage
    {
        public string Status
        {
            get
            {
                return Enabled ? "Enabled" : "Disabled";
            }
        }

    }
}
