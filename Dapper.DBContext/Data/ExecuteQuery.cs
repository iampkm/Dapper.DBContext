using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.DBContext.Builder;
using System.Data;
using System.Diagnostics;
namespace Dapper.DBContext.Data
{
    public class ExecuteQuery : IExecuteQuery
    {
        IConnectionFactory _connectionFactory;
        IDbConnection _connection;
        public ExecuteQuery(IConnectionFactory connectionFactory)
        {
            this._connectionFactory = connectionFactory;
        }
        public T ExecuteScalar<T>(string sql, object param, int? timeout = null)
        {
            var result = default(T);
            try
            {
                this._connection = this._connectionFactory.CreateConnection();
                this._connection.Open();
                print(sql);
                result = this._connection.ExecuteScalar<T>(sql, param, commandTimeout: timeout);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this._connection.Close();
            }

            return result;
        }
        public Task<T> ExecuteScalarAsync<T>(string sql, object param, int? timeout = null)
        {
            this._connection = this._connectionFactory.CreateConnection();
            this._connection.Open();
            print(sql);
            var result = this._connection.ExecuteScalarAsync<T>(sql, param, commandTimeout: timeout);
            this._connection.Close();
            return result;
        }

        public IEnumerable<TEntity> Query<TEntity>(string sql, object param, int? timeout = null)
        {
            IEnumerable<TEntity> result = new List<TEntity>();
            try
            {
                this._connection = this._connectionFactory.CreateConnection();
                this._connection.Open();
                print(sql);
                result = this._connection.Query<TEntity>(sql, param, commandTimeout: timeout);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this._connection.Close();
            }
            return result;
        }

        public Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string sql, object param, int? timeout = null)
        {
            this._connection = this._connectionFactory.CreateConnection();
            this._connection.Open();
            print(sql);
            var result = this._connection.QueryAsync<TEntity>(sql, param, commandTimeout: timeout);
            this._connection.Close();
            return result;
        }

        public TEntity QuerySingle<TEntity>(string sql, object param, int? timeout = null)
        {
            TEntity result = default(TEntity);
            try
            {
                this._connection = this._connectionFactory.CreateConnection();
                this._connection.Open();
                print(sql);
                result = this._connection.Query<TEntity>(sql, param, commandTimeout: timeout).FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw new Exception("sql 异常", ex);
            }
            finally
            {
                this._connection.Close();
            }

            return result;
        }

        public Task<TEntity> QuerySingleAsync<TEntity>(string sql, object param, int? timeout = null)
        {
            this._connection = this._connectionFactory.CreateConnection();
            this._connection.Open();
            print(sql);
            var result = this._connection.QueryFirstOrDefaultAsync<TEntity>(sql, param, commandTimeout: timeout);
            this._connection.Close();
            return result;
        }

        private void print(string msg)
        {
            if (Debugger.IsAttached)
                Trace.WriteLine(msg);
        }
    }
}
