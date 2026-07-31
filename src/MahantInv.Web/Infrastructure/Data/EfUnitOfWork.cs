using MahantInv.Web.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MahantInv.Web.Infrastructure.Data
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly MIDbContext _context;
        private IDbContextTransaction _transaction;

        public EfUnitOfWork(MIDbContext context)
        {
            _context = context;
        }

        public async Task BeginAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction is not null)
                throw new InvalidOperationException("Previous transaction is in progress.");

            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction is null)
                throw new InvalidOperationException("No transaction is in progress.");

            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction is null)
                throw new InvalidOperationException("No transaction is in progress.");

            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}
