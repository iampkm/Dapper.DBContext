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
       /// <summary>
       ///  custom query condition. alias of first table is t0 ,second t1 and so on. 
       /// </summary>
       /// <typeparam name="TResult"></typeparam>
       /// <param name="where">eg: t0.Id=@Id and t1.Code like '@Code'</param>
       /// <param name="arguments">eg:  new {Id=1,Code=2 }</param>
       /// <returns></returns>
       IEnumerable<TResult> Where<TResult>(string where,object arguments);
    }
}
