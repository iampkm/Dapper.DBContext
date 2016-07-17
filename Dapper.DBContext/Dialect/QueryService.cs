using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace Dapper.DBContext.Dialect
{
    public class QueryService : IQuery
    {
        IDbConnection _connection;
        ISqlBuilder _builder;
        public QueryService(IDbConnection connection, ISqlBuilder builder)
        {
            this._connection = connection;
            this._builder = builder;
        }

        public bool Exists<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : IEntity
        {
            throw new NotImplementedException();
        }

        public TEntity Find<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : IEntity
        {
            object args = new object();
            string sql =string.Format("{0} {1}",this._builder.BuildSelect<TEntity>(), this._builder.BuildWhere<TEntity>(expression,out args)) ;
            this._connection.Open();
            var result = this._connection.Query<TEntity>(sql, args).FirstOrDefault();
            this._connection.Close();
            return result;

        }

        public IEnumerable<TEntity> Find<TEntity>(int[] Id) where TEntity : IEntity
        {
            string sql = string.Format("{0} where {1} in @{2}", this._builder.BuildSelect<TEntity>(),this._builder.GetKeyName(typeof(TEntity),true), this._builder.GetKeyName(typeof(TEntity), false));
            this._connection.Open();
            var result = this._connection.Query<TEntity>(sql, Id);
            this._connection.Close();
            return result;
        }

        public IEnumerable<TEntity> Find<TEntity>(string[] Id) where TEntity : IEntity
        {
            string sql = string.Format("{0} where {1} in @{2}", this._builder.BuildSelect<TEntity>(), this._builder.GetKeyName(typeof(TEntity), true), this._builder.GetKeyName(typeof(TEntity), false));
            this._connection.Open();
            var result = this._connection.Query<TEntity>(sql, Id);
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
            var result = this._connection.Query<TEntity>(sql,null);
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
