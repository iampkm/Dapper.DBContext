using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext.Data
{
   public interface IUnitOfWork
    {
        void Add(string sql, object paramObject, InsertMethodEnum method = InsertMethodEnum.Normal, string parentIdName = "");
        void Commit();
        /// <summary>
        /// 异步提交
        /// </summary>
        void CommitAsync();
    }
}
