using MahantInv.Web.Infrastructure.Entities;

namespace MahantInv.Web.Infrastructure.ViewModels
{
    public class PartyVM : Party
    {
        public string LastModifiedBy { get; set; }
        public string Category { get; set; }
    }

}
