using System.Data.Common;

namespace MahantInv.Web.Infrastructure.Data
{
    public class BaseRepository
    {
        protected readonly IDapperUnitOfWork _uow;
        protected DbConnection db => _uow?.DbConnection;
        protected DbTransaction t => _uow?.DbTransaction;

        public BaseRepository(IDapperUnitOfWork uow)
        {
            _uow = uow;
        }
    }
}
