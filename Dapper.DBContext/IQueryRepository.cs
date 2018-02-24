using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext
{
    public interface IQueryRepository<TEntity>
    {
        IQueryRepository<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
        IQueryRepository<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> keySelector);
        IQueryRepository<TEntity> OrderByDesc<TKey>(Expression<Func<TEntity, TKey>> keySelector);

        IQueryRepository<TResult> Select<TResult>(Expression<Func<TEntity, TResult>> selector);
               
        //IQueryRepository<TEntity> Limit(int pageSize = 10,int pageIndex = 0 );
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
         
        List<TEntity> ToList();
        IEnumerable<TEntity> ToEnumerable();
        Dictionary<TKey, TEntity> ToDictionary<TKey>(Func<TEntity, TKey> keySelector);
        int Count(Expression<Func<TEntity, bool>> predicate = null);
        bool Exists(Expression<Func<TEntity, bool>> predicate);


#region  异步查询方法

        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        List<TEntity> ToListAsync();
        Task<IEnumerable<TEntity>> ToEnumerableAsync();
        Dictionary<TKey, TEntity> ToDictionaryAsync<TKey>(Func<TEntity, TKey> keySelector);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
        bool ExistsAsync(Expression<Func<TEntity, bool>> predicate);
#endregion

        #region Dapper 原生查询方法

        IEnumerable<TEntity> Query(string sql, object param = null);
        TEntity QuerySingle(string sql, object param = null);

        #endregion

    }
}
