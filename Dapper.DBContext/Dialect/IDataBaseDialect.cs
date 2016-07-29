using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext.Dialect
{
    public interface IDataBaseDialect
    {      
         string WrapFormat { get; }
        /// <summary>
        /// 分页格式字符串
        /// </summary>
         string PageFormat { get; }
         string IdentityFromat { get; }
       
    }
}
