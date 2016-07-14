using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dapper.DBContext.Schema
{
   [AttributeUsage(AttributeTargets.Property)]
    /// <summary>
    /// 标记主键
    /// </summary>
   public class KeyAttribute:Attribute
    {      
    }
}
