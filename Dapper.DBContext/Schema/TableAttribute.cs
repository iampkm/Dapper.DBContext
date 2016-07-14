using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dapper.DBContext.Schema
{
     [AttributeUsage(AttributeTargets.Class)]
    /// <summary>
    /// 设置表名
    /// </summary>
    public class TableAttribute : Attribute
    {
        public TableAttribute(string name)
        {
            this.Name = name;
        }
        /// <summary>
        /// 表名
        /// </summary>
        public string Name { get; private set; }
    }
}
