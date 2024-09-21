using MahantInv.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.ViewModels
{
    public class PartyVM : Party
    {
        public string LastModifiedBy { get; set; }
        public string Category { get; set; }
        public string Address
        {
            get
            {
                return $"{Line1} {Line2}";
            }
        }
    }

}
