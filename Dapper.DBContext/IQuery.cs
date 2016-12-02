using Dapper.DBContext.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext
{
    public interface IQuery
    {
        TEntity Find<TEntity>(int Id) where TEntity : class;
        TEntity Find<TEntity>(string Id) where TEntity : class;
        TEntity Find<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class;
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        TEntity Find<TEntity>(string sql, object param) where TEntity : class;
        IEnumerable<TEntity> Find<TEntity>(int[] Ids) where TEntity : class;
        IEnumerable<TEntity> Find<TEntity>(string[] Ids) where TEntity : class;
        IEnumerable<TEntity> FindAll<TEntity>() where TEntity : class;
        IEnumerable<TEntity> FindAll<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class;
        IEnumerable<TEntity> FindAll<TEntity>(string sql, object param) where TEntity : class;
        bool Exists<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class;
        int Count<TEntity>(Expression<Func<TEntity, bool>> expression = null) where TEntity : class;
        int Count<TEntity>(string where, object param) where TEntity : class;
        TResult Sum<TEntity, TResult>(Expression<Func<TEntity, TResult>> select, Expression<Func<TEntity, bool>> expression = null) where TEntity : class;
        /// <summary>
        /// 查询，返回一行一列值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        TResult FindScalar<TResult>(string sql, object param);

        /// <summary>
        /// join query
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        IJoinQuery FindJoin<TEntity>() where TEntity : class;
        /// <summary>
        ///  find by page
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IJoinQuery FindPage<TEntity>(int pageIndex, int pageSize) where TEntity : class;
        Task<TEntity> FindAsync<TEntity>(int Id) where TEntity : class;
        Task<TEntity> FindAsync<TEntity>(string Id) where TEntity : class;
        Task<TEntity> FindAsync<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class;
        Task<TEntity> FindAsync<TEntity>(string sql, object param) where TEntity : class;
        Task<IEnumerable<TEntity>> FindAsync<TEntity>(string[] Ids) where TEntity : class;
        Task<IEnumerable<TEntity>> FindAsync<TEntity>(int[] Ids) where TEntity : class;
        Task<IEnumerable<TEntity>> FindAllAsync<TEntity>() where TEntity : class;
        Task<IEnumerable<TEntity>> FindAllAsync<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class;
        Task<IEnumerable<TEntity>> FindAllAsync<TEntity>(string sql, object param) where TEntity : class;
        bool ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class;
        Task<int> CountAsync<TEntity>(Expression<Func<TEntity, bool>> expression = null) where TEntity : class;
        Task<int> CountAsync<TEntity>(string where, object param) where TEntity : class;
        Task<TResult> SumAsync<TEntity, TResult>(Expression<Func<TEntity, TResult>> select, Expression<Func<TEntity, bool>> expression = null) where TEntity : class;


    }
}
