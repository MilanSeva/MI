using AutoMapper;
using AutoMapper.QueryableExtensions;
using MahantInv.Infrastructure.Data;
using MahantInv.Infrastructure.Dtos.Product;
using MahantInv.Infrastructure.Entities;
using MahantInv.Infrastructure.Interfaces;
using MahantInv.Infrastructure.Utility;
using MahantInv.Infrastructure.ViewModels;
using MahantInv.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MahantInv.Web.Api
{
    [Authorize]
    public class ProductApiController : BaseApiController
    {
        private readonly ILogger<ProductApiController> _logger;
        private readonly IProductsRepository _productRepository;
        private readonly IStorageRepository _storageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MIDbContext _context;
        // Allowed image MIME types for validation
        private static readonly string[] AllowedImageMimeTypes = new[]
        {
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/bmp",
        "image/svg+xml"
        };
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

        [HttpPost("product/image")]
        public async Task<IActionResult> ChangeProductImage(ChangeProductImageRequest request)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest(new { success = false, message = "No file uploaded." });
            }
            // Validate the MIME type of the uploaded file
            if (!IsImageFile(request.File))
            {
                return BadRequest(new { success = false, message = "Only image files are allowed." });
            }
            // Optionally, check the file extension if needed
            string extension = Path.GetExtension(request.File.FileName).ToLower();
            if (!IsValidImageExtension(extension))
            {
                return BadRequest(new { success = false, message = "Invalid image file extension." });
            }
            const long maxFileSize = 2 * 1024 * 1024; // 2 MB
            if (request.File.Length > maxFileSize)
            {
                return BadRequest(new { success = false, message = "File size exceeds 2 MB." });
            }
            Product product = await _context.Products.FindAsync(request.Id);
            if (product == null)
            {
                return BadRequest(new { success = false, message = "Product not found" });
            }

            // Save file to a specific location
            var uploadsFolder = Path.Combine("wwwroot", "ProductImages");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            // Create a relative path for the file
            var relativeFilePath = Path.Combine(uploadsFolder, $"{product.Id}_{Guid.NewGuid():N}{Path.GetExtension(request.File.FileName)}");

            // Save the file to the relative path
            var absoluteFilePath = Path.Combine(Directory.GetCurrentDirectory(), relativeFilePath);
            using (var stream = new FileStream(absoluteFilePath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream);
            }
            product.PicturePath = $"{relativeFilePath}".Replace("wwwroot", string.Empty);
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            ProductVM productVM = await _productRepository.GetProductById(product.Id);
            return Ok(new { success = true, data = productVM });
        }
        // Check if the file is a valid image based on MIME type
        private bool IsImageFile(IFormFile file)
        {
            // Check if the MIME type starts with "image/"
            return file.ContentType.StartsWith("image/");
        }

        // Optionally validate file extension to ensure it's an image
        private bool IsValidImageExtension(string extension)
        {
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg" };
            return allowedExtensions.Contains(extension);
        }
    }

}
