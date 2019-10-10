using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext.Data
{
    public class DataBaseService : IDataBase
    {
        IConnectionFactory _connectionFactory;
        IDbConnection _connection;
        IUnitOfWork _uow;
        public DataBaseService(IConnectionFactory connectionFactory, IUnitOfWork uow)
        {
            this._connectionFactory = connectionFactory;
            this._uow = uow;
        }

        public int ExecuteSql(string sql, object param = null, int? timeout = null)
        {
            using (this._connection = this._connectionFactory.CreateConnection())
            {
                this._connection.Open();
                print(sql);
                var result = this._connection.Execute(sql, param, null, timeout);
                return result;
            }
        }
        public Task<int> ExecuteSqlAsync(string sql, object param = null, int? timeout = null)
        {
            using (this._connection = this._connectionFactory.CreateConnection())
            {
                this._connection.Open();
                print(sql);
                var result = this._connection.ExecuteAsync(sql, param, null, timeout);
                return result;
            }
        }
        public T ExecuteScalar<T>(string sql, object param, int? timeout = null)
        {
            using (this._connection = this._connectionFactory.CreateConnection())
            {
                this._connection.Open();
                print(sql);
                var result = this._connection.ExecuteScalar<T>(sql, param, commandTimeout: timeout);
                return result;
            }
        }

        private void print(string msg)
        {
            if (Debugger.IsAttached)
                Trace.WriteLine(msg);
        }


        public void AddExecute(string sql, object param = null)
        {
            this._uow.Add(sql, param);
        }

        public Task<T> ExecuteScalarAsync<T>(string sql, object param, int? timeout = null)
        {
            using (this._connection = this._connectionFactory.CreateConnection())
            {
                this._connection.Open();
                print(sql);
                var result = this._connection.ExecuteScalarAsync<T>(sql, param, commandTimeout: timeout);
                return result;
            }
        }

        public IEnumerable<TEntity> Query<TEntity>(string sql, object param, int? timeout = null)
        {

            using (this._connection = this._connectionFactory.CreateConnection())
            {
                this._connection.Open();
                print(sql);
                var result = this._connection.Query<TEntity>(sql, param, commandTimeout: timeout);
                return result;
            }
        }

        public Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string sql, object param, int? timeout = null)
        {

            using (this._connection = this._connectionFactory.CreateConnection())
            {
                this._connection.Open();
                print(sql);
                var result = this._connection.QueryAsync<TEntity>(sql, param, commandTimeout: timeout);
                return result;
            }
        }

        public TEntity QuerySingle<TEntity>(string sql, object param, int? timeout = null)
        {
            using (this._connection = this._connectionFactory.CreateConnection())
            {
                this._connection.Open();
                print(sql);
                var result = this._connection.Query<TEntity>(sql, param, commandTimeout: timeout).FirstOrDefault();
                return result;
            }
        }

        public Task<TEntity> QuerySingleAsync<TEntity>(string sql, object param, int? timeout = null)
        {
            using (this._connection = this._connectionFactory.CreateConnection())
            {
                this._connection.Open();
                print(sql);
                var result = this._connection.QueryFirstOrDefaultAsync<TEntity>(sql, param, commandTimeout: timeout);
                return result;
            }
        }
    }
}
