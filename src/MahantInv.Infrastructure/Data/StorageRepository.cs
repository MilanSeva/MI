using Dapper;
using MahantInv.Infrastructure.Entities;
using MahantInv.Infrastructure.Interfaces;
using MahantInv.Infrastructure.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.Data
{
    public class StorageRepository : DapperRepository<Storage>, IStorageRepository
    {
        public StorageRepository(IDapperUnitOfWork uow) : base(uow)
        {
        }

        public Task<StorageVM> GetStorageById(int storageId)
        {
            return db.QuerySingleAsync<StorageVM>(@"select * from Storages where Id = @storageId", new { storageId }, transaction: t);
        }

        public Task<IEnumerable<StorageVM>> GetStorages()
        {
            return db.QueryAsync<StorageVM>(@"select * from Storages", transaction: t);
        }
    }
}
