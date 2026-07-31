using System.Threading;
using System.Threading.Tasks;

namespace MahantInv.Web.Infrastructure.Interfaces
{
    public interface IUnitOfWork
    {
        Task BeginAsync(CancellationToken cancellationToken = default);
        Task CommitAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}
