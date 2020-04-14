using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.DBContext.Builder;
using System.Linq.Expressions;
using Dapper.DBContext.Data;
namespace Dapper.DBContext
{
   public class QueryService<TEntity>:IQuery<TEntity>
    {
       QueryContext _queryContext;
       IConnectionFactory _connectionFactory;
       IExecuteQuery _executeQuery;
       ISqlBuilder _builder; 

       public QueryService(string connectionStringName)
       {
           this._connectionFactory = IConnectionFactory.Create(connectionStringName);
           this._executeQuery = new ExecuteQuery(this._connectionFactory);
           this._builder = this._connectionFactory.CreateBuilder();
           _queryContext = new QueryContext(_builder, _executeQuery);
           _queryContext.EntityType = typeof(TEntity);
       }

       private QueryService(QueryContext context)
       {
           this._queryContext = context;
           this._builder = context.Builder;
           this._executeQuery = context.Query;
       }

        public IQuery<TEntity> Where(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {
            _queryContext.Nodes.Add(new QueryNode(QueryNodeType.Where, predicate) );
            return this;
        }

        public IQuery<TEntity> OrderBy<TKey>(System.Linq.Expressions.Expression<Func<TEntity, TKey>> keySelector)
        {
            _queryContext.Nodes.Add(new QueryNode(QueryNodeType.OrderBy, keySelector));
            return this;
        }

        public IQuery<TEntity> OrderByDesc<TKey>(System.Linq.Expressions.Expression<Func<TEntity, TKey>> keySelector)
        {
            _queryContext.Nodes.Add(new QueryNode(QueryNodeType.OrderByDesc, keySelector));
            return this;
        }

        public IQuery<TResult> Select<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            // 只允许使用一次select 
            if (_queryContext.Nodes.Exists(n => n.NodeType == QueryNodeType.Select)) throw new Exception("The select method can only be used once ");
            _queryContext.Nodes.Add(new QueryNode(QueryNodeType.Select, selector));
            return new QueryService<TResult>(_queryContext);
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            object args = new object();
            string sql = this._builder.BuildSelectByLamda(predicate, out args);
            var result = this._executeQuery.QuerySingle<TEntity>(sql, args);
            _queryContext.Clear();
            return result;
        }

        public List<TEntity> ToList()
        {
            return this.ToEnumerable().ToList();
        }

        public IEnumerable<TEntity> ToEnumerable()
        {
            object args = new object();
            string sql = this._builder.BuildSelectByContext<TEntity>(this._queryContext, out args);
            var result = this._executeQuery.Query<TEntity>(sql, args);
            _queryContext.Clear();
            return result;
        }

        public Dictionary<TKey, TEntity> ToDictionary<TKey>(Func<TEntity, TKey> keySelector)
        {
            return this.ToEnumerable().ToDictionary(keySelector);
        }

        public int Count(Expression<Func<TEntity, bool>> predicate = null)
        {
            object args = new object();
            string sql = this._builder.BuildSelectByLamda<TEntity>(predicate, out args, "count(*)");
            var result = this._executeQuery.ExecuteScalar<int>(sql, args);
            _queryContext.Clear();
            return result;
        }

        public bool Exists(Expression<Func<TEntity, bool>> predicate)
        {
            return Count(predicate) > 0;
        }

        public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            object args = new object();
            string sql = this._builder.BuildSelectByLamda(predicate, out args);
            var result = this._executeQuery.QuerySingleAsync<TEntity>(sql, args);
            _queryContext.Clear();
            return result;
        }

        public List<TEntity> ToListAsync()
        {
            return ToEnumerableAsync().Result.ToList();
        }

        public Task<IEnumerable<TEntity>> ToEnumerableAsync()
        {
            object args = new object();
            string sql = this._builder.BuildSelectByContext<TEntity>(this._queryContext, out args);
            var result = this._executeQuery.QueryAsync<TEntity>(sql, args);
            _queryContext.Clear();
            return result;
        }

        public Dictionary<TKey, TEntity> ToDictionaryAsync<TKey>(Func<TEntity, TKey> keySelector)
        {
            return ToEnumerableAsync().Result.ToDictionary(keySelector);
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            object args = new object();
            string sql = this._builder.BuildSelectByLamda<TEntity>(predicate, out args, "count(*)");
            var result = this._executeQuery.ExecuteScalarAsync<int>(sql, args);
            _queryContext.Clear();
            return result;
        }

        public bool ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return CountAsync(predicate).Result > 0;
        }

        public IEnumerable<TEntity> Query(string sql, object param = null)
        {
            return this._executeQuery.Query<TEntity>(sql, param);
        }

        public TEntity QuerySingle(string sql, object param = null)
        {
            return this._executeQuery.QuerySingle<TEntity>(sql, param);
        }

      
    }
}
