using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dapper.DBContext.Schema
{
     [AttributeUsage(AttributeTargets.Property)]
    /// <summary>
    /// 不映射到表
    /// </summary>
   public class NotMappedAttribute:Attribute
    {
    }
}
