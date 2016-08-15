using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.DBContext.Builder;
using System.Data;
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
        public T ExecuteScalar<T>(string sql, object param)
        {
            this._connection = this._connectionFactory.CreateConnection();
            this._connection.Open();
            var result = this._connection.ExecuteScalar<T>(sql, param);
            this._connection.Close();
            return result;
        }
        public Task<T> ExecuteScalarAsync<T>(string sql, object param)
        {
            this._connection = this._connectionFactory.CreateConnection();
            this._connection.Open();
            var result = this._connection.ExecuteScalarAsync<T>(sql, param);
            this._connection.Close();
            return result;
        }

        public IEnumerable<TEntity> Query<TEntity>(string sql, object param)
        {
            this._connection = this._connectionFactory.CreateConnection();
            this._connection.Open();
            var result = this._connection.Query<TEntity>(sql, param);
            this._connection.Close();
            return result;
        }

        public Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string sql, object param)
        {
            this._connection = this._connectionFactory.CreateConnection();
            this._connection.Open();
            var result = this._connection.QueryAsync<TEntity>(sql, param);
            this._connection.Close();
            return result;
        }

        public TEntity QuerySingle<TEntity>(string sql, object param)
        {
            this._connection = this._connectionFactory.CreateConnection();
            this._connection.Open();
            var result = this._connection.QueryFirst<TEntity>(sql, param);
            this._connection.Close();
            return result;
        }

        public Task<TEntity> QuerySingleAsync<TEntity>(string sql, object param)
        {
            this._connection = this._connectionFactory.CreateConnection();
            this._connection.Open();
            var result = this._connection.QueryFirstAsync<TEntity>(sql, param);
            this._connection.Close();
            return result;
        }
    }
}
