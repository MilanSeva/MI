using AutoMapper;
using AutoMapper.QueryableExtensions;
using MahantInv.Web.Infrastructure.Dtos.Purchase;
using MahantInv.Web.Infrastructure.Entities;
using MahantInv.Web.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MahantInv.Web.Infrastructure.Data
{
    public class OrdersRepository : EfRepository<Order>, IOrdersRepository
    {
        private readonly IMapper _mapper;
        public OrdersRepository(MIDbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public Task DeleteOrderTransactionByOrderId(int orderId)
        {
            return _context.OrderTransactions.Where(ot => ot.OrderId == orderId).ExecuteDeleteAsync();
        }

        public async Task<OrderCreateDto> GetOrderById(int orderId)
        {
            return await _context.Orders
                 .Where(o => o.Id == orderId)
                 .ProjectTo<OrderCreateDto>(_mapper.ConfigurationProvider)
                 .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<OrderListDto>> GetOrders(DateOnly? startDate = null, DateOnly? endDate = null, int? Id = null)
        {
            var query = _context.Orders.AsQueryable();
            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate);
            }
            if (Id.HasValue)
            {
                query = query.Where(o => o.Id == Id.Value);
            }
            return await query
                .ProjectTo<OrderListDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}
