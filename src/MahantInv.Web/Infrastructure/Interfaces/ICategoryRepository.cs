using MahantInv.Web.Infrastructure.Dtos.Category;
using MahantInv.Web.Infrastructure.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MahantInv.Web.Infrastructure.Interfaces
{
    public interface ICategoryRepository : IAsyncRepository<Category>
    {
        Task<IEnumerable<CategoryDto>> SearchCategories(string query, int limit);

        /// <summary>
        /// Makes <paramref name="product"/>.ProductCategories match <paramref name="categoryNames"/>:
        /// existing categories are reused, unknown names are created, and links not in the list are removed.
        /// Changes are staged on the context only; the caller must call SaveChangesAsync.
        /// </summary>
        Task SyncProductCategories(Product product, IEnumerable<string> categoryNames);
    }
}
