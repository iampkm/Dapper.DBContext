using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace Dapper.DBContext.Builder
{
   public interface IJoinQuery
    {
       JoinBuilderContext JoinContext { get; }
       IJoinQuery InnerJoin<TEntity>();
       IJoinQuery LeftJoin<TEntity>();
       IJoinQuery RightJoin<TEntity>();
       IEnumerable<TResult> Where<TResult>(Expression<Func<TResult, bool>> expression);
    }
}
