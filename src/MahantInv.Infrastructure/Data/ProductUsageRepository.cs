using MahantInv.Infrastructure.Entities;
using MahantInv.Infrastructure.Interfaces;
using MahantInv.Infrastructure.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.Data
{
    public class ProductUsageRepository : DapperRepository<ProductUsage>, IProductUsageRepository
    {
        private readonly MIDbContext _context;

        public ProductUsageRepository(IDapperUnitOfWork uow, MIDbContext context) : base(uow)
        {
            _context = context;
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
