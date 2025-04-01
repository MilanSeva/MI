using AutoMapper;
using MahantInv.Infrastructure.Data;
using MahantInv.Infrastructure.Entities;
using MahantInv.Infrastructure.Interfaces;
using MahantInv.Infrastructure.Utility;
using MahantInv.Infrastructure.ViewModels;
using MahantInv.SharedKernel.Interfaces;
using MahantInv.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static MahantInv.Infrastructure.Utility.Meta;

namespace MahantInv.Web.Api
{
    [Authorize(Roles = Roles.Admin + "," + Roles.User)]
    public class HomeApiController : BaseApiController
    {
        private readonly ILogger<HomeApiController> _logger;
        private readonly IProductInventoryRepository _productInventoryRepository;
        private readonly IAsyncRepository<ProductInventoryHistory> _productInventoryHistoryRepository;
        private readonly IProductUsageRepository _productUsageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MIDbContext _context;
        public HomeApiController(IUnitOfWork unitOfWork, MIDbContext context, IProductUsageRepository productUsageRepository, IAsyncRepository<ProductInventoryHistory> productInventoryHistoryRepository, IProductInventoryRepository productInventoryRepository, ILogger<HomeApiController> logger, IMapper mapper) : base(mapper)
        {
            _logger = logger;
            _productInventoryRepository = productInventoryRepository;
            _unitOfWork = unitOfWork;
            _productUsageRepository = productUsageRepository;
            _productInventoryHistoryRepository = productInventoryHistoryRepository;
            _context = context;
        }
        [HttpGet("usages")]
        public async Task<IActionResult> GetUsages()
        {
            try
            {
                var data = await _productUsageRepository.GetProductUsages();
                return Ok(data);
            }
            catch (Exception e)
            {
                string GUID = Guid.NewGuid().ToString();
                _logger.LogError(e, GUID, null);
                return BadRequest(new { success = false, errors = new[] { "Unexpected Error " + GUID } });
            }
        }
        [HttpPost("product/usage")]
        public async Task<object> ProductUsage([FromBody] ProductUsageModel productUsageModel)
        {
            try
            {
                if (productUsageModel.Quantity <= 0)
                {
                    return BadRequest(new { success = false, errors = new[] { "Quantity must be larger than 0" } });
                }
                ProductUsage productUsage = new()
                {
                    ProductId = productUsageModel.ProductId,
                    Quantity = productUsageModel.Quantity,
                    Buyer = productUsageModel.Buyer,
                    RefNo = Guid.NewGuid().ToString(),
                    UsageDate = productUsageModel.UsageDate,
                    LastModifiedById = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                    ModifiedAt = DateTime.UtcNow
                };
                ProductInventory productInventory = await _productInventoryRepository.GetByProductId(productUsageModel.ProductId);

                if (productInventory == null)
                {
                    return BadRequest(new { success = false, errors = new[] { "Product/Stock not available" } });
                }

                productInventory.Quantity -= productUsageModel.Quantity;
                productInventory.RefNo = productUsage.RefNo;
                productInventory.LastModifiedById = productUsage.LastModifiedById;
                productInventory.ModifiedAt = Meta.Now;

                ProductInventoryHistory productInventoryHistory = new()
                {
                    ProductId = productInventory.ProductId,
                    LastModifiedById = productInventory.LastModifiedById,
                    ModifiedAt = productInventory.ModifiedAt,
                    Quantity = productInventory.Quantity,
                    RefNo = productInventory.RefNo
                };

                await _unitOfWork.BeginAsync();
                await _productInventoryHistoryRepository.AddAsync(productInventoryHistory);
                await _productInventoryRepository.UpdateAsync(productInventory);
                await _productUsageRepository.AddAsync(productUsage);
                await _productInventoryRepository.IFStockLowGenerateNotification(productUsage.ProductId.Value);
                await _unitOfWork.CommitAsync();

                ProductUsageVM productUsageVM = await _productUsageRepository.GetProductUsageById(productUsage.Id);
                return Ok(new { success = true, data = productUsageVM });
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();
                string GUID = Guid.NewGuid().ToString();
                _logger.LogError(e, GUID, null);
                return BadRequest(new { success = false, errors = new[] { "Unexpected Error " + GUID } });
            }
        }
        [HttpPut("product/usage/{id}")]
        public async Task<IActionResult> UpdateQuentity(int id, [FromBody] double quantity)
        {
            try
            {
                //update the code using EF
                ProductUsage productUsage = await _context.ProductUsages.FindAsync(id);
                if (productUsage == null)
                {
                    return BadRequest(new { success = false, errors = new[] { "Product Usage not found" } });
                }
                ProductInventory productInventory = await _context.ProductInventories.Where(pi => pi.ProductId == productUsage.ProductId).SingleOrDefaultAsync();
                if (productInventory == null)
                {
                    return BadRequest(new { success = false, errors = new[] { "Product/Stock not available" } });
                }
                
                productInventory.Quantity += productUsage.Quantity.Value;
                productInventory.Quantity -= quantity;
                productInventory.ModifiedAt = DateTime.UtcNow;

                productUsage.Quantity = quantity;
                productUsage.ModifiedAt = DateTime.UtcNow;
                //if (productInventoryHistory != null) then update quentity else add new record
                ProductInventoryHistory productInventoryHistory = await _context.ProductInventoryHistories.Where(pih => pih.RefNo == productUsage.RefNo).SingleOrDefaultAsync();
                if (productInventoryHistory != null)
                {
                    productInventoryHistory.Quantity = productInventory.Quantity;
                    productInventoryHistory.ModifiedAt = productInventory.ModifiedAt;
                    productInventoryHistory.LastModifiedById = productInventory.LastModifiedById;
                }
                else
                {
                    productInventoryHistory = new ProductInventoryHistory
                    {
                        ProductId = productInventory.ProductId,
                        Quantity = productInventory.Quantity,
                        RefNo = productInventory.RefNo,
                        LastModifiedById = productInventory.LastModifiedById,
                        ModifiedAt = productInventory.ModifiedAt
                    };
                    await _context.ProductInventoryHistories.AddAsync(productInventoryHistory);
                }
                await _context.SaveChangesAsync();
                return Ok(new { success = true });
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();
                string GUID = Guid.NewGuid().ToString();
                _logger.LogError(e, GUID, null);
                return BadRequest(new { success = false, errors = new[] { "Unexpected Error " + GUID } });
        }
        }

        [HttpDelete("product/usage/{id}")]
        public async Task<IActionResult> DeleteProductUsage(int id)
        {
            try
            {
                ProductUsage productUsage = await _context.ProductUsages.FindAsync(id);
                if (productUsage == null)
                {
                    return BadRequest(new { success = false, errors = new[] { "Product Usage not found" } });
                }
                ProductInventory productInventory = await _context.ProductInventories.Where(pi => pi.ProductId == productUsage.ProductId).SingleOrDefaultAsync();
                if (productInventory == null)
                {
                    return BadRequest(new { success = false, errors = new[] { "Product/Stock not available" } });
                }
                ProductInventoryHistory productInventoryHistory = await _context.ProductInventoryHistories.Where(pih => pih.RefNo == productUsage.RefNo).SingleOrDefaultAsync();
                //if (productInventoryHistory == null)
                //{
                //    return BadRequest(new { success = false, errors = new[] { "Product Inventory History not found" } });
                //}
                productInventory.Quantity += productUsage.Quantity.Value;
                //productInventory.RefNo = productInventoryHistory.RefNo;
                //productInventory.LastModifiedById = productInventoryHistory.LastModifiedById;
                productInventory.ModifiedAt = DateTime.UtcNow;

                _context.ProductUsages.Remove(productUsage);
                if (productInventoryHistory != null)
                {
                    _context.ProductInventoryHistories.Remove(productInventoryHistory);
                }
                await _context.SaveChangesAsync();
                //await _unitOfWork.BeginAsync();
                //await _productInventoryHistoryRepository.DeleteAsync(productInventoryHistory);
                //await _productInventoryRepository.UpdateAsync(productInventory);
                //await _productUsageRepository.DeleteAsync(productUsage);
                //await _unitOfWork.CommitAsync();
                return Ok(new { success = true });
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
