using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext.Dialect
{
   public interface IJoinQuery
    {
       IList <JoinArgument> JoinContainer { get; }
       IJoinQuery InnerJoin<TEntity>();
       IJoinQuery LeftJoin<TEntity>();
       IJoinQuery RightJoin<TEntity>();
       IEnumerator<TResult> Where<TEntity, TResult>(Expression<Func<TEntity, bool>> expression);
       IEnumerator<TResult> Where<TEntity1, TEntity2, TResult>(Expression<Func<TEntity1, TEntity2, bool>> expression);
       IEnumerator<TResult> Where<TEntity1, TEntity2, TEntity3, TResult>(Expression<Func<TEntity1, TEntity2, TEntity3, bool>> expression);
    }
}
