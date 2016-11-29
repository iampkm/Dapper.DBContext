using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext.Data
{
    public interface IExecuteQuery
    {
        T ExecuteScalar<T>(string sql, object param);      
        IEnumerable<TEntity> Query<TEntity>(string sql, object param);     
        TEntity QuerySingle<TEntity>(string sql, object param);     
    }
}
