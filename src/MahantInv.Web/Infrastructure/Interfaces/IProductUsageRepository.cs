using MahantInv.Web.Infrastructure.Entities;
using MahantInv.Web.Infrastructure.ViewModels;
using MahantInv.Web.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MahantInv.Web.Infrastructure.Interfaces
{
    public interface IProductUsageRepository : IAsyncRepository<ProductUsage>
    {
        Task<IEnumerable<ProductUsageVM>> GetProductUsages();
        Task<ProductUsageVM> GetProductUsageById(int id);
    }
}
