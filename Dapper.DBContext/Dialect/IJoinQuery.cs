using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace Dapper.DBContext.Dialect
{
   public interface IJoinQuery
    {
        IJoinQuery BuildJoin<TEntity>();
        IJoinQuery BuildPage<TEntity>(int pageIndex, int pageSize);
       IJoinQuery InnerJoin<TEntity>();
       IJoinQuery LeftJoin<TEntity>();
       IJoinQuery RightJoin<TEntity>();
       IEnumerable<TResult> Where<TEntity, TResult>(Expression<Func<TEntity, bool>> expression);
       IEnumerable<TResult> Where<TEntity1, TEntity2, TResult>(Expression<Func<TEntity1, TEntity2, bool>> expression);
       IEnumerable<TResult> Where<TEntity1, TEntity2, TEntity3, TResult>(Expression<Func<TEntity1, TEntity2, TEntity3, bool>> expression);
    }
}
