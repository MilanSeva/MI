using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.Interfaces
{
    public interface IAdHocRepo
    {
        Task<IEnumerable<string>> GetSchemaAsync();
        Task<DataSet> QueryAsync(string query);
        Task<IEnumerable<dynamic>> QueryObjectAsync(string query);
        Task<T> QueryScalarAsync<T>(string sql);
        Task<IEnumerable<T>> QueryAsync<T>(string sql);
    }
}
