using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext.Transaction
{
    public enum InsertMethodEnum
    {
        /// <summary>
        /// 普通
        /// </summary>
        Normal,
        /// <summary>
        /// 父表
        /// </summary>
        Parent,
        /// <summary>
        /// 子表
        /// </summary>
        Child
    }
}
