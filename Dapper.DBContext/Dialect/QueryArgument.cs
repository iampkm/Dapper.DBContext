using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext.Dialect
{
   public class QueryArgument
    {
        public QueryArgument(string name, object value, string operate, string link)
        {
            this.Name = name;
            this.Value = value;
            this.Operator = operate;
            this.Link = link;
        }
         public string Name { get; private set; }

         public object Value { get; private set; }

         public string Operator { get; private set; }

        public string Link { get; private set; }

    }
}
