using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext.Dialect
{

    public class JoinArgument
    {
        public JoinArgument(Type modelType,string joinMethod,int aliasIndex)
        {
            this.EntityType = modelType;
            this.JoinMethod = joinMethod;
            this.Alias = string.Format("t{0}",aliasIndex);
        }
        public Type EntityType { get; private set; }
        /// <summary>
        ///  "inner join","left join" ,"right join" ,""
        /// </summary>
        public string JoinMethod { get; private set; }
        /// <summary>
        ///  entity alias eg: t0,t1,t2,t3
        /// </summary>
        public string Alias { get; private set; }



    }
}
