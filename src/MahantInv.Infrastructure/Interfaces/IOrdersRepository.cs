using MahantInv.Infrastructure.Dtos.Purchase;
using MahantInv.Infrastructure.Entities;
using MahantInv.Infrastructure.ViewModels;
using MahantInv.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.Interfaces
{
    public interface IOrdersRepository : IAsyncRepository<Order>
    {
        Task<IEnumerable<OrderListDto>> GetOrders(DateTime startDate, DateTime endDate);
        Task<OrderCreateDto> GetOrderById(int orderId);
        Task DeleteOrderTransactionByOrderId(int orderId);
    }
}
