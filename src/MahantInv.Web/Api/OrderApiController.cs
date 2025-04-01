using AutoMapper;
using MahantInv.Infrastructure.Data;
using MahantInv.Infrastructure.Dtos.Purchase;
using MahantInv.Infrastructure.Entities;
using MahantInv.Infrastructure.Interfaces;
using MahantInv.Infrastructure.Utility;
using MahantInv.SharedKernel.Interfaces;
using MahantInv.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static MahantInv.Infrastructure.Utility.Meta;

namespace MahantInv.Web.Api
{
    [Authorize(Roles = Roles.Admin + "," + Roles.User)]
    public class OrderApiController : BaseApiController
    {
        private readonly ILogger<OrderApiController> _logger;
        private readonly IOrdersRepository _orderRepository;
        private readonly IProductInventoryRepository _productInventoryRepository;
        private readonly IAsyncRepository<ProductInventoryHistory> _productInventoryHistoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAsyncRepository<OrderTransaction> _orderTransactionRepository;
        private readonly MIDbContext _context;
        public OrderApiController(MIDbContext context, IMapper mapper, IUnitOfWork unitOfWork, IAsyncRepository<OrderTransaction> orderTransactionRepository, IAsyncRepository<ProductInventoryHistory> productInventoryHistoryRepository, IProductInventoryRepository productInventoryRepository, ILogger<OrderApiController> logger, IOrdersRepository orderRepository) : base(mapper)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _productInventoryRepository = productInventoryRepository;
            _unitOfWork = unitOfWork;
            _productInventoryHistoryRepository = productInventoryHistoryRepository;
            _orderTransactionRepository = orderTransactionRepository;
            _context = context;
        }
        [HttpPost("orders")]
        public async Task<object> GetAllOrders([FromBody] FilterModel filterModel)
        {
            try
            {
                IEnumerable<OrderListDto> data = await _orderRepository.GetOrders(filterModel.StartDate, filterModel.EndDate);

                return Ok(data);
            }
            catch (Exception e)
            {
                string GUID = Guid.NewGuid().ToString();
                _logger.LogError(e, GUID, null);
                return BadRequest(new { success = false, errors = new[] { "Unexpected Error " + GUID } });
            }
        }
        [HttpPost("order/save")]
        public async Task<object> SaveOrder([FromBody] OrderCreateDto orderDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    List<ModelErrorCollection> errors = ModelState.Select(x => x.Value.Errors)
                          .Where(y => y.Count > 0)
                          .ToList();
                    return BadRequest(new { success = false, errors });
                }
                if (orderDto.Id != 0)
                {
                    var order = await _context.Orders
                        .Include(o => o.Status)
                        .SingleOrDefaultAsync(o => o.Id == orderDto.Id);
                    if (order == null)
                    {
                        ModelState.AddModelError(nameof(orderDto.Status), "Order not found");
                        List<ModelErrorCollection> errors = ModelState.Select(x => x.Value.Errors)
                              .Where(y => y.Count > 0)
                              .ToList();
                        return BadRequest(new { success = false, errors });

                    }
                    if (order.Status.Title == Meta.OrderStatusTypes.Cancelled)
                    {
                        ModelState.AddModelError(nameof(orderDto.Quantity), "Order is cancelled");
                        List<ModelErrorCollection> errors = ModelState.Select(x => x.Value.Errors)
                             .Where(y => y.Count > 0)
                             .ToList();
                        return BadRequest(new { success = false, errors });
                    }
                }
                orderDto.Id = await LogOrder(orderDto, isReceived: false);
                IEnumerable<OrderListDto> data = await _orderRepository.GetOrders(null, null, orderDto.Id);
                return Ok(new { success = true, data });
            }
            catch (Exception e)
            {
                string GUID = Guid.NewGuid().ToString();
                _logger.LogError(e, GUID, null);
                ModelState.AddModelError("", "Unexpected Error " + GUID);
                List<ModelErrorCollection> errors = ModelState.Select(x => x.Value.Errors)
                          .Where(y => y.Count > 0)
                          .ToList();
                return BadRequest(new { success = false, errors });
            }
        }
        private async Task<int> LogOrder(OrderCreateDto orderDto, bool isReceived)
        {
            Order order;
            if (orderDto.Id == 0)
            {
                order = _mapper.Map<Order>(orderDto);
            }
            else
            {
                order = await _context.Orders
                    .Include(o => o.Status)
                   .Include(o => o.OrderTransactions)
                   .Include(o => o.ProductExpiries)
                   .SingleOrDefaultAsync(o => o.Id == orderDto.Id);

                if (order.OrderTransactions != null && order.OrderTransactions.Any())
                {
                    order.OrderTransactions.RemoveAll(o => true);
                }
                if (order.ProductExpiries != null && order.ProductExpiries.Any())
                {
                    order.ProductExpiries.RemoveAll(o => true);
                }
            }
            order = _mapper.Map<OrderCreateDto, Order>(orderDto, order);
            order.LastModifiedById = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            order.ModifiedAt = DateTime.UtcNow;
            if (orderDto.Id == 0 || order.Status.Title != Meta.OrderStatusTypes.Received)
            {
                order.StatusId = isReceived ? OrderStatusTypes.Received : OrderStatusTypes.Ordered;
            }

            order.RefNo = Guid.NewGuid().ToString();

            if (orderDto.ProductExpiries != null && orderDto.ProductExpiries.Any())
            {
                order.ProductExpiries.AddRange(
                    orderDto.ProductExpiries.Select(e => new ProductExpiry
                    {
                        ProductId = order.ProductId.Value,
                        ExpiryDate = e,
                        IsArchive = false,
                    })
                    );
            }
            order.OrderTransactions = _mapper.Map<List<OrderTransaction>>(orderDto.OrderTransactions);
            order.PaymentStatus = Meta.PaymentStatuses.Unpaid;
            if (order.OrderTransactions != null && order.OrderTransactions.Any())
            {
                double transAmout = order.OrderTransactions.Sum(e => (double)e.Amount);
                if (transAmout > 0)
                {
                    order.PaymentStatus = transAmout >= (order.NetAmount.HasValue ? Math.Truncate(order.NetAmount.Value) : order.NetAmount) ? Meta.PaymentStatuses.Paid : Meta.PaymentStatuses.PartiallyPaid;
                }
            }
            if (isReceived)
            {
                ProductInventory? productInventory = await _context.ProductInventories.FindAsync(order.ProductId);
                if (productInventory == null)
                {
                    productInventory = new()
                    {
                        LastModifiedById = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                        RefNo = order.RefNo,
                        ModifiedAt = Meta.Now,
                        ProductId = order.ProductId.Value,
                        Quantity = order.ReceivedQuantity.Value
                    };
                    await _context.ProductInventories.AddAsync(productInventory);
                }
                else
                {
                    ProductInventoryHistory piHistory = _mapper.Map<ProductInventoryHistory>(productInventory);
                    _context.ProductInventoryHistories.Add(piHistory);
                    productInventory.RefNo = order.RefNo;
                    productInventory.ModifiedAt = Meta.Now;
                    productInventory.LastModifiedById = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    productInventory.Quantity += order.ReceivedQuantity.Value;
                }
            }
            if (orderDto.Id == 0)
            {
                await _context.Orders.AddAsync(order);
            }

            await _context.SaveChangesAsync();
            return order.Id;

        }
        [HttpGet("order/byid/{orderId}")]
        public async Task<object> OrderGetById(int orderId)
        {
            try
            {
                OrderCreateDto order = await _orderRepository.GetOrderById(orderId);
                return Ok(order);
            }
            catch (Exception e)
            {
                string GUID = Guid.NewGuid().ToString();
                _logger.LogError(e, GUID, null);
                return BadRequest(new { success = false, errors = new[] { "Unexpected Error " + GUID } });
            }
        }
        [HttpPost("order/receive")]
        public async Task<object> ReceiveOrder([FromBody] OrderCreateDto orderDto)
        {
            try
            {
                if (!orderDto.Quantity.HasValue)
                {
                    ModelState.AddModelError(nameof(orderDto.Quantity), "Quantity field is required");
                }
                if (orderDto.Quantity <= 0)
                {
                    ModelState.AddModelError(nameof(orderDto.Quantity), "Quantity larger than 0");
                }
                if (orderDto.ReceivedQuantity == null)
                {
                    ModelState.AddModelError(nameof(orderDto.ReceivedQuantity), "Received Quantity field is required");
                }
                if (orderDto.ReceivedQuantity <= 0)
                {
                    ModelState.AddModelError(nameof(orderDto.ReceivedQuantity), "Received Quantity larger than 0");
                }
                if (orderDto.ReceivedDate == null)
                {
                    ModelState.AddModelError(nameof(orderDto.ReceivedDate), "Received Date field is required");
                }
                if (!orderDto.OrderDate.HasValue)
                {
                    ModelState.AddModelError(nameof(orderDto.OrderDate), "Order Date field is required");
                }
                else
                {
                    if (orderDto.OrderDate.Value > DateOnly.FromDateTime(DateTime.Today))
                    {
                        ModelState.AddModelError(nameof(orderDto.OrderDate), "Order Date can't be future date");
                    }
                }
                if (!ModelState.IsValid)
                {
                    List<ModelErrorCollection> errors = ModelState.Select(x => x.Value.Errors)
                          .Where(y => y.Count > 0)
                          .ToList();
                    return BadRequest(new { success = false, errors });
                }
                if (orderDto.Id != 0)
                {
                    var existingOrder = await _context.Orders.Include(o => o.Status).SingleOrDefaultAsync(o => o.Id == orderDto.Id);
                    if (existingOrder == null)
                    {
                        ModelState.AddModelError(nameof(orderDto.Status), "Order not found");
                        List<ModelErrorCollection> errors = ModelState.Select(x => x.Value.Errors)
                              .Where(y => y.Count > 0)
                              .ToList();
                        return BadRequest(new { success = false, errors });

                    }
                    if (existingOrder.Status.Title == Meta.OrderStatusTypes.Cancelled || existingOrder.Status.Title == Meta.OrderStatusTypes.Received)
                    {
                        ModelState.AddModelError(nameof(orderDto.Quantity), "Order is cancelled/received");
                        List<ModelErrorCollection> errors = ModelState.Select(x => x.Value.Errors)
                             .Where(y => y.Count > 0)
                             .ToList();
                        return BadRequest(new { success = false, errors });
                    }

                }
                orderDto.Id = await LogOrder(orderDto, isReceived: true);
                IEnumerable<OrderListDto> data = await _orderRepository.GetOrders(null, null, orderDto.Id);
                return Ok(new { success = true, data });
            }
            catch (Exception e)
            {
                string GUID = Guid.NewGuid().ToString();
                _logger.LogError(e, GUID, null);
                return BadRequest(new { success = false, errors = new[] { "Unexpected Error " + GUID } });
            }
        }
        [HttpPost("order/cancel")]
        public async Task<object> CancelOrder([FromBody] int orderId)
        {
            try
            {
                Order oldOrder = await _orderRepository.GetByIdAsync(orderId);
                if (!oldOrder.StatusId.Equals(Meta.OrderStatusTypes.Ordered))
                {
                    return BadRequest(new { success = false, errors = new[] { "Order not in Ordered state." } });
                }
                oldOrder.StatusId = Meta.OrderStatusTypes.Cancelled;
                oldOrder.LastModifiedById = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                oldOrder.ModifiedAt = DateTime.UtcNow;
                await _orderRepository.UpdateAsync(oldOrder);
                OrderCreateDto orderVM = await _orderRepository.GetOrderById(orderId);
                return Ok(new { success = true, data = orderVM });
            }
            catch (Exception e)
            {
                string GUID = Guid.NewGuid().ToString();
                _logger.LogError(e, GUID, null);
                return BadRequest(new { success = false, errors = new[] { "Unexpected Error " + GUID } });
            }
        }
    }
}
