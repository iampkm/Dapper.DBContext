using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext.Data
{
   public interface IDataBase
    {
        /// <summary>
        /// 立即执行，并返回所影响行数,无事务
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        int ExecuteSql(string sql, object param = null,int? commandTimeout = null);
        /// <summary>
        /// 立即执行，并返回所影响行数,无事务
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<int> ExecuteSqlAsync(string sql, object param = null, int? commandTimeout = null);
        /// <summary>
        /// 加入工作单元，Sql命令将在SaveChange 时执行
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        void AddExecute(string sql, object param = null);
        /// <summary>
        /// 执行，返回一行一列值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        T ExecuteScalar<T>(string sql, object param = null);
       
        Task<T> ExecuteScalarAsync<T>(string sql, object param);
        IEnumerable<TEntity> Query<TEntity>(string sql, object param);
        Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string sql, object param);
        TEntity QuerySingle<TEntity>(string sql, object param);
        Task<TEntity> QuerySingleAsync<TEntity>(string sql, object param);
    }
}
