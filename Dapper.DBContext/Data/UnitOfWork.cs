using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.DBContext.Builder;
using System.Diagnostics;
using Dapper.DBContext.Helper;
namespace Dapper.DBContext.Data
{
    /// <summary>
    /// 工作单元
    /// </summary>
    internal class UnitOfWork : IUnitOfWork
    {
        List<SqlArgument> _sqlList = new List<SqlArgument>();
        private Dictionary<string, object> _ParentKeyDic;
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
            object exceuteObject = null;
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
                        exceuteObject = model.ParamObj;
                        print(executeSql);
                        switch (model.InsertMethod)
                        {
                            case InsertMethodEnum.Parent:
                                executeResult = conn.ExecuteScalar<int>(model.Sql, model.ParamObj, tran);
                                if (!string.IsNullOrWhiteSpace(model.ParentIdName) && !_ParentKeyDic.ContainsKey(model.ParentIdName))
                                {
                                    _ParentKeyDic.Add(model.ParentIdName, executeResult);
                                }
                                // 设置主键自增ID 值
                                ReflectionHelper.SetPrimaryKey(model.ParamObj, executeResult);
                                break;
                            case InsertMethodEnum.Child:
                                if (_ParentKeyDic.ContainsKey(model.ParentIdName))
                                {
                                    ReflectionHelper.SetForeignKey(model.ParamObj, model.ParentIdName, _ParentKeyDic[model.ParentIdName]);
                                }
                                executeResult = conn.Execute(model.Sql, model.ParamObj, tran);
                                break;
                            default:
                                executeResult = conn.Execute(model.Sql, model.ParamObj, tran);
                                break;
                        }
                    }
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw new Exception(string.Format("sql exception.sql={0}.\r\n paramObject={1}", executeSql, ReflectionHelper.Serialize(exceuteObject)), ex);
                }
                finally
                {
                    this._sqlList.Clear();
                    this._ParentKeyDic.Clear();
                    conn.Close();
                }
            }
        }


        public void CommitAsync()
        {
            string executeSql = "";
            object exceuteObject = null;
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
                        exceuteObject = model.ParamObj;
                        print(executeSql);
                        switch (model.InsertMethod)
                        {
                            case InsertMethodEnum.Parent:
                                executeResult = conn.ExecuteScalarAsync<int>(model.Sql, model.ParamObj, tran).Result;
                                if (!string.IsNullOrWhiteSpace(model.ParentIdName) && !_ParentKeyDic.ContainsKey(model.ParentIdName))
                                {
                                    _ParentKeyDic.Add(model.ParentIdName, executeResult);
                                }
                                ReflectionHelper.SetPrimaryKey(model.ParamObj, executeResult);
                                break;
                            case InsertMethodEnum.Child:
                                if (_ParentKeyDic.ContainsKey(model.ParentIdName))
                                {
                                    // executeSql = model.ReplaceParentIdValue(_ParentKeyDic[model.ParentIdName]);
                                    ReflectionHelper.SetForeignKey(model.ParamObj, model.ParentIdName, _ParentKeyDic[model.ParentIdName]);
                                }
                                executeResult = conn.ExecuteAsync(model.Sql, model.ParamObj, tran).Result;
                                break;
                            default:
                                executeResult = conn.ExecuteAsync(model.Sql, model.ParamObj, tran).Result;
                                break;
                        }
                    }
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    this._sqlList.Clear();
                    throw new Exception(string.Format("sql exception.sql={0}.\r\n paramObject={1}", executeSql, ReflectionHelper.Serialize(exceuteObject)), ex);
                }
                finally
                {
                    this._sqlList.Clear();
                    this._ParentKeyDic.Clear();
                    conn.Close();
                }
            }
        }
        private void print(string msg)
        {
            if (Debugger.IsAttached)
                Trace.WriteLine(msg);
        }
    }
}
