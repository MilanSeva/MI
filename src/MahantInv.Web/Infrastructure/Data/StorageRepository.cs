using MahantInv.Web.Infrastructure.Entities;
using MahantInv.Web.Infrastructure.Interfaces;
using MahantInv.Web.Infrastructure.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MahantInv.Web.Infrastructure.Data
{
    public class StorageRepository : EfRepository<Storage>, IStorageRepository
    {
        public StorageRepository(MIDbContext context) : base(context)
        {
        }

        public Task<StorageVM> GetStorageById(int storageId)
        {
            return _context.Storages
                .Where(s => s.Id == storageId)
                .Select(s => new StorageVM { Id = s.Id, Name = s.Name, Enabled = s.Enabled })
                .SingleAsync();
        }

        public async Task<IEnumerable<StorageVM>> GetStorages()
        {
            return await _context.Storages
                .Select(s => new StorageVM { Id = s.Id, Name = s.Name, Enabled = s.Enabled })
                .ToListAsync();
        }
    }
}
