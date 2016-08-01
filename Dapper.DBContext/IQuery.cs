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
        IEnumerable<TEntity> Find<TEntity>(string[] Ids) where TEntity : class;
        IEnumerable<TEntity> Find<TEntity>(int[] Ids) where TEntity : class;
        IEnumerable<TEntity> FindAll<TEntity>() where TEntity : class;
        IEnumerable<TEntity> FindAll<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class;
        bool Exists<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class;
        int Count<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class;
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
        IJoinQuery FindPage<TEntity>(int pageIndex , int pageSize) where TEntity : class;
    }
}
