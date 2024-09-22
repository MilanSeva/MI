using MahantInv.SharedKernel.Interfaces;
using System.Data.Common;

namespace MahantInv.Infrastructure.Data
{
    public interface IDapperUnitOfWork : IUnitOfWork
    {
        DbConnection DbConnection { get; }
        DbTransaction DbTransaction { get; }
    }
}
