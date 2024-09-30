using AutoMapper;
using MahantInv.Infrastructure.Data;
using MahantInv.Infrastructure.Dtos.Purchase;
using MahantInv.Infrastructure.Entities;
using MahantInv.Infrastructure.Interfaces;
using MahantInv.Infrastructure.Utility;
using MahantInv.Infrastructure.ViewModels;
using MahantInv.SharedKernel.Interfaces;
using MahantInv.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NuGet.Packaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static MahantInv.Infrastructure.Utility.Meta;

namespace MahantInv.Web.Api
{
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
                IEnumerable<OrderListDto> data = await _orderRepository.GetOrders(filterModel.StartDate.Date, filterModel.EndDate.Date);

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
                await LogOrder(orderDto, isReceived: false);
                OrderVM orderVM = await _orderRepository.GetOrderById(orderDto.Id);
                return Ok(new { success = true, data = orderVM });
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
        private async Task<IActionResult> LogOrder(OrderCreateDto orderDto, bool isReceived)
        {
            Order order;
            if (orderDto.Id == 0)
            {
                order = _mapper.Map<Order>(orderDto);
            }
            else
            {
                order = await _context.Orders
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
            order.LastModifiedById = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            order.ModifiedAt = DateTime.UtcNow;
            order.StatusId = isReceived ? OrderStatusTypes.Received : OrderStatusTypes.Ordered;

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
                    order.PaymentStatus = transAmout >= order.NetAmount ? Meta.PaymentStatuses.Paid : Meta.PaymentStatuses.PartiallyPaid;
                }
            }
            if (isReceived)
            {
                order.ReceivedDate = Meta.Now;
                ProductInventory? productInventory = await _context.ProductInventories.SingleOrDefaultAsync(i => i.ProductId == order.ProductId);
                if (productInventory == null)
                {
                    productInventory = new()
                    {
                        LastModifiedById = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                        RefNo = order.RefNo,
                        ModifiedAt = DateTime.UtcNow,
                        ProductId = order.ProductId.Value,
                        Quantity = order.Quantity.Value
                    };
                    await _context.ProductInventories.AddAsync(productInventory);
                }
                else
                {
                    ProductInventoryHistory piHistory = _mapper.Map<ProductInventoryHistory>(productInventory);
                    _context.ProductInventoryHistories.Add(piHistory);
                    productInventory.RefNo = order.RefNo;
                    productInventory.ModifiedAt = order.ModifiedAt;
                    productInventory.Quantity += order.ReceivedQuantity.Value;
                }
            }
            if (orderDto.Id == 0)
            {
                await _context.Orders.AddAsync(order);
            }

            await _context.SaveChangesAsync();
            return null;

        }
        [HttpGet("order/byid/{orderId}")]
        public async Task<object> OrderGetById(int orderId)
        {
            try
            {
                Order order = await _orderRepository.GetOrderById(orderId);
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
                    if (orderDto.OrderDate.Value > DateTime.Today.Date)
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
                    var existingOrder = await _context.Orders.FindAsync(orderDto.Id);
                    if (existingOrder == null)
                    {
                        ModelState.AddModelError(nameof(orderDto.Id), "Order not found");
                    }
                }
                await LogOrder(orderDto, isReceived: false);
                OrderVM orderVM = await _orderRepository.GetOrderById(orderDto.Id);
                return Ok(new { success = true, data = orderVM });
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
                OrderVM orderVM = await _orderRepository.GetOrderById(orderId);
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
