using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext.Data
{
    /// <summary>
    /// execute arguments
    /// </summary>
  public  class SqlArgument
    {
        public SqlArgument(string sql, object paramObject, InsertMethodEnum method = InsertMethodEnum.Normal, string parentIdName = "")
        {
            this.Sql = sql;
            this.ParamObj = paramObject;
            this.InsertMethod = method;
            this.ParentIdName = parentIdName;
        }

        public string Sql { get; private set; }

        public object ParamObj { get; private set; }

        public InsertMethodEnum InsertMethod { get; private set; }
        /// <summary>
        /// 父表主键 名字 ：例如 ParentIdName=OrderId
        /// </summary>
        public string ParentIdName { get; private set; }
        /// <summary>
        /// 用父表子增长ID值，替换sql 中的外键值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ReplaceParentIdValue(object value)
        {
            if (value is int || value is long)
            {
                this.Sql = this.Sql.Replace("@" + this.ParentIdName, string.Format("{0}", value));
            }
            else //其他都当做字符串处理
            {
                this.Sql = this.Sql.Replace("@" + this.ParentIdName, string.Format("'{0}'", value));
            }
            return this.Sql;
        }
    }
}
