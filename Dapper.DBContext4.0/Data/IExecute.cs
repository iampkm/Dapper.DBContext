using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dapper.DBContext.Data
{
   public interface IExecute
    {
       /// <summary>
       /// 立即执行，并返回结果,无事务
       /// </summary>
       /// <param name="sql"></param>
       /// <param name="param"></param>
       /// <returns></returns>
       int Execute(string sql, object param = null);
       /// <summary>
       /// 加入工作单元，纳入事务执行
       /// </summary>
       /// <param name="sql"></param>
       /// <param name="param"></param>
       void AddExecute(string sql, object param = null);
    }
}
