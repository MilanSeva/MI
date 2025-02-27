﻿using AutoMapper;
using MahantInv.Infrastructure.Entities;
using MahantInv.Infrastructure.Interfaces;
using MahantInv.Infrastructure.Utility;
using MahantInv.Infrastructure.ViewModels;
using MahantInv.SharedKernel.Interfaces;
using MahantInv.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MahantInv.Web.Api
{
    [Authorize]
    public class HomeApiController : BaseApiController
    {
        private readonly ILogger<HomeApiController> _logger;
        private readonly IProductInventoryRepository _productInventoryRepository;
        private readonly IAsyncRepository<ProductInventoryHistory> _productInventoryHistoryRepository;
        private readonly IProductUsageRepository _productUsageRepository;
        private readonly IUnitOfWork _unitOfWork;
        public HomeApiController(IUnitOfWork unitOfWork, IProductUsageRepository productUsageRepository, IAsyncRepository<ProductInventoryHistory> productInventoryHistoryRepository, IProductInventoryRepository productInventoryRepository, ILogger<HomeApiController> logger, IMapper mapper) : base(mapper)
        {
            _logger = logger;
            _productInventoryRepository = productInventoryRepository;
            _unitOfWork = unitOfWork;
            _productUsageRepository = productUsageRepository;
            _productInventoryHistoryRepository = productInventoryHistoryRepository;
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

                ProductInventoryHistory productInventoryHistory = new()
                {
                    ProductId = productInventory.ProductId,
                    LastModifiedById = productInventory.LastModifiedById,
                    ModifiedAt = productInventory.ModifiedAt,
                    Quantity = productInventory.Quantity,
                    RefNo = productInventory.RefNo
                };

                productInventory.Quantity -= productUsageModel.Quantity;
                productInventory.RefNo = productUsage.RefNo;
                productInventory.LastModifiedById = productUsage.LastModifiedById;
                productInventory.ModifiedAt = Meta.Now;

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
    }
}
