using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper.DBContext.Helper;
using System.Collections;
using System.Dynamic;

namespace Dapper.DBContext.Dialect
{
    public class SqlBuilder : ISqlBuilder
    {
        private static readonly ConcurrentDictionary<string, string> _SqlCache = new ConcurrentDictionary<string, string>();
        DataBaseEnum _dbType;
        string _encapsulation;
        string _identitySql;
        string _getPagedSql;
        public SqlBuilder()
        {          
            SetDataBase();
        }
        public SqlBuilder(DataBaseEnum dbType):this()
        {
            this._dbType = dbType;          
        }
        public string BuildInsert(Type modelType)
        {
            var sqlKey = GetModelSqlKey(modelType, Operator.Insert);
            if (_SqlCache.ContainsKey(sqlKey))
            {
                return _SqlCache[sqlKey];
            }
            string table = GetTable(modelType);
            var properties = ReflectionHelper.GetBuildSqlProperties(modelType);
            var values = string.Join(",", properties.Select(p => "@" + p));
            var columns = string.Join(",", properties.Select(p => string.Format(_encapsulation, p)));
            var sql = string.Format("insert into {0} ({1}) values ({2}) {3}", table, columns, values, this._identitySql);
            _SqlCache[sqlKey] = sql;
            return sql;

        }

        public string BuildUpdate(Type modelType)
        {
            var sqlKey = GetModelSqlKey(modelType, Operator.Update);
            if (_SqlCache.ContainsKey(sqlKey))
            {
                return _SqlCache[sqlKey];
            }

            string table = GetTable(modelType);
            //var updatePropertyInfos = GetPropertyInfos(obj);
            //var updateProperties = updatePropertyInfos.Where(p => p.Name != this._defaultKey && IsSimpleType(p.PropertyType)).Select(p => p.Name);
            var updateProperties = ReflectionHelper.GetBuildSqlProperties(modelType);
            var updateFields = string.Join(",", updateProperties.Select(p => string.Format(_encapsulation, p) + " = @" + p));
            var whereFields = string.Empty;
            var rowVersion = "";
            if (ReflectionHelper.GetPropertyInfos(modelType).Exists(p => p.Name == "RowVersion" && p.PropertyType == typeof(byte[])))
            {
                rowVersion = string.Format("and [RowVersion]=@RowVersion");
            }
            whereFields = string.Format("where {0}=@{1} {2}", GetKey(modelType), ReflectionHelper.GetKeyName(modelType),rowVersion);
            var sql = string.Format("update {0} set {1} {2}", table, updateFields, whereFields);
            _SqlCache[sqlKey] = sql;
            return sql;
        }

        public string BuildDelete(Type modelType)
        {
            var sqlKey = GetModelSqlKey(modelType, Operator.Update);
            if (_SqlCache.ContainsKey(sqlKey))
            {
                return _SqlCache[sqlKey];
            }
            string table = GetTable(modelType);
            var rowVersion = "";
            if (ReflectionHelper.GetPropertyInfos(modelType).Exists(p => p.Name == "RowVersion" && p.PropertyType == typeof(byte[])))
            {
                rowVersion = string.Format("and [RowVersion]=@RowVersion");
            }
            var sql = string.Format("delete from {0} where {1}=@{2} {3}", table, GetKey(modelType), ReflectionHelper.GetKeyName(modelType),rowVersion);
            _SqlCache[sqlKey] = sql;
            return sql;
        }

        public string BuildWhere<TEntity>(System.Linq.Expressions.Expression<Func<TEntity, bool>> expression,out object arguments) where TEntity : IEntity
        {
            var queryArgments = LamdaHelper.GetWhere<TEntity>(expression);
          //  Dictionary<string, object> dic = new Dictionary<string, object>();
            dynamic args = new ExpandoObject();
            StringBuilder sql = new StringBuilder();
            sql.Append("where ");
            foreach (QueryArgument argument in queryArgments)
            {
                ((IDictionary<string, object>)args)[argument.Name] = argument.Value;
              
                 sql.AppendFormat("{0} {2} @{3} {4}", getColumn(argument.Name), argument.Operator, argument.Name, argument.Link);
               

            }
            arguments =args;

            return sql.ToString() ;
        }

        public string BuildSelect<TEntity>() where TEntity : IEntity
        {
            return BuildSelect<TEntity>("");
        }

        public string BuildSelect<TEntity>(string columns) where TEntity : IEntity
        {
            var modelType = typeof(TEntity);
            string table = GetTable(modelType);
            var properties = ReflectionHelper.GetSelectSqlProperties(modelType);
            if (string.IsNullOrEmpty(columns))
            {
                columns = string.Join(",", properties.Select(p => string.Format(_encapsulation, p)));
            }          
            return string.Format("select {0} from {1} ", columns, table);
        }
        public string GetKeyName(Type modelType, bool isWarpDialect)
        {
            if (isWarpDialect)
            {
                return GetKey(modelType);
            }
            else {
                return ReflectionHelper.GetKeyName(modelType);
            }
        }

        private string GetModelSqlKey(Type modelType, Operator operate)
        {
            return string.Format("{0}.{1}", modelType.Name, operate.ToString());
        }

        private string GetTable(Type modelType)
        {
            return string.Format(_encapsulation, ReflectionHelper.GetTableName(modelType));
        }

        private string GetKey(Type modelType)
        {
            return string.Format(_encapsulation, ReflectionHelper.GetKeyName(modelType));
        }

        private string getColumn(string name)
        {
            return string.Format(_encapsulation, name);
        }

        private void SetDataBase()
        {
            switch (this._dbType)
            {
                //case Dialect.PostgreSQL:
                //    _dialect = Dialect.PostgreSQL;
                //    _encapsulation = "{0}";
                //    _getIdentitySql = string.Format("SELECT LASTVAL() AS id");
                //    _getPagedListSql = "Select {SelectColumns} from {TableName} {WhereClause} Order By {OrderBy} LIMIT {RowsPerPage} OFFSET (({PageNumber}-1) * {RowsPerPage})";
                //    break;
                //case Dialect.SQLite:
                //    _dialect = Dialect.SQLite;
                //    _encapsulation = "{0}";
                //    _getIdentitySql = string.Format("SELECT LAST_INSERT_ROWID() AS id");
                //    _getPagedListSql = "Select {SelectColumns} from {TableName} {WhereClause} Order By {OrderBy} LIMIT {RowsPerPage} OFFSET (({PageNumber}-1) * {RowsPerPage})";
                //    break;
                case DataBaseEnum.MySql:
                    this._dbType = DataBaseEnum.MySql;
                    _encapsulation = "`{0}`";
                    // _identitySql = string.Format("SELECT LAST_INSERT_ID() AS id");
                    _identitySql = "SELECT LAST_INSERT_ID()";
                    _getPagedSql = "Select {SelectColumns} from {TableName} {WhereClause} Order By {OrderBy} LIMIT {Offset},{RowsPerPage}";
                    break;
                default:
                    this._dbType = DataBaseEnum.SqlServer;
                    _encapsulation = "[{0}]";
                    //  _identitySql = string.Format("SELECT CAST(SCOPE_IDENTITY()  AS BIGINT) AS [id]");
                    _identitySql = "SELECT SCOPE_IDENTITY()";
                    _getPagedSql = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY {OrderBy}) AS PagedNumber, {SelectColumns} FROM {TableName} {WhereClause}) AS u WHERE PagedNUMBER BETWEEN (({PageNumber}-1) * {RowsPerPage} + 1) AND ({PageNumber} * {RowsPerPage})";
                    break;
            }


        }

       
    }
}
