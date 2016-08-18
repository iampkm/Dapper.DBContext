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
        protected IExecuteQuery Select { get { return new ExecuteQuery(this._connectionFactory); } }

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
            var result = this._executeQuery.QuerySingle<TEntity>(sql, Id);
            return result;
        }

        public TEntity Find<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            object args = new object();
            string sql = this._builder.BuildSelectByLamda(expression, out args);
            var result = this._executeQuery.QuerySingle<TEntity>(sql, args);
            return result;
        }

        public IEnumerable<TEntity> Find<TEntity>(int[] Ids) where TEntity : class
        {
            return FindByIds<TEntity>(Ids);
        }

        public IEnumerable<TEntity> Find<TEntity>(string[] Ids) where TEntity : class
        {
            return FindByIds<TEntity>(Ids);
        }

        private IEnumerable<TEntity> FindByIds<TEntity>(object Ids) where TEntity : class
        {
            string sql = this._builder.buildSelectById<TEntity>(false);
            var result = this._executeQuery.Query<TEntity>(sql, Ids);
            return result;
        }

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

        public TResult Sum<TEntity, TResult>(Expression<Func<TEntity, TResult>> select, Expression<Func<TEntity, bool>> expression = null) where TEntity : class
        {
            object args = new object();
            string sql = this._builder.BuildSelectByLamda<TEntity,TResult>(expression, out args,select, "sum");
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


        public Task<TEntity> FindAsync<TEntity>(int Id) where TEntity : class
        {
            return FindSingleAsync<TEntity>(Id);
        }

        public Task<TEntity> FindAsync<TEntity>(string Id) where TEntity : class
        {
            return FindSingleAsync<TEntity>(Id);
        }
        private Task<TEntity> FindSingleAsync<TEntity>(object Id) where TEntity : class
        {
            string sql = this._builder.buildSelectById<TEntity>();
            var result = this._executeQuery.QuerySingleAsync<TEntity>(sql, Id);
            return result;
        }

        public Task<TEntity> FindAsync<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            object args = new object();
            string sql = this._builder.BuildSelectByLamda(expression, out args);
            var result = this._executeQuery.QuerySingleAsync<TEntity>(sql, args);
            return result;
        }

        public Task<IEnumerable<TEntity>> FindAsync<TEntity>(string[] Ids) where TEntity : class
        {
            return FindByIdsAsync<TEntity>(Ids);
        }

        public Task<IEnumerable<TEntity>> FindAsync<TEntity>(int[] Ids) where TEntity : class
        {
            return FindByIdsAsync<TEntity>(Ids);
        }

        private Task<IEnumerable<TEntity>> FindByIdsAsync<TEntity>(object Ids) where TEntity : class
        {
            string sql = this._builder.buildSelectById<TEntity>(false);
            var result = this._executeQuery.QueryAsync<TEntity>(sql, Ids);
            return result;
        }

        public Task<IEnumerable<TEntity>> FindAllAsync<TEntity>() where TEntity : class
        {
            string sql = this._builder.buildSelect<TEntity>();
            var result = this._executeQuery.QueryAsync<TEntity>(sql, null);
            return result;
        }

        public Task<IEnumerable<TEntity>> FindAllAsync<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            object args = new object();
            string sql = this._builder.BuildSelectByLamda(expression, out args);
            var result = this._executeQuery.QueryAsync<TEntity>(sql, args);
            return result;
        }

        public bool ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            object args = new object();
            string sql = this._builder.BuildSelectByLamda<TEntity>(expression, out args, "count(*)");
            var result = this._executeQuery.ExecuteScalarAsync<int>(sql, args);
            return result.Result > 0;
        }

        public Task<int> CountAsync<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            object args = new object();
            string sql = this._builder.BuildSelectByLamda<TEntity>(expression, out args, "count(*)");
            var result = this._executeQuery.ExecuteScalarAsync<int>(sql, args);
            return result;
        }

        public Task<TResult> SumAsync<TEntity, TResult>(Expression<Func<TEntity, TResult>> select, Expression<Func<TEntity, bool>> expression = null) where TEntity : class
        {
            object args = new object();
            string sql = this._builder.BuildSelectByLamda<TEntity, TResult>(expression, out args, select, "sum");
            var result = this._executeQuery.ExecuteScalarAsync<TResult>(sql, args);
            return result;
        }
   
    }
}
