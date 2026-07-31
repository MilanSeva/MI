using MahantInv.Web.Infrastructure.Entities;
using MahantInv.Web.Infrastructure.ViewModels;
using MahantInv.Web.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MahantInv.Web.Infrastructure.Interfaces
{
    public interface IStorageRepository : IAsyncRepository<Storage>
    {
        Task<IEnumerable<StorageVM>> GetStorages();
        Task<StorageVM> GetStorageById(int storageId);
    }
}
