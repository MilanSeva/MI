using MahantInv.Web.Infrastructure.Dtos;
using MahantInv.Web.Infrastructure.Entities;
using MahantInv.Web.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MahantInv.Web.Infrastructure.Interfaces
{
    public interface IProductInventoryRepository : IAsyncRepository<ProductInventory>
    {
        Task<ProductInventory> GetByProductId(int productId);
        Task IFStockLowGenerateNotification(int productId);
        Task<IEnumerable<NotificationViewDTO>> GetNotificationByStatus(List<string> status);
    }
}
