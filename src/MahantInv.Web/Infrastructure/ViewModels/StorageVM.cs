using MahantInv.Web.Infrastructure.Entities;

namespace MahantInv.Web.Infrastructure.ViewModels
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
