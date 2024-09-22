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
        Task<IEnumerable<OrderVM>> GetOrders(DateTime startDate, DateTime endDate);
        Task<OrderVM> GetOrderById(int orderId);
        Task DeleteOrderTransactionByOrderId(int orderId);
    }
}
