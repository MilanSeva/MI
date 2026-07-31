using Dapper;
using MahantInv.Web.Infrastructure.Entities;
using MahantInv.Web.Infrastructure.Interfaces;
using MahantInv.Web.Infrastructure.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MahantInv.Web.Infrastructure.Data
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
