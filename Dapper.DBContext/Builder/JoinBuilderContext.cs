using Dapper.DBContext.Helper;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext.Builder
{
    public class JoinBuilderContext
    {
        public JoinBuilderContext()
        {
            this.IsPage = false;
            this.JoinTables = new List<JoinArgument>();
        }
        /// <summary>
        /// is page
        /// </summary>
        public bool IsPage { get; private set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        /// <summary>
        /// all join table object
        /// </summary>
        public IList<JoinArgument> JoinTables { get; private set; }
        public void SetPageInfo(int pageIndex, int pageSize)
        {
            this.IsPage = true;
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
        }

        public void Add(Type entityType, string joinMethod = "")
       {
           var args = new JoinArgument(entityType, joinMethod, this.JoinTables.Count);
       }

    }
}
