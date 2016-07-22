using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace Dapper.DBContext.Dialect
{
    public interface ISqlBuilder
    {
        string BuildInsert(Type modelType);
        string BuildUpdate(Type modelType);
        string BuildDelete(Type modelType);

        string BuildWhere<TEntity>(Expression<Func<TEntity, bool>> expression, out object arguments) where TEntity : IEntity;

        string BuildSelect<TEntity>() where TEntity : IEntity;
        string BuildSelect<TEntity>(string columns) where TEntity : IEntity;
        
        string GetKeyName(Type modelType, bool isWrapDialect);

        IJoinQuery BuildJoin<TEntity>();
        IJoinQuery BuildPage<TEntity>(int pageIndex, int pageSize);

    }
}
