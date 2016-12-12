using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Dapper.DBContext.Builder;
using Dapper.DBContext.Data;
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
        /// <summary>
        /// Common Query method,execute sql 
        /// </summary>
        public IExecuteQuery Context { get { return this._executeQuery; } }

        public TEntity Find<TEntity>(int Id) where TEntity : class
        {
            return FindById<TEntity>(Id);
        }
        public TEntity Find<TEntity>(string Id) where TEntity : class
        {
            return FindById<TEntity>(Id);
        }

        private TEntity FindById<TEntity>(object Id) where TEntity : class
        {
            string sql = this._builder.buildSelectById<TEntity>();
            var result = this._executeQuery.QuerySingle<TEntity>(sql, new { Id = Id });
            return result;
        }

        public TEntity Find<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            object args = new object();
            string sql = this._builder.BuildSelectByLamda(expression, out args);
            var result = this._executeQuery.QuerySingle<TEntity>(sql, args);
            return result;
        }

        public TEntity Find<TEntity>(string sql, object param) where TEntity : class
        {
            return this._executeQuery.QuerySingle<TEntity>(sql, param);
        }

        public IEnumerable<TEntity> Find<TEntity>(int[] Ids) where TEntity : class
        {
           // return FindByIds<TEntity>(Ids);
            string sql = this._builder.buildSelectById<TEntity>(false);
            var result = this._executeQuery.Query<TEntity>(sql, new { Id = Ids });
            return result;
        }

        public IEnumerable<TEntity> Find<TEntity>(string[] Ids) where TEntity : class
        {
           // return FindByIds<TEntity>(Ids);
            string sql = this._builder.buildSelectById<TEntity>(false);
            var result = this._executeQuery.Query<TEntity>(sql, new { Id = Ids });
            return result;
        }

        //private IEnumerable<TEntity> FindByIds<TEntity>(object Ids) where TEntity : class
        //{
        //    string sql = this._builder.buildSelectById<TEntity>(false);
        //    var result = this._executeQuery.Query<TEntity>(sql, new { Id = Ids });
        //    return result;
        //}

        public IEnumerable<TEntity> FindAll<TEntity>() where TEntity : class
        {
            string sql = this._builder.buildSelect<TEntity>();
            var result = this._executeQuery.Query<TEntity>(sql, null);
            return result;
        }

        public IEnumerable<TEntity> FindAll<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            object args = new object();
            string sql = this._builder.BuildSelectByLamda(expression, out args);
            var result = this._executeQuery.Query<TEntity>(sql, args);
            return result;
        }

        public IEnumerable<TEntity> FindAll<TEntity>(string sql, object param) where TEntity : class
        {
            return this._executeQuery.Query<TEntity>(sql, param);
        }
        public bool Exists<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            object args = new object();
            string sql = this._builder.BuildSelectByLamda<TEntity>(expression, out args, "count(*)");
            var result = this._executeQuery.ExecuteScalar<int>(sql, args);
            return result > 0;
        }
        public int Count<TEntity>(Expression<Func<TEntity, bool>> expression = null) where TEntity : class
        {
            object args = new object();
            string sql = this._builder.BuildSelectByLamda<TEntity>(expression, out args, "count(*)");
            var result = this._executeQuery.ExecuteScalar<int>(sql, args);
            return result;
        }
        public int Count<TEntity>(string where, object param) where TEntity : class
        {
            if (string.IsNullOrEmpty(where)) { return Count<TEntity>(); }
            string sql = string.Format("{0} where 1=1 {1}", _builder.buildSelect<TEntity>("count(*)"), where);
            var result = this._executeQuery.ExecuteScalar<int>(sql, param);
            return result;
        }
        public TResult Sum<TEntity, TResult>(Expression<Func<TEntity, TResult>> select, Expression<Func<TEntity, bool>> expression = null) where TEntity : class
        {
            object args = new object();
            string sql = this._builder.BuildSelectByLamda<TEntity, TResult>(expression, out args, select, "sum");
            var result = this._executeQuery.ExecuteScalar<TResult>(sql, args);
            return result;
        }


        public IJoinQuery FindJoin<TEntity>() where TEntity : class
        {
            var entityType = typeof(TEntity);
            this._joinQuery.JoinContext.Add(entityType);
            return this._joinQuery;
        }

        public IJoinQuery FindPage<TEntity>(int pageIndex, int pageSize) where TEntity : class
        {
            var entityType = typeof(TEntity);
            this._joinQuery.JoinContext.SetPageInfo(pageIndex, pageSize);
            this._joinQuery.JoinContext.Add(entityType);
            return this._joinQuery;
        }
    }
}
