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

      string BuildWhere<TEntity>(Expression<Func<TEntity, bool>> expression, out object arguments) where TEntity : IEntity;

        string BuildSelect<TEntity>() where TEntity : IEntity;

        /// <summary>
        /// 获取ｋｅｙ　名
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="isWarpDialect">是否包裹方言</param>
        /// <returns></returns>
        string GetKeyName(Type modelType,bool isWarpDialect);
    }
}
