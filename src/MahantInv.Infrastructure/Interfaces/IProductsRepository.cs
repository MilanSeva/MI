using MahantInv.Infrastructure.Entities;
using MahantInv.Infrastructure.ViewModels;
using MahantInv.SharedKernel.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.Interfaces
{
    public interface IProductsRepository : IAsyncRepository<Product>
    {
        Task<IEnumerable<ProductVM>> GetProducts();
        Task<ProductVM> GetProductById(int productId);
        Task<bool> IsProductExist(string unitTypeCode);
        Task RemoveProductStorages(int productId);
        Task AddProductStorage(ProductStorage productStorage);
    }
}
