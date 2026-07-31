using MahantInv.Web.Infrastructure.Dtos;
using MahantInv.Web.Infrastructure.Interfaces;
using MahantInv.Web.Infrastructure.Utility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MahantInv.Web.Infrastructure.Data
{
    public class AdHocRepo : IAdHocRepo
    {
        private readonly MIDbContext _context;
        static readonly Regex ddmlRegex = new("\"[^\"]*\"|'[^']*'|(\\b(insert|update|delete|create|alter|drop|begin|commit|rollback)\\b)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public AdHocRepo(MIDbContext context)
        {
            _context = context;
        }

        private async Task<DbConnection> GetOpenConnectionAsync()
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }
            return connection;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql)
        {
            var connection = await GetOpenConnectionAsync();
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            var results = new List<T>();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(reader.IsDBNull(0) ? default : (T)Convert.ChangeType(reader.GetValue(0), typeof(T)));
            }
            return results;
        }

        public async Task<T> QueryScalarAsync<T>(string sql)
        {
            var connection = await GetOpenConnectionAsync();
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            var result = await command.ExecuteScalarAsync();
            return result is null or DBNull ? default : (T)Convert.ChangeType(result, typeof(T));
        }

        public Task<IEnumerable<string>> GetSchemaAsync()
        {
            return QueryAsync<string>("select name from sqlite_master where type='table' order by name");
        }

        private async Task FillTableAsync(DataTable table, string sql)
        {
            try
            {
                var connection = await GetOpenConnectionAsync();
                if (IsSelectOnly(sql))
                {
                    using var command = connection.CreateCommand();
                    command.CommandText = sql;
                    using var dr = await command.ExecuteReaderAsync();
                    table.Load(dr);
                    table.TableName = $"{table.TableName}-{Guid.NewGuid()}";
                }
                else
                {
                    using var command = connection.CreateCommand();
                    command.CommandText = sql;
                    var affectedRows = await command.ExecuteNonQueryAsync();
                    table.Columns.Add("Outcome");
                    table.Rows.Add($"{affectedRows} row(s) affected.");
                }
            }
            catch (Exception ex)
            {
                table.Reset();
                table.Columns.Add("Error");
                table.Rows.Add(ex.ToString());
            }
        }

        private bool IsSelectOnly(string sql)
        {
            return !ddmlRegex.Matches(sql).Any(m => m.Groups[1].Success);
        }

        public async Task<DataSet> QueryAsync(string query)
        {
            if (query == null)
            {
                return null;
            }

            // this will allow running arbitrary queries without any constraint checks
            using DataSet ds = new() { EnforceConstraints = false };

            var sqlList = query.QuotedSplit(";")
                .Where(s => !s.IsNullOrWhiteSpace());

            foreach (var sql in sqlList)
            {
                using var table = new DataTable();
                ds.Tables.Add(table);
                if (sql.Equals("commit", StringComparison.OrdinalIgnoreCase))
                {
                    // no-op: matches historical behavior where a bare "commit" statement was swallowed
                }
                else
                {
                    await this.FillTableAsync(table, sql);
                }
            }

            return ds;
        }

        public async Task<IEnumerable<dynamic>> QueryObjectAsync(string query)
        {
            if (query == null)
            {
                return new List<ValidationError>
                {
                    new ValidationError
                    {
                        Key = "SQL",
                        ErrorMessage = "Try a vaild SQL."
                    }
                }.AsEnumerable<dynamic>();
            }

            var connection = await GetOpenConnectionAsync();
            using var command = connection.CreateCommand();
            command.CommandText = query;
            var results = new List<dynamic>();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                IDictionary<string, object> row = new ExpandoObject();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                }
                results.Add(row);
            }
            return results;
        }
    }
}
