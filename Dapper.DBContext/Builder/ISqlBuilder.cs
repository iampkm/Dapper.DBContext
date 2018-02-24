using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace Dapper.DBContext.Builder
{
    public interface ISqlBuilder
    {
        string BuildInsert(Type modelType);
        string BuildUpdate(Type modelType);
        string BuildUpdate<TEntity>(TEntity entity, Expression<Func<TEntity, bool>> where, out object argumnets);
        string BuildUpdate<TEntity>(Expression<Func<TEntity, TEntity>> columns, Expression<Func<TEntity, bool>> where,out object argumnets);
        string BuildDelete(Type modelType, bool isOnlyOneId = true);
        string BuildDeleteByLamda<TEntity>(Expression<Func<TEntity, bool>> expression,out object arguments);
        string BuildSelectById<TEntity>(bool isOnlyOneId = true);
        string BuildSelect<TEntity>();
        string BuildSelect<TEntity>(string columns);
        string BuildSelectByLamda<TEntity>(Expression<Func<TEntity, bool>> expression, out object arguments, string columns = "");
        string BuildSelectByLamda<TEntity, TResult>(Expression<Func<TEntity, bool>> expression, out object arguments, Expression<Func<TEntity, TResult>> select,string function);

        string BuildSelectByContext<TEntity>(QueryContext context, out object arguments);

    }
}
