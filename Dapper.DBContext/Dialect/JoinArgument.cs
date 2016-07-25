using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext.Dialect
{

    public class JoinArgument
    {
        public JoinArgument(Type modelType,string joinMethod)
        {
            this.EntityType = modelType;
            this.JoinMethod = joinMethod;
        }
        public Type EntityType { get; private set; }
        /// <summary>
        ///  "inner join","left join" ,"right join" ,""
        /// </summary>
        public string JoinMethod { get; private set; }

       

    }
}
