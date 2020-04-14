using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext
{
    /// <summary>
    ///  系统全局配置参数
    /// </summary>
   public  class Settings
    {
       static Settings() {
           PrimaryKey = "Id";
           Timestamp = "RowVersion";
       }      

       /// <summary>
       ///  主键名，默认"Id" 
       /// </summary>
       public static string PrimaryKey { get; set; }
       /// <summary>
       ///  时间戳并发控制字段。 默认数据库列名值："RowVersion". C#实体类属性： sqlserver 用byte[] 类型，mysql 用datetime 类型
       /// </summary>
       public static string Timestamp { get; set; }

       public static string ForeignKeyFormat = "{0}{1}";
    }
}
