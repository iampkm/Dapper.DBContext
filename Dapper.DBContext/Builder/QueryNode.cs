using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext.Builder
{
   public class QueryNode
    {
       public QueryNode(QueryNodeType nodeType, Expression condition)
       {
           this.NodeType = nodeType;
           this.Condition = condition;
       }
       public QueryNodeType NodeType { get; set; }
       public Expression Condition { get; set; }
      
    }

   public enum QueryNodeType
   {
       Where,
       OrderBy,
       OrderByDesc,
       Select,
       Limit
   }
}
