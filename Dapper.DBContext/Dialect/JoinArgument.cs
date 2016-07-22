using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext.Dialect
{

    public class JoinArgument
    {
        public JoinArgument(Type modelType, string tableAlias, string sql, object value = null, bool isWhere = false, int pageIndex = 0, int pageSize = 0,bool isPageSql = false)
        {
            this.EntityType = modelType;
            this.TableAlias = tableAlias;
            this.value = value;
            this.IsWhere = isWhere;
            this.Sql = sql;
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.IsPageSql = isPageSql;
        }
        public Type EntityType { get; private set; }

        public string TableAlias { get; private set; }

        public string Sql { get; set; }

        public object value { get; private set; }

        public bool IsWhere { get; private set; }
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }

        public bool IsPageSql { get; private set; }

        /// <summary>
        /// execute sql
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="callback"></param>
        public void QueryCallBack<TResult>(Func<string, object, IEnumerable<TResult>> callback)
        {
            if (IsWhere)
            {
                callback(this.Sql, value);
            }
        }

    }
}
