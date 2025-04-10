﻿using Dapper;
using MahantInv.Infrastructure.Entities;
using MahantInv.Infrastructure.Interfaces;
using MahantInv.Infrastructure.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.Data
{
    public class ProductsRepository : DapperRepository<Product>, IProductsRepository
    {
        public ProductsRepository(IDapperUnitOfWork uow) : base(uow)
        {
        }

        public Task AddProductStorage(ProductStorage productStorage)
        {
            return db.ExecuteAsync("insert into ProductStorages (ProductId,StorageId) values (@ProductId,@StorageId)", productStorage, transaction: t);
        }

        public Task<ProductVM> GetProductById(int productId)
        {
            return db.QuerySingleAsync<ProductVM>(@"
                        with storagecte as
                        (
                            select ps.ProductId,group_concat(s.Id) as StorageIds,group_concat(s.Name) as Storage from ProductStorages ps
                            left outer join Storages s on ps.StorageId = s.Id
                            where ps.ProductId = @productId
                            group by ps.ProductId
                        )
                        select p.Id,p.Name, p.GujaratiName, p.PicturePath, cast(p.Size as real) Size, p.Description, p.UnitTypeCode,p.OrderBulkName,p.OrderBulkQuantity,p.ReorderLevel, p.IsDisposable, p.Company, p.Enabled, p.LastModifiedById, p.ModifiedAt,
                    s.StorageIds,s.Storage, u.UserName as [LastModifiedBy], ut.Name as [UnitTypeName], pi.Quantity as [CurrentStock] 
                        from Products p
                        inner join AspNetUsers u on p.LastModifiedById = u.Id
                        left outer join storagecte s on p.Id = s.ProductId
                        left outer join UnitTypes ut on p.UnitTypeCode = ut.Code
                        left outer join ProductInventory pi on p.Id = pi.ProductId
                        where p.Id = @productId", new { productId }, transaction: t);
        }

        public Task<IEnumerable<ProductVM>> GetProducts()
        {
            return db.QueryAsync<ProductVM>(@"with storagecte as
                        (
                            select ps.ProductId,group_concat(s.Id) as StorageIds,group_concat(s.Name) as Storage from ProductStorages ps
                            left outer join Storages s on ps.StorageId = s.Id
                            group by ps.ProductId
                        )
                        select p.Id,p.Name, p.GujaratiName, p.PicturePath, cast(p.Size as real) Size, p.Description, p.UnitTypeCode,p.OrderBulkName,p.OrderBulkQuantity,p.ReorderLevel, p.IsDisposable, p.Company, p.Enabled, p.LastModifiedById, p.ModifiedAt,
                        s.StorageIds,s.Storage, u.UserName as [LastModifiedBy], ut.Name as [UnitTypeName], pi.Quantity as [CurrentStock] from Products p
                        inner join AspNetUsers u on p.LastModifiedById = u.Id
                        left outer join storagecte s on p.Id = s.ProductId
                        left outer join UnitTypes ut on p.UnitTypeCode = ut.Code
                        left outer join ProductInventory pi on p.Id = pi.ProductId", transaction: t);
        }

        public Task<bool> IsProductExist(string unitTypeCode)
        {
            return db.QuerySingleAsync<bool>(@"if EXISTS(select top 1 from Products where UnitTypeCode = @unitTypeCode
                                        BEGIN
                                        	select 1
                                        END
                                        ELSE
                                        BEGIN
                                        	select 0
                                        END", new { unitTypeCode }, transaction: t);
        }

        public Task RemoveProductStorages(int productId)
        {
            return db.ExecuteAsync("delete from ProductStorages where ProductId = @productId", new { productId }, transaction: t);
        }
    }
}
