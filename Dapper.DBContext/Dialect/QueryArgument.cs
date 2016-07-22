using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext.Dialect
{
   public class QueryArgument
    {
        public QueryArgument(Type entityType, string name,string argumentName, object value, string operate, string link)
        {
            this.EntityType = entityType;
            this.Name = name;
            this.Value = value;
            this.Operator = operate;
            this.Link = link;
            this.ArgumentName = argumentName;
        }
        public Type EntityType { get; private set; }
         public string Name { get; private set; }

         public string ArgumentName { get; private set; }

         public object Value { get; private set; }

         public string Operator { get; private set; }

        public string Link { get; private set; }

    }
}
