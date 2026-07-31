using MahantInv.Web.Infrastructure.Interfaces;
using System.Data.Common;

namespace MahantInv.Web.Infrastructure.Data
{
    public interface IDapperUnitOfWork : IUnitOfWork
    {
        DbConnection DbConnection { get; }
        DbTransaction DbTransaction { get; }
    }
}
