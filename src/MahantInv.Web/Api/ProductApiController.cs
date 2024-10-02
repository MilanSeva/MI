using AutoMapper;
using AutoMapper.QueryableExtensions;
using MahantInv.Infrastructure.Data;
using MahantInv.Infrastructure.Dtos.Product;
using MahantInv.Infrastructure.Entities;
using MahantInv.Infrastructure.Interfaces;
using MahantInv.Infrastructure.Utility;
using MahantInv.Infrastructure.ViewModels;
using MahantInv.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MahantInv.Web.Api
{
    public class ProductApiController : BaseApiController
    {
        private readonly ILogger<ProductApiController> _logger;
        private readonly IProductsRepository _productRepository;
        private readonly IStorageRepository _storageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MIDbContext _context;

        public ProductApiController(MIDbContext context, IUnitOfWork unitOfWork, IStorageRepository storageRepository, IMapper mapper, ILogger<ProductApiController> logger, IProductsRepository productRepository) : base(mapper)
        {
            _logger = logger;
            _productRepository = productRepository;
            _storageRepository = storageRepository;
            _unitOfWork = unitOfWork;
            _context = context;
        }
        [HttpGet("product/search")]
        public async Task<IEnumerable<ProductSearchDto>> ProductSearch()
        {
            return await _context.Products.Where(p => p.Enabled).ProjectTo<ProductSearchDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        [HttpGet("products")]
        public async Task<object> GetAllProducats()
        {
            try
            {
                IEnumerable<ProductVM> data = await _productRepository.GetProducts();
                return Ok(data);
            }
            catch (Exception e)
            {
                string GUID = Guid.NewGuid().ToString();
                _logger.LogError(e, GUID, null);
                return BadRequest(new { success = false, errors = new[] { "Unexpected Error " + GUID } });
            }
        }
        [HttpPost("product/save")]
        public async Task<object> SaveProduct([FromBody] ProductCreateDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    List<ModelErrorCollection> errors = ModelState.Select(x => x.Value.Errors)
                          .Where(y => y.Count > 0)
                          .ToList();
                    return BadRequest(errors);
                }
                if (string.IsNullOrWhiteSpace(input.StorageNames))
                {
                    return BadRequest(new { success = false, errors = new[] { "Storage field is required" } });
                }
                if (!input.Size.HasValue)
                {
                    return BadRequest(new { success = false, errors = new[] { "Size field is required" } });
                }

                Product? product;
                if (input.Id == 0)
                {
                    product = _mapper.Map<Product>(input);
                    product.Enabled = true;
                }
                else
                {
                    product = await _context.Products
                        .Include(p => p.ProductStorages)
                        .SingleOrDefaultAsync(p => p.Id == input.Id);
                    if (product == null)
                    {
                        return BadRequest(new { success = false, errors = new[] { "Product not found" } });
                    }
                    product = _mapper.Map<ProductCreateDto, Product>(input, product);
                }
                product.ModifiedAt = DateTime.UtcNow;
                product.LastModifiedById = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                if (input.Id != 0)
                {
                    product.ProductStorages.RemoveAll(p => true);
                }
                var storages = await _context.Storages.ToListAsync();
                product.ProductStorages = new();

                string[] storageNames = input.StorageNames.Split(",");
                foreach (string storageName in storageNames)
                {
                    Storage storage = storages.SingleOrDefault(s => s.Name == storageName);
                    if (storage == null)
                    {
                        product.ProductStorages.Add(new ProductStorage
                        {
                            Storage = new Storage { Name = storageName, Enabled = true }
                        });
                    }
                    else
                    {
                        product.ProductStorages.Add(new ProductStorage
                        {
                            ProductId = product.Id,
                            StorageId = storage.Id
                        });
                        storage.Enabled = true;
                        _context.Storages.Update(storage);
                    }
                }
                if (input.Id == 0)
                {
                    await _context.AddAsync(product);
                }
                
                await _context.SaveChangesAsync();

                ProductVM productVM = await _productRepository.GetProductById(product.Id);
                return Ok(new { success = true, data = productVM });
            }
            catch (Exception e)
            {
                string GUID = Guid.NewGuid().ToString();
                _logger.LogError(e, GUID, null);
                return BadRequest(new { success = false, errors = new[] { "Unexpected Error " + GUID } });
            }
        }
        [HttpGet("product/byid/{productId}")]
        public async Task<object> ProductGetById(int productId)
        {
            try
            {
                ProductVM product = await _productRepository.GetProductById(productId);
                return Ok(product);
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
