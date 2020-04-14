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

       public static bool In(this int name, int[] value)
       {
           return true;
       }
       public static bool In(this long name, long[] value)
       {
           return true;
       }
       public static bool In(this string name, string[] value)
       {
           return true;
       }

       public static bool Between(this DateTime value, DateTime from, DateTime to)
       {
           return true;
       }
       public static bool Between(this int value, int from, int to)
       {
           return true;
       }
       public static bool Between(this long value, long from, long to)
       {
           return true;
       }
       public static bool Between(this decimal value, decimal from, decimal to)
       {
           return true;
       }
    }
}
