using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext.Data
{
    public interface IDataBaseDialect
    {      
        /// <summary>
        ///  表名，列名包裹格式
        /// </summary>
         string WrapFormat { get; }
        /// <summary>
        /// 分页格式字符串
        /// </summary>
         string PageFormat { get; }
        /// <summary>
        ///  自增格式
        /// </summary>
         string IdentityFormat { get; }
         /// <summary>
         ///  变量声明符号 sql,mysql = @ Oracle =:
         /// </summary>
         string VariableFormat { get; }
       
    }
}
