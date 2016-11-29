using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dapper.DBContext.Schema
{
    /// <summary>
    /// 设置列名
    /// </summary>
   [AttributeUsage(AttributeTargets.Property)]
   public class ColumnAttribute:Attribute
    {
       public ColumnAttribute(string name)
        {
            this.Name = name;
        }
        /// <summary>
        ///  列名
        /// </summary>
        public string Name { get; private set; }
    }
}
