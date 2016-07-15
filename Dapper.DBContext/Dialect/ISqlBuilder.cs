using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext.Dialect
{
  public interface ISqlBuilder
    {
      string BuildInsert(Type modelType);
      string BuildUpdate(Type modelType);
      string BuildDelete(Type modelType);

      string BuildSelect<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : IEntity;
    }
}
