//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using CQSS.O2O.Utils.Helper;
//using System.Dynamic;
//using System.Diagnostics;
//using CQSS.O2O.Models.Enum;
//using System.Collections;
//using CQSS.O2O.Models.Schema;
//namespace CQSS.O2O.DAL
//{

//    public class DapperContext : IDapperContext
//    {
//        private static readonly ConcurrentDictionary<Type, List<PropertyInfo>> _paramCache = new ConcurrentDictionary<Type, List<PropertyInfo>>();
//        private static readonly ConcurrentDictionary<Type, string> _KeyCache = new ConcurrentDictionary<Type, string>();
//        private string _tablePrefix = "O2O_";
//        private string _defaultKey = "SysNo"; //默认主键的名字
//        private IUnitOfWork _uow = new DapperUnitOfWork();
//        public DapperContext() { }
//        public DapperContext(string connectionString)
//        {
//            _uow = new DapperUnitOfWork(connectionString);
//        }
//        /// <summary>
//        /// 自定义连接字符串
//        /// </summary>
//        /// <param name="connectionString">连接字符串</param>
//        /// <param name="tablePrefix">表前缀</param>
//        /// <param name="keyName">主键名</param>
//        /// <param name="keyAutoIncrement">是否自增长</param>
//        public DapperContext(string connectionString, string tablePrefix, string keyName)
//            : this(connectionString)
//        {
//            this._tablePrefix = tablePrefix;
//            this._defaultKey = keyName;
//        }
//        /// <summary>
//        /// 采用默认连接串
//        /// </summary>
//        /// <param name="tablePrefix">表前缀</param>
//        /// <param name="keyName">主键名</param>
//        /// <param name="keyAutoIncrement">是否自增长</param>
//        public DapperContext(string tablePrefix, string keyName)
//        {
//            this._tablePrefix = tablePrefix;
//            this._defaultKey = keyName;
//        }

//        #region 接口实现
//        public void Insert<T>(T model) where T : class
//        {
//            string table = GetTableName(model.GetType());
//            var propertieInfos = GetPropertyInfos(model);
//            var properties = new List<string>();
//            var childObjects = new List<object>();
//            properties = GetBuildSqlProperties(model.GetType());
//            string keyName = GetKeyName(model.GetType());
//            // 子类的外键名，必须是 父类名+默认ID名；
//            string parentIdName = string.Format("{0}{1}", model.GetType().Name, keyName);
//            string sql = buildInsertSql(properties, table);
//            Trace.WriteLine(String.Format("Insert: {0}", sql));          
//            this._uow.Add(sql, model, InsertMethodEnum.Parent, parentIdName);           

//            // 查找 关联的子类对象
//            foreach (PropertyInfo pi in propertieInfos)
//            {
//                if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericArguments()[0].IsClass)
//                {
//                    object childLists = pi.GetValue(model, null);
//                    childObjects.Add(childLists);
//                }
//            }
//            //构造子对象sql 
//            foreach (object childObjList in childObjects)
//            {
//                foreach (object childObj in childObjList as IEnumerable)
//                {
//                    var childProperties = GetBuildSqlProperties(childObj.GetType());
//                    var childSql = buildInsertSql(childProperties, GetTableName(childObj.GetType()));
//                    Trace.WriteLine(String.Format("Insert: {0}", childSql));                  
//                    //Dapper 支持一个insert 语句 集合数据插入，
//                    this._uow.Add(childSql, childObjList, InsertMethodEnum.Child, parentIdName);
//                    break;
//                }
//            }
//        }

//        private string buildInsertSql(List<string> properties, string table)
//        {
//            var columns = string.Format("[{0}]", string.Join("],[", properties));
//            var values = string.Join(",", properties.Select(p => "@" + p));
//            var sql = string.Format("insert into [{0}] ({1}) values ({2}) select scope_identity()", table, columns, values);
//            return sql;
//        }

//        public List<string> GetBuildSqlProperties(Type type)
//        {
//            var propertieInfos = GetPropertyInfos(type);
//            var properties = new List<string>();
//            foreach (PropertyInfo pi in propertieInfos)
//            {
//                var attrs= pi.GetCustomAttributes(false);
//                if(attrs!=null||attrs.Length!=0)
//                {
//                    bool isSkip = false;
//                    foreach (var attr in attrs)
//                    {
//                        if (attr is NotMappedAttribute)
//                        {
//                            isSkip = true;
//                            break;
//                        }
//                        else if (attr is KeyAttribute && pi.PropertyType == typeof(int))  //标记了key 属性，且为int 型 就默认为自增长字段
//                        {
//                            isSkip = true;
//                            break;
//                        }
//                        else {
//                            continue;
//                        }
//                    }
//                    if (isSkip) { continue; }
//                };

//                if (pi.Name == this._defaultKey && pi.PropertyType == typeof(int)) // 属性名为约定的 值，且为int 型 就默认为自增长字段 
//                {
//                    continue; //自增跳过
//                }
//                if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericArguments()[0].IsClass)
//                {
//                    continue;  // 泛型集合跳过
//                }
//                // 值对象 加入
//                if (pi.PropertyType.IsClass && pi.PropertyType != typeof(string))
//                {
//                    properties.AddRange(GetBuildSqlProperties(pi.PropertyType));  //获取类中类子属性
//                    continue;
//                }
//                properties.Add(pi.Name);
//            }
//            return properties;
//        }

//        public void Insert<T>(List<T> models) where T : class
//        {
//            if (models.Count <= 0) return;
//            var obj = models[0] as object;
//            string table = GetTableName(obj.GetType());
//            // var propertiesInfo = GetPropertyInfos(obj);
//            var properties = GetBuildSqlProperties(obj.GetType());

//            var columns = string.Format("[{0}]", string.Join("],[", properties));
//            var values = string.Join(",", properties.Select(p => "@" + p));
//            var sql = string.Format("insert into [{0}] ({1}) values ({2})", table, columns, values);
//            Trace.WriteLine(String.Format("Insert: {0}", sql));
//            this._uow.Add(sql, models);
//        }

//        public void Update<T>(T model) where T : class
//        {
//            var obj = model as object;
//            string table = GetTableName(model.GetType());
//            //var updatePropertyInfos = GetPropertyInfos(obj);
//            //var updateProperties = updatePropertyInfos.Where(p => p.Name != this._defaultKey && IsSimpleType(p.PropertyType)).Select(p => p.Name);
//            var updateProperties = GetBuildSqlProperties(obj.GetType());
//            var updateFields = string.Join(",", updateProperties.Select(p => "[" + p + "] = @" + p));
//            var whereFields = string.Empty;
//            whereFields = string.Format(" where [{0}]=@{0}", GetKeyName(model.GetType()));
//            var sql = string.Format("update [{0}] set {1}{2}", table, updateFields, whereFields);
//            Trace.WriteLine(String.Format("Update: {0}", sql));
//            this._uow.Add(sql, model);
//        }
//        public void Update<T>(List<T> models) where T : class
//        {
//            if (models.Count == 0) throw new Exception("明细为空");
//            var model = models[0];
//            var obj = model as object;
//            string table = GetTableName(model.GetType());
//            // var updatePropertyInfos = GetPropertyInfos(obj);
//            var updateProperties = GetBuildSqlProperties(obj.GetType());
//            var updateFields = string.Join(",", updateProperties.Select(p => "[" + p + "] = @" + p));
//            var whereFields = string.Empty;
//            whereFields = string.Format(" where [{0}]=@{0}", GetKeyName(model.GetType()));
//            var sql = string.Format("update [{0}] set {1}{2}", table, updateFields, whereFields);
//            Trace.WriteLine(String.Format("Update: {0}", sql));
//            this._uow.Add(sql, models);
//        }

//        public void Update<T>(T model, object condition) where T : class
//        {
//            var obj = model as object;
//            var conditionObj = condition as object;
//            string table = GetTableName(model.GetType());
//            // var updatePropertyInfos = GetPropertyInfos(obj);
//            var wherePropertyInfos = GetPropertyInfos(conditionObj);

//            var updateProperties = GetBuildSqlProperties(obj.GetType());
//            var whereProperties = wherePropertyInfos.Select(p => p.Name);

//            var updateFields = string.Join(",", updateProperties.Select(p => "[" + p + "] = @" + p));
//            var whereFields = string.Empty;

//            if (whereProperties.Any())
//            {
//                //有更新条件，按更新条件修改
//                whereFields = " where " + string.Join(" and ", whereProperties.Select(p => "[" + p + "] = @w_" + p));
//            }
//            else
//            {
//                whereFields = string.Format(" where [{0}]=@{0}", GetKeyName(model.GetType()));
//            }

//            var sql = string.Format("update [{0}] set {1}{2}", table, updateFields, whereFields);
//            var parameters = new DynamicParameters(model);
//            var expandoObject = new ExpandoObject() as IDictionary<string, object>;
//            wherePropertyInfos.ForEach(p => expandoObject.Add("w_" + p.Name, p.GetValue(conditionObj, null)));
//            parameters.AddDynamicParams(expandoObject);
//            Trace.WriteLine(String.Format("Update: {0}", sql));
//            this._uow.Add(sql, parameters);
//        }

//        public void Delete<T>(int Id) where T : class
//        {
//            string table = GetTableName(typeof(T));
//            string keyName = GetKeyName(typeof(T));
//            var sql = string.Format("delete from [{0}] where [{1}]=@{2}", table, keyName, "ID");
//            Trace.WriteLine(String.Format("Delete: {0}", sql));
//            this._uow.Add(sql, new { ID = Id });
//        }

//        public void Delete<T>(List<int> identitys) where T : class
//        {
//            string table = GetTableName(typeof(T));
//            string keyName = GetKeyName(typeof(T));
//            var sql = string.Format("delete from [{0}] where [{1}] in @{2}", table, keyName, "ID");
//            Trace.WriteLine(String.Format("Delete: {0}", sql));
//            this._uow.Add(sql, new { ID = identitys.ToArray() });
//        }

//        public void Delete<T>(string Id) where T : class
//        {
//            string table = GetTableName(typeof(T));
//            string keyName = GetKeyName(typeof(T));
//            var sql = string.Format("delete from [{0}] where [{1}]='{2}'", table, keyName, "ID");
//            Trace.WriteLine(String.Format("Delete: {0}", sql));
//            this._uow.Add(sql, new { ID = Id });
//        }

//        public void Delete<T>(object condition) where T : class
//        {
//            var conditionObj = condition as object;
//            var whereFields = string.Empty;
//            var whereProperties = GetProperties(conditionObj);
//            if (whereProperties.Count > 0)
//            {
//                whereFields = " where " + string.Join(" and ", whereProperties.Select(p => "[" + p + "] = @" + p));
//            }
//            string table = GetTableName(typeof(T));
//            var sql = string.Format("delete from [{0}]{1}", table, whereFields);
//            Trace.WriteLine(String.Format("Delete: {0}", sql));
//            this._uow.Add(sql, condition);
//        }

//        public void SaveChange()
//        {
//            this._uow.Commit();
//        }

//        public T GetById<T>(int Id) where T : class
//        {
//            string keyName = GetKeyName(typeof(T));
//            string sql = string.Format("select * from {0} where [{1}]={2}", GetTableName(typeof(T)), keyName, Id);

//            Trace.WriteLine(String.Format("GetById: {0}", sql));

//            return DbHelper.QuerySingle<T>(sql);
//        }

//        public T GetById<T>(string Id) where T : class
//        {
//            string keyName = GetKeyName(typeof(T));
//            string sql = string.Format("select * from {0} where [{1}]='{2}'", GetTableName(typeof(T)), keyName, Id);
//            Trace.WriteLine(String.Format("GetById: {0}", sql));
//            return DbHelper.QuerySingle<T>(sql);
//        }

//        public T GetByCondition<T>(object condition) where T : class
//        {
//            string sql = BuildQuerySQL(condition, GetTableName(typeof(T)));
//            Trace.WriteLine(String.Format("GetByCondition: {0}", sql));
//            return DbHelper.QuerySingle<T>(sql, condition);
//        }

//        public IEnumerable<T> QueryList<T>(object condition) where T : class
//        {
//            string sql = BuildQuerySQL(condition, GetTableName(typeof(T)));
//            Trace.WriteLine(String.Format("QueryList: {0}", sql));
//            return DbHelper.Query<T>(sql, condition);
//        }

//        public IEnumerable<T> QueryPage<T>(object condition, int pageIndex, int pageSize, out int totalRows, string columns = "*", bool isOr = false, string orderBy = "") where T : class
//        {
//            var conditionObj = condition as object;
//            var whereFields = string.Empty;
//            var properties = GetProperties(conditionObj);
//            if (properties.Count > 0)
//            {
//                var separator = isOr ? " OR " : " AND ";
//                whereFields = " WHERE " + string.Join(separator, properties.Select(p => p + " = @" + p));
//            }
//            string table = GetTableName(typeof(T));
//            var sql = string.Format("SELECT {0} FROM (SELECT ROW_NUMBER() OVER (ORDER BY {1}) AS RowNumber, {0} FROM {2}{3}) AS Total WHERE RowNumber >= {4} AND RowNumber <= {5}", columns, orderBy, table, whereFields, (pageIndex - 1) * pageSize + 1, pageIndex * pageSize);
//            var sqlTotal = string.Format("SELECT {0} FROM (SELECT ROW_NUMBER() OVER (ORDER BY {1}) AS RowNumber, {0} FROM {2}{3}) AS Total WHERE RowNumber >= {4} AND RowNumber <= {5}", "count(*)", orderBy, table, whereFields, (pageIndex - 1) * pageSize + 1, pageIndex * pageSize);
//            totalRows = DbHelper.QueryScalar<int>(sqlTotal, conditionObj);
//            var listResult = DbHelper.Query<T>(sql, conditionObj);
//            Trace.WriteLine(String.Format("QueryPage: {0}", sql));
//            Trace.WriteLine(String.Format("QueryPage: {0}", sqlTotal));
//            return listResult;
//        }

//        public bool Exists<T>(object condition) where T : class
//        {
//            string sql = BuildQuerySQL(condition, GetTableName(typeof(T)), selectPart: "count(*)");
//            Trace.WriteLine(String.Format("GetByCondition: {0}", sql));
//            return DbHelper.QueryScalar<int>(sql, condition) > 0;
//        }

//        #endregion

//        #region 属性辅助方法



//        private static string BuildQuerySQL(dynamic condition, string table, string selectPart = "*", bool isOr = false)
//        {
//            var conditionObj = condition as object;
//            var properties = GetProperties(conditionObj);
//            if (properties.Count == 0)
//            {
//                return string.Format("SELECT {1} FROM [{0}]", table, selectPart);
//            }

//            var separator = isOr ? " OR " : " AND ";
//            var wherePart = string.Join(separator, properties.Select(p => p + " = @" + p));

//            return string.Format("SELECT {2} FROM [{0}] WHERE {1}", table, wherePart, selectPart);
//        }

//        private static List<string> GetProperties(object obj)
//        {
//            if (obj == null)
//            {
//                return new List<string>();
//            }
//            if (obj is DynamicParameters)
//            {
//                return (obj as DynamicParameters).ParameterNames.ToList();
//            }
//            return GetPropertyInfos(obj).Select(x => x.Name).ToList();
//        }
//        private static List<PropertyInfo> GetPropertyInfos(Type type)
//        {
//            List<PropertyInfo> properties = new List<PropertyInfo>();
//            if (_paramCache.TryGetValue(type, out properties)) return properties.ToList();
//            properties = type.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public).ToList();
//            _paramCache[type] = properties;
//            return properties;
//        }
//        private static List<PropertyInfo> GetPropertyInfos(object obj)
//        {
//            if (obj == null)
//            {
//                return new List<PropertyInfo>();
//            }

//            List<PropertyInfo> properties;
//            if (_paramCache.TryGetValue(obj.GetType(), out properties)) return properties.ToList();
//            properties = obj.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public).ToList();
//            _paramCache[obj.GetType()] = properties;
//            return properties;
//        }

//        private string GetTableName(Type t)
//        {
//            if (t.Name.StartsWith(this._tablePrefix))
//            {
//                return t.Name;
//            }
//            else
//            {
//                return string.Format("{0}{1}", this._tablePrefix, t.Name);
//            }
//        }

//        private static List<Type> _simpleTypes = new List<Type>
//                               {
//                                   typeof(byte),
//                                   typeof(sbyte),
//                                   typeof(short),
//                                   typeof(ushort),
//                                   typeof(int),
//                                   typeof(uint),
//                                   typeof(long),
//                                   typeof(ulong),
//                                   typeof(float),
//                                   typeof(double),
//                                   typeof(decimal),
//                                   typeof(bool),
//                                   typeof(string),
//                                   typeof(char),
//                                   typeof(Guid),
//                                   typeof(DateTime),
//                                   typeof(DateTimeOffset),
//                                   typeof(byte[])                                 
//                               };

//        private static bool IsSimpleType(Type type)
//        {
//            Type actualType = type;
//            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
//            {
//                actualType = type.GetGenericArguments()[0];
//            }
//            if (type.IsEnum) { return true; }
//            return _simpleTypes.Contains(actualType);
//        }

//        public string GetKeyName(Type type)
//        {
//            string keyName ="";
//            _KeyCache.TryGetValue(type, out keyName);
//            if (!string.IsNullOrEmpty(keyName))
//            {
//                return keyName;
//            }
//            keyName = this._defaultKey;
//            var propertiesInfos = GetPropertyInfos(type);
//            foreach (var column in propertiesInfos)
//            {               
//                var attrs = column.GetCustomAttributes(typeof(KeyAttribute), false);
//                if (attrs==null||attrs.Length == 0)
//                {
//                    if (column.Name == this._defaultKey)
//                    {
//                        keyName = column.Name;
//                        break;
//                    }
//                }
//                else
//                {
//                    keyName = column.Name;
//                    break;
//                }
//            }
//            _KeyCache[type] = keyName;
//            return keyName;
//        }

//        #endregion


//    }
//}
