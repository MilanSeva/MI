﻿using Dapper;
using MahantInv.Infrastructure.Entities;
using MahantInv.Infrastructure.Interfaces;
using MahantInv.Infrastructure.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.Data
{
    public class ProductUsageRepository : DapperRepository<ProductUsage>, IProductUsageRepository
    {
        public ProductUsageRepository(IDapperUnitOfWork uow) : base(uow)
        {
        }

        public Task<IEnumerable<ProductUsageVM>> GetProductUsages()
        {
            return db.QueryAsync<ProductUsageVM>(@"select pu.*,p.Name as ProductName, u.Email as LastModifiedBy 
                            from ProductUsages pu
                            inner join Products p on pu.ProductId = p.Id
                            inner join AspNetUsers u on pu.LastModifiedById = u.Id
                            order by pu.Id desc limit 500", transaction: t);
        }
        public Task<ProductUsageVM> GetProductUsageById(int id)
        {
            return db.QuerySingleAsync<ProductUsageVM>(@"select pu.*,p.Name as ProductName, u.Email as LastModifiedBy 
                            from ProductUsages pu
                            inner join Products p on pu.ProductId = p.Id
                            inner join AspNetUsers u on pu.LastModifiedById = u.Id 
                            where pu.Id = @id", new { id }, transaction: t);
        }
    }
}
