using MahantInv.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.ViewModels
{
    public class StorageVM : Storage
    {
        public string Status { get {
                return Enabled ? "Enabled" : "Disabled";
            } }

    }
}
