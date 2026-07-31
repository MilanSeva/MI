using MahantInv.Web.Infrastructure.Dtos.Purchase;
using MahantInv.Web.Infrastructure.Entities;
using MahantInv.Web.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MahantInv.Web.Infrastructure.Interfaces
{
    public interface IOrdersRepository : IAsyncRepository<Order>
    {
        Task<IEnumerable<OrderListDto>> GetOrders(DateOnly? startDate = null, DateOnly? endDate = null, int? Id = null);
        Task<OrderCreateDto> GetOrderById(int orderId);
        Task DeleteOrderTransactionByOrderId(int orderId);
    }
}
