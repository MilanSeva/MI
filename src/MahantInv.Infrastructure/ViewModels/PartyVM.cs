using MahantInv.Infrastructure.Entities;

namespace MahantInv.Infrastructure.ViewModels
{
    public class PartyVM : Party
    {
        public string LastModifiedBy { get; set; }
        public string Category { get; set; }
    }

}
