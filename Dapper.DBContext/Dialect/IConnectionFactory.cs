using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext.Dialect
{
   public interface IConnectionFactory
    {
        IDbConnection Create();

        ISqlBuilder Builder { get; }
    }
}
