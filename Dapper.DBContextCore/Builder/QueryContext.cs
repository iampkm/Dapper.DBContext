using Dapper.DBContext.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext.Builder
{
    /// <summary>
    ///  查询上下文
    /// </summary>
    public class QueryContext
    {
        public QueryContext(ISqlBuilder builder,IExecuteQuery query)
        {
            Nodes = new List<QueryNode>();
            this.IsLimit = false;
            this.Builder = builder;
            this.Query = query;
        }

        public QueryContext() {
            Nodes = new List<QueryNode>();
            this.IsLimit = false;
        }

        public Type EntityType { get; set; }

        public List<QueryNode> Nodes { get; set; }
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public bool IsLimit { get; private set; }

        public void Limit(int pageSize, int pageIndex)
        {
            this.IsLimit = true;
            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
        }

        public ISqlBuilder Builder { get; private set; }

        public IExecuteQuery Query { get; private set; }

        public void Clear() {
            this.Nodes.Clear();
        }
    }
}
