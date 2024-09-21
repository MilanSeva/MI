using MahantInv.Infrastructure.Entities;
using MahantInv.Infrastructure.ViewModels;
using MahantInv.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.Interfaces
{
    public interface IStorageRepository : IAsyncRepository<Storage>
    {
        Task<IEnumerable<StorageVM>> GetStorages();
        Task<StorageVM> GetStorageById(int storageId);
    }
}
