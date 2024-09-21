using MahantInv.Infrastructure.Entities;
using MahantInv.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.Interfaces
{
    public interface IProductInventoryRepository : IAsyncRepository<ProductInventory>
    {
        Task<ProductInventory> GetByProductId(int productId);
        Task IFStockLowGenerateNotification(int productId);
        Task<IEnumerable<Notification>> GetNotificationByStatus(List<string> status);
    }
}
