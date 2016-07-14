//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using CQSS.O2O.Utils.Extension;
//using CQSS.O2O.Utils.Helper;
//using System.Data;
//using CQSS.O2O.Models;
//using CQSS.O2O.Models.Enum;
//namespace CQSS.O2O.DAL
//{
//    public class DapperUnitOfWork : IUnitOfWork
//    {
//        private string _connectionStringName = "";
//        private Dictionary<string, object> _ParentKeyDic;

//        public DapperUnitOfWork()
//        {
//            _ParentKeyDic = new Dictionary<string, object>();
//        }

//        public DapperUnitOfWork(string connectionStringName)
//            : this()
//        {
//            this._connectionStringName = connectionStringName;
//        }



//        List<SqlModel> _sqlList = new List<SqlModel>();
//        public void Add(string sql, object paramObject, InsertMethodEnum method = InsertMethodEnum.Normal, string parentIdName = "")
//        {
//            SqlModel model = new SqlModel(sql, paramObject, method, parentIdName);
//            this._sqlList.Add(model);
//        }

//        public void Commit()
//        {
//            string executeSql = "";
//            int executeResult = 0;
//            using (IDbConnection conn = DbHelper.OpenConnection(this._connectionStringName))
//            {
//                IDbTransaction tran = conn.BeginTransaction();
//                try
//                {
//                    foreach (SqlModel model in _sqlList)
//                    {
//                        executeSql = model.Sql;
//                        switch (model.InsertMethod)
//                        {                           
//                            case InsertMethodEnum.Parent:
//                                executeResult = conn.ExecuteScalar<int>(model.Sql, model.ParamObj, tran);
//                                if (!string.IsNullOrWhiteSpace(model.ParentIdName) && !_ParentKeyDic.ContainsKey(model.ParentIdName))
//                                {
//                                    _ParentKeyDic.Add(model.ParentIdName, executeResult);
//                                }
//                                break;
//                            case InsertMethodEnum.Child:
//                                //替换parentid
//                                if (_ParentKeyDic.ContainsKey(model.ParentIdName))
//                                {
//                                    executeSql = model.ReplaceParentIdValue(_ParentKeyDic[model.ParentIdName]); 
//                                }                               
//                                executeResult = conn.Execute(model.Sql, model.ParamObj, tran);                              
//                                break;
//                            default:
//                                executeResult = conn.Execute(model.Sql, model.ParamObj, tran);                               
//                                break;
//                        }
//                        if (executeResult <= 0) { LogWriter.WriteLog("执行结果{0},sql={1};参数{2}", ExceptionHelper.ExceptionLevel.Infomation, executeResult, model.Sql, model.ParamObj.ToString()); }

//                    }
//                    tran.Commit();
//                    this._sqlList.Clear();
//                }
//                catch (Exception ex)
//                {
//                    LogWriter.WriteLog("sql={0}错误消息：{1},堆栈{2}", ExceptionHelper.ExceptionLevel.Exception, executeSql, ex.Message, ex.StackTrace);
//                    tran.Rollback();
//                    this._sqlList.Clear();
//                    throw new Exception("数据存储异常！");
//                }
//            }
//        }
//    }

//    public class SqlModel
//    {

//        public SqlModel(string sql, object paramObject, InsertMethodEnum method = InsertMethodEnum.Normal, string parentIdName = "")
//        {
//            this.Sql = sql;
//            this.ParamObj = paramObject;
//            this.InsertMethod = method;
//            this.ParentIdName = parentIdName;
//        }

//        public string Sql { get; private set; }

//        public object ParamObj { get; private set; }

//        public InsertMethodEnum InsertMethod { get; private set; }
//        /// <summary>
//        /// 父表主键 名字 ：例如 ParentIdName=OrderId
//        /// </summary>
//        public string ParentIdName { get; private set; }
//        /// <summary>
//        /// 用父表子增长ID值，替换sql 中的外键值
//        /// </summary>
//        /// <param name="value"></param>
//        /// <returns></returns>
//        public string ReplaceParentIdValue(object value)
//        {
//            if (value is int || value is long)
//            {
//                this.Sql = this.Sql.Replace("@" + this.ParentIdName, string.Format("{0}", value));
//            }
//            else //其他都当做字符串处理
//            {
//                this.Sql = this.Sql.Replace("@" + this.ParentIdName, string.Format("'{0}'", value));
//            }
//            return this.Sql;
//        }

//    }
//    /// <summary>
//    /// 新增方式
//    /// </summary>
//    public enum InsertMethodEnum
//    {
//        /// <summary>
//        /// 普通
//        /// </summary>
//        Normal,
//        /// <summary>
//        /// 父表
//        /// </summary>
//        Parent,
//        /// <summary>
//        /// 子表
//        /// </summary>
//        Child
//    }
//}
