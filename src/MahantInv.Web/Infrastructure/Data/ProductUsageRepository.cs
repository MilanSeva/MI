using MahantInv.Web.Infrastructure.Entities;
using MahantInv.Web.Infrastructure.Interfaces;
using MahantInv.Web.Infrastructure.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MahantInv.Web.Infrastructure.Data
{
    public class ProductUsageRepository : EfRepository<ProductUsage>, IProductUsageRepository
    {
        public ProductUsageRepository(MIDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProductUsageVM>> GetProductUsages()
        {
            return await _context.ProductUsages
                .OrderByDescending(pu => pu.Id)
                .Take(500)
                .Select(pu => new ProductUsageVM
                {
                    Id = pu.Id,
                    ProductId = pu.ProductId,
                    Quantity = pu.Quantity,
                    RefNo = pu.RefNo,
                    LastModifiedById = pu.LastModifiedById,
                    ModifiedAt = pu.ModifiedAt,
                    Buyer = pu.Buyer,
                    UsageDate = pu.UsageDate,
                    Note = pu.Note,
                    ProductName = pu.Product.Name,
                    LastModifiedBy = pu.LastModifiedBy.Email
                })
                .ToListAsync();
        }

        public Task<ProductUsageVM> GetProductUsageById(int id)
        {
            return _context.ProductUsages
                .Where(pu => pu.Id == id)
                .Select(pu => new ProductUsageVM
                {
                    Id = pu.Id,
                    ProductId = pu.ProductId,
                    Quantity = pu.Quantity,
                    RefNo = pu.RefNo,
                    LastModifiedById = pu.LastModifiedById,
                    ModifiedAt = pu.ModifiedAt,
                    Buyer = pu.Buyer,
                    UsageDate = pu.UsageDate,
                    Note = pu.Note,
                    ProductName = pu.Product.Name,
                    LastModifiedBy = pu.LastModifiedBy.Email
                })
                .SingleAsync();
        }
    }
}
