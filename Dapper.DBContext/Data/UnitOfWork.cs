using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.DBContext.Builder;
namespace Dapper.DBContext.Data
{
    /// <summary>
    /// 工作单元
    /// </summary>
    internal class UnitOfWork : IUnitOfWork
    {
        List<SqlArgument> _sqlList = new List<SqlArgument>();
        private Dictionary<string, object> _ParentKeyDic;
       // string _connectionStringName;
        IConnectionFactory _connectionFactory;
        public UnitOfWork(IConnectionFactory connectionFactory)
        {
            _ParentKeyDic = new Dictionary<string, object>();
            this._connectionFactory = connectionFactory;
        }
        public void Add(string sql, object paramObject, InsertMethodEnum method = InsertMethodEnum.Normal, string parentIdName = "")
        {
            SqlArgument model = new SqlArgument(sql, paramObject, method, parentIdName);
            this._sqlList.Add(model);
        }

        public void Commit()
        {
            string executeSql = "";
            int executeResult = 0;
            using (IDbConnection conn = this._connectionFactory.CreateConnection())
            {
                conn.Open();
                IDbTransaction tran = conn.BeginTransaction();
                try
                {
                    foreach (SqlArgument model in _sqlList)
                    {
                        executeSql = model.Sql;
                        switch (model.InsertMethod)
                        {
                            case InsertMethodEnum.Parent:
                                executeResult = conn.ExecuteScalar<int>(model.Sql, model.ParamObj, tran);
                                if (!string.IsNullOrWhiteSpace(model.ParentIdName) && !_ParentKeyDic.ContainsKey(model.ParentIdName))
                                {
                                    _ParentKeyDic.Add(model.ParentIdName, executeResult);
                                }
                                break;
                            case InsertMethodEnum.Child:                               
                                if (_ParentKeyDic.ContainsKey(model.ParentIdName))
                                {
                                    executeSql = model.ReplaceParentIdValue(_ParentKeyDic[model.ParentIdName]);
                                }
                                executeResult = conn.Execute(model.Sql, model.ParamObj, tran);
                                break;
                            default:
                                executeResult = conn.Execute(model.Sql, model.ParamObj, tran);
                                break;
                        }
                        if (executeResult <= 0) { 
                            throw new Exception(string.Format("rows is zero.sql={0},parameters={1}", model.Sql, model.ParamObj.ToString()));
                        }
                    }
                    tran.Commit();
                    this._sqlList.Clear();
                }
                catch (Exception ex)
                {     
                    tran.Rollback();
                    this._sqlList.Clear();
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    }
}
