using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Dapper.DBContext.Dialect;
using Dapper.DBContext.Transaction;
namespace Dapper.DBContext
{
    public class QueryService : IQuery
    {
        ISqlBuilder _builder;
        IConnectionFactory _connectionFactory;
        IExecuteQuery _executeQuery;
        IJoinQuery _joinQuery;
        public QueryService(string connectionStringName)
        {
            this._connectionFactory = IConnectionFactory.Create(connectionStringName);
            this._executeQuery = new ExecuteQuery(this._connectionFactory);
            this._builder = this._connectionFactory.CreateBuilder();
            this._joinQuery = this._connectionFactory.CreateJoinBuilder();
        }
        public QueryService(IConnectionFactory connectionFactory)
        {
            this._executeQuery = new ExecuteQuery(connectionFactory);
            this._builder = this._connectionFactory.CreateBuilder();
        }
        /// <summary>
        /// Dapper Connection. When you use it ,please first open it. When you finish, close it.
        /// </summary>
        protected IDbConnection DBConnection { get { return this._connectionFactory.CreateConnection(); } }

        public bool Exists<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : IEntity
        {
            object args = new object();
            string sql = string.Format("{0} {1}", this._builder.BuildSelect<TEntity>("count(*)"), this._builder.BuildWhere<TEntity>(expression, out args));
            var result = this._executeQuery.ExecuteScalar<int>(sql, args);
            return result > 0;
        }

        public TEntity Find<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : IEntity
        {
            object args = new object();
            string sql = string.Format("{0} {1}", this._builder.BuildSelect<TEntity>(), this._builder.BuildWhere<TEntity>(expression, out args));
            var result = this._executeQuery.QuerySingle<TEntity>(sql, args);
            return result;

        }

        public IEnumerable<TEntity> Find<TEntity>(int[] Ids) where TEntity : IEntity
        {
            string sql = string.Format("{0} where {1} in @{2}", this._builder.BuildSelect<TEntity>(), this._builder.GetKeyName(typeof(TEntity), true), this._builder.GetKeyName(typeof(TEntity), false));
            var result = this._executeQuery.Query<TEntity>(sql, Ids);
            return result;
        }

        public IEnumerable<TEntity> Find<TEntity>(string[] Ids) where TEntity : IEntity
        {
            string sql = string.Format("{0} where {1} in @{2}", this._builder.BuildSelect<TEntity>(), this._builder.GetKeyName(typeof(TEntity), true), this._builder.GetKeyName(typeof(TEntity), false));
            var result = this._executeQuery.Query<TEntity>(sql, Ids);
            return result;
        }

        public TEntity Find<TEntity>(string Id) where TEntity : IEntity
        {
            string sql = string.Format("{0} where {1} in @{2}", this._builder.BuildSelect<TEntity>(), this._builder.GetKeyName(typeof(TEntity), true), this._builder.GetKeyName(typeof(TEntity), false));
            var result = this._executeQuery.QuerySingle<TEntity>(sql, Id);
            return result;
        }

        public TEntity Find<TEntity>(int Id) where TEntity : IEntity
        {
            string sql = string.Format("{0} where {1} in @{2}", this._builder.BuildSelect<TEntity>(), this._builder.GetKeyName(typeof(TEntity), true), this._builder.GetKeyName(typeof(TEntity), false));
            var result = this._executeQuery.QuerySingle<TEntity>(sql, Id);
            return result;
        }

        public IEnumerable<TEntity> FindAll<TEntity>() where TEntity : IEntity
        {
            string sql = this._builder.BuildSelect<TEntity>();
            var result = this._executeQuery.Query<TEntity>(sql, null);
            return result;
        }

        public IEnumerable<TEntity> FindAll<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : IEntity
        {
            object args = new object();
            string sql = string.Format("{0} {1}", this._builder.BuildSelect<TEntity>(), this._builder.BuildWhere<TEntity>(expression, out args));
            var result = this._executeQuery.Query<TEntity>(sql, args);
            return result;
        }


        public IJoinQuery FindJoin<TEntity>() where TEntity : IEntity
        {
            var entityType = typeof(TEntity);
            this._joinQuery.JoinContext.Add(entityType);
            return this._joinQuery;
        }

        public IJoinQuery FindPage<TEntity>(int pageIndex, int pageSize) where TEntity : IEntity
        {
            var entityType = typeof(TEntity);
            this._joinQuery.JoinContext.SetPageInfo(pageIndex, pageSize);
            this._joinQuery.JoinContext.Add(entityType);
            return this._joinQuery;
        }
    }
}
