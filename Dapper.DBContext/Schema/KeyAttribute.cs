using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dapper.DBContext.Schema
{
   [AttributeUsage(AttributeTargets.Property)]
    /// <summary>
    /// 主键标记，默认自增长
    /// </summary>
   public class KeyAttribute:Attribute
    {
       public KeyAttribute(bool autoIncrement = true)
       {
           this.AutoIncrement = autoIncrement;      
       }
       /// <summary>
       ///  主键自动增长，主键类型为int 或 long 才生效
       /// </summary>
       public bool AutoIncrement { get; private set; }
    }

    // 主键默认逻辑：  列名=Id ，列名被标记 Key
    // 主键是否自增逻辑：  主键类型为 int 或long  默认自增
   // 标记Key 的属性， 根据 AutoIncrement 判断是否自增
}
