using MahantInv.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dapper;
using MahantInv.Infrastructure.Dtos;
using MahantInv.Infrastructure.Utility;

namespace MahantInv.Infrastructure.Data
{
    public class AdHocRepo : IAdHocRepo, IDisposable
    {
        private readonly DbConnection db;
        static Regex ddmlRegex = new("\"[^\"]*\"|'[^']*'|(\\b(insert|update|delete|create|alter|drop|begin|commit|rollback)\\b)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public AdHocRepo(DbConnection db)
        {
            this.db = db;
        }
        public void Dispose()
        {
            if (db != null)
            {
                // Closing a closed connection doesn't make a different so calling this method as a backup
                // in case the connection was kept open.
                db.Close();
                db.Dispose();
            }
        }


        public Task<IEnumerable<T>> QueryAsync<T>(string sql)
        {
            return db.QueryAsync<T>(sql);
        }

        public Task<T> QueryScalarAsync<T>(string sql)
        {
            return db.ExecuteScalarAsync<T>(sql);
        }

        public Task<IEnumerable<string>> GetSchemaAsync()
        {
            return db.QueryAsync<string>("select name from sqlite_master where type='table' order by name");
        }

        private async Task FillTableAsync(DataTable table, string sql)
        {
            try
            {
                if (IsSelectOnly(sql))
                {
                    using var dr = await db.ExecuteReaderAsync(sql);
                    //try { table.Load(dr); } catch (ConstraintException) { }
                    table.Load(dr);
                    table.TableName = $"{table.TableName}-{Guid.NewGuid()}";
                }
                else
                {
                    var affectedRows = await db.ExecuteAsync(sql);
                    table.Columns.Add("Outcome");
                    table.Rows.Add($"{affectedRows} row(s) affected.");
                }
            }
            catch (Exception ex)
            {
                //using var table = new DataTable();
                table.Reset();
                table.Columns.Add("Error");
                table.Rows.Add(ex.ToString());
            }
        }

        private bool IsSelectOnly(string sql)
        {
            //sql = sql.ToLower();
            //var dmlClauses = new string[] { "insert", "update", "delete", "create", "alter", "drop", "begin", "commit", "rollback" };
            //return !dmlClauses.Any(cl => IsClausePresent(sql, cl));
            return !ddmlRegex.Matches(sql).Any(m => m.Groups[1].Success);
        }

        //private bool IsClausePresent(string sql, string clause)
        //{
        //  var quotedSplit = sql.QuotedSplit(clause);
        //  return quotedSplit.Count() > 1;
        //}

        public async Task<DataSet> QueryAsync(string query)
        {
            if (query == null)
            {
                return null;
            }

            // this will allow running arbitrary queries without any constraint checks
            using DataSet ds = new() { EnforceConstraints = false };

            // initate a transaction, this will commit only if use provides a commit statement
            //await _uow.BeginAsync();

            var sqlList = query.QuotedSplit(";")
                .Where(s => !s.IsNullOrWhiteSpace());

            foreach (var sql in sqlList)
            {
                using var table = new DataTable();
                ds.Tables.Add(table);
                if (sql.Equals("commit", StringComparison.OrdinalIgnoreCase))
                {

                    //await _uow.CommitAsync();
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

            return await db.QueryAsync<dynamic>(query);
        }
    }
}
