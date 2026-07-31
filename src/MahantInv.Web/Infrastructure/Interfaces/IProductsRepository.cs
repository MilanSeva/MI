using MahantInv.Web.Infrastructure.Entities;
using MahantInv.Web.Infrastructure.ViewModels;
using MahantInv.Web.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MahantInv.Web.Infrastructure.Interfaces
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
