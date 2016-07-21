using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Dapper.DBContext.Dialect;
namespace Dapper.DBContext
{
    public class QueryService : IQuery
    {
        IDbConnection _connection;
        ISqlBuilder _builder;
        IConnectionFactory _connectionFactory;
        public QueryService(string connectionStringName)
        {
            this._connectionFactory = new ConnectionFactory(connectionStringName);
            this._connection = this._connectionFactory.Create();
            this._builder = this._connectionFactory.Builder; ;
        }
        public QueryService(IConnectionFactory connectionFactory, ISqlBuilder builder)
        {
            this._connection = connectionFactory.Create();
            this._builder = builder;
        }
        /// <summary>
        /// Dapper Connection. When you use it ,please first open it. When you finish, close it.
        /// </summary>
        protected IDbConnection DBConnection { get { return _connection; } }

        public bool Exists<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : IEntity
        {
            object args = new object();
            string sql = string.Format("{0} {1}", this._builder.BuildSelect<TEntity>("count(*)"), this._builder.BuildWhere<TEntity>(expression, out args));
            this._connection.Open();
            var result = this._connection.ExecuteScalar<int>(sql, args);
            this._connection.Close();
            return result > 0;
        }

        public TEntity Find<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : IEntity
        {
            object args = new object();
            string sql = string.Format("{0} {1}", this._builder.BuildSelect<TEntity>(), this._builder.BuildWhere<TEntity>(expression, out args));
            this._connection.Open();
            var result = this._connection.Query<TEntity>(sql, args).FirstOrDefault();
            this._connection.Close();
            return result;

        }

        public IEnumerable<TEntity> Find<TEntity>(int[] Ids) where TEntity : IEntity
        {
            string sql = string.Format("{0} where {1} in @{2}", this._builder.BuildSelect<TEntity>(), this._builder.GetKeyName(typeof(TEntity), true), this._builder.GetKeyName(typeof(TEntity), false));
            this._connection.Open();
            var result = this._connection.Query<TEntity>(sql, Ids);
            this._connection.Close();
            return result;
        }

        public IEnumerable<TEntity> Find<TEntity>(string[] Ids) where TEntity : IEntity
        {
            string sql = string.Format("{0} where {1} in @{2}", this._builder.BuildSelect<TEntity>(), this._builder.GetKeyName(typeof(TEntity), true), this._builder.GetKeyName(typeof(TEntity), false));
            this._connection.Open();
            var result = this._connection.Query<TEntity>(sql, Ids);
            this._connection.Close();
            return result;
        }

        public TEntity Find<TEntity>(string Id) where TEntity : IEntity
        {
            string sql = string.Format("{0} where {1} in @{2}", this._builder.BuildSelect<TEntity>(), this._builder.GetKeyName(typeof(TEntity), true), this._builder.GetKeyName(typeof(TEntity), false));
            this._connection.Open();
            var result = this._connection.Query<TEntity>(sql, Id).FirstOrDefault();
            this._connection.Close();
            return result;
        }

        public TEntity Find<TEntity>(int Id) where TEntity : IEntity
        {
            string sql = string.Format("{0} where {1} in @{2}", this._builder.BuildSelect<TEntity>(), this._builder.GetKeyName(typeof(TEntity), true), this._builder.GetKeyName(typeof(TEntity), false));
            this._connection.Open();
            var result = this._connection.Query<TEntity>(sql, Id).FirstOrDefault();
            this._connection.Close();
            return result;
        }

        public IEnumerable<TEntity> FindAll<TEntity>() where TEntity : IEntity
        {
            string sql = this._builder.BuildSelect<TEntity>();
            this._connection.Open();
            var result = this._connection.Query<TEntity>(sql, null);
            this._connection.Close();
            return result;
        }

        public IEnumerable<TEntity> FindAll<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : IEntity
        {
            object args = new object();
            string sql = string.Format("{0} {1}", this._builder.BuildSelect<TEntity>(), this._builder.BuildWhere<TEntity>(expression, out args));
            this._connection.Open();
            var result = this._connection.Query<TEntity>(sql, args);
            this._connection.Close();
            return result;
        }
    }
}
