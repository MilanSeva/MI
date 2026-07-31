using MahantInv.Web.Infrastructure.Entities;
using MahantInv.Web.Infrastructure.Interfaces;
using MahantInv.Web.Infrastructure.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MahantInv.Web.Infrastructure.Data
{
    public class ProductsRepository : EfRepository<Product>, IProductsRepository
    {
        public ProductsRepository(MIDbContext context) : base(context)
        {
        }

        public async Task AddProductStorage(ProductStorage productStorage)
        {
            await _context.ProductStorages.AddAsync(productStorage);
            await _context.SaveChangesAsync();
        }

        public async Task<ProductVM> GetProductById(int productId)
        {
            var product = await ProductsWithIncludes()
                .SingleAsync(p => p.Id == productId);
            return MapToProductVM(product);
        }

        public async Task<IEnumerable<ProductVM>> GetProducts()
        {
            var products = await ProductsWithIncludes().ToListAsync();
            return products.Select(MapToProductVM);
        }

        public Task<bool> IsProductExist(string unitTypeCode)
        {
            return _context.Products.AnyAsync(p => p.UnitTypeCode == unitTypeCode);
        }

        public Task RemoveProductStorages(int productId)
        {
            return _context.ProductStorages.Where(ps => ps.ProductId == productId).ExecuteDeleteAsync();
        }

        private IQueryable<Product> ProductsWithIncludes()
        {
            return _context.Products
                .Include(p => p.ProductStorages).ThenInclude(ps => ps.Storage)
                .Include(p => p.LastModifiedBy)
                .Include(p => p.UnitTypeCodeNavigation)
                .Include(p => p.ProductInventory);
        }

        private static ProductVM MapToProductVM(Product p)
        {
            return new ProductVM
            {
                Id = p.Id.ToString(),
                PicturePath = p.PicturePath,
                Name = p.Name,
                GujaratiName = p.GujaratiName,
                Size = p.Size,
                Description = p.Description,
                UnitTypeCode = p.UnitTypeCode,
                ReorderLevel = p.ReorderLevel,
                OrderBulkName = p.OrderBulkName,
                OrderBulkQuantity = p.OrderBulkQuantity,
                IsDisposable = p.IsDisposable,
                Company = p.Company,
                Enabled = p.Enabled,
                LastModifiedById = p.LastModifiedById,
                ModifiedAt = p.ModifiedAt,
                StorageIds = p.ProductStorages.Count > 0 ? string.Join(",", p.ProductStorages.Select(ps => ps.StorageId)) : null,
                Storage = p.ProductStorages.Count > 0 ? string.Join(",", p.ProductStorages.Select(ps => ps.Storage.Name)) : null,
                LastModifiedBy = p.LastModifiedBy?.UserName,
                UnitTypeName = p.UnitTypeCodeNavigation?.Name,
                CurrentStock = (decimal)(p.ProductInventory?.Quantity ?? 0)
            };
        }
    }
}
