using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dapper;
using MahantInv.Infrastructure.Dtos.Purchase;
using MahantInv.Infrastructure.Entities;
using MahantInv.Infrastructure.Interfaces;
using MahantInv.Infrastructure.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.Data
{
    public class OrdersRepository : DapperRepository<Order>, IOrdersRepository
    {
        private readonly MIDbContext _context;
        private readonly IMapper _mapper;
        public OrdersRepository(MIDbContext context, IMapper mapper, IDapperUnitOfWork uow) : base(uow)
        {
            _context = context;
            _mapper = mapper;
        }

        public Task DeleteOrderTransactionByOrderId(int orderId)
        {
            return db.ExecuteAsync("delete from OrderTransactions where OrderId = @orderId", new { orderId }, transaction: t);
        }

        public async Task<OrderCreateDto> GetOrderById(int orderId)
        {
            return await _context.Orders
                 .Where(o => o.Id == orderId)
                 .ProjectTo<OrderCreateDto>(_mapper.ConfigurationProvider)
                 .SingleOrDefaultAsync();
            //string sql = @"select * from vOrders o
            //    left outer join vOrderTransactions ot on o.Id = ot.OrderId
            //        where o.Id = @orderId";
            //var orderVMDictionary = new Dictionary<int, OrderVM>();
            //var result = await db.QueryAsync<OrderVM, OrderTransactionVM, OrderVM>(sql,
            //    (order, orderTransaction) =>
            //    {
            //        if (!orderVMDictionary.TryGetValue(order.Id, out OrderVM orderVMEntry))
            //        {
            //            orderVMEntry = order;
            //            orderVMDictionary.Add(orderVMEntry.Id, orderVMEntry);
            //        }
            //        if (orderTransaction != null)
            //        {
            //            if (orderVMEntry.OrderTransactionVMs == null)
            //            {
            //                orderVMEntry.OrderTransactionVMs = new();
            //            }
            //            orderVMEntry.OrderTransactionVMs.Add(orderTransaction);
            //        }
            //        return orderVMEntry;
            //    },
            //    new { orderId },
            //    splitOn: "Id",
            //     transaction: t);
            //return result.Distinct().Single();
        }

        public async Task<IEnumerable<OrderListDto>> GetOrders(DateTime? startDate = null, DateTime? endDate = null, int? Id = null)
        {
            //string sql = @"select * from vOrders o
            //    left outer join vOrderTransactions ot on o.Id = ot.OrderId
            //    where date(o.OrderDate) between date(@startDate) and date(@endDate)
            //    order by ModifiedAt desc";
            //var orderVMDictionary = new Dictionary<int, OrderVM>();
            //var result = await db.QueryAsync<OrderVM, OrderTransactionVM, OrderVM>(sql,
            //    (order, orderTransaction) =>
            //    {
            //        if (!orderVMDictionary.TryGetValue(order.Id, out OrderVM orderVMEntry))
            //        {
            //            orderVMEntry = order;
            //            orderVMDictionary.Add(orderVMEntry.Id, orderVMEntry);
            //        }
            //        if (orderTransaction != null)
            //        {
            //            if (orderVMEntry.OrderTransactionVMs == null)
            //            {
            //                orderVMEntry.OrderTransactionVMs = new();
            //            }
            //            orderVMEntry.OrderTransactionVMs.Add(orderTransaction);
            //        }
            //        return orderVMEntry;
            //    },
            //    new { startDate, endDate },
            //    splitOn: "Id",
            //     transaction: t);
            return await _context.Orders.Where(o => (startDate == null || endDate == null || o.OrderDate >= startDate && o.OrderDate <= endDate) && (Id == null || o.Id == Id))
                .ProjectTo<OrderListDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}
