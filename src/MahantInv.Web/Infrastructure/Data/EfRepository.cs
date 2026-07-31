using MahantInv.Web.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MahantInv.Web.Infrastructure.Data
{
    public class EfRepository<T> : IReadOnlyRepository<T>, IAsyncRepository<T> where T : class, IAggregateRoot
    {
        protected readonly MIDbContext _context;

        public EfRepository(MIDbContext context)
        {
            _context = context;
        }

        public virtual async Task<int> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _context.Set<T>().AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return GetPrimaryKeyIntValue(entity);
        }

        public virtual async Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public virtual async Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public virtual async Task<bool> DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            _context.Set<T>().RemoveRange(entities);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public virtual Task<T> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            return _context.Set<T>().FindAsync(new object[] { id }, cancellationToken).AsTask();
        }

        public virtual Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return _context.Set<T>().CountAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> ListAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().ToListAsync(cancellationToken);
        }

        public virtual Task<IEnumerable<T>> ListAllAsync(bool isActive, CancellationToken cancellationToken = default)
        {
            // No entity in this codebase has an IsActive column and nothing calls this overload;
            // kept only for interface compatibility, aliased to ListAllAsync().
            return ListAllAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> ListRangeAsync(IEnumerable<object> ids, CancellationToken cancellationToken = default)
        {
            var keyName = PrimaryKeyName();
            var idList = ids.Select(id => (int)id).ToList();
            return await _context.Set<T>()
                .Where(e => idList.Contains(EF.Property<int>(e, keyName)))
                .ToListAsync(cancellationToken);
        }

        private string PrimaryKeyName()
        {
            return _context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties[0].Name;
        }

        private int GetPrimaryKeyIntValue(T entity)
        {
            var value = _context.Entry(entity).Property(PrimaryKeyName()).CurrentValue;
            return value is int intValue ? intValue : 0;
        }
    }
}
