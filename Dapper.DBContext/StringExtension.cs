using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext
{
   public static class StringExtension
    {
       /// <summary>
       /// sql like 
       /// </summary>
       /// <param name="name"></param>
       /// <param name="value">eg: like("%123%")</param>
       /// <returns></returns>
       public static bool Like(this string name, string value)
       {
           return true;
       }
    }
}
