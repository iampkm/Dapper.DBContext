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
using System.Data;
using Dapper;
namespace Dapper.DBContext.Dialect
{
    public class SqlBuilder : ISqlBuilder
    {
        private static readonly ConcurrentDictionary<string, string> _SqlCache = new ConcurrentDictionary<string, string>();
        IDataBaseDialect _dialect;
        string _pageSql = "";
        public SqlBuilder(IDataBaseDialect dialect)
        {
            this._dialect = dialect;
            this._pageSql = dialect.PageFormat;
        }
        #region ISqlBuilder Implement
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
            var columns = string.Join(",", properties.Select(p => string.Format(this._dialect.WrapFormat, p)));
            var sql = string.Format("insert into {0} ({1}) values ({2}) {3}", table, columns, values, this._dialect.IdentityFromat);
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
            var updateProperties = ReflectionHelper.GetBuildSqlProperties(modelType);
            var updateFields = string.Join(",", updateProperties.Select(p => string.Format(this._dialect.WrapFormat, p) + " = @" + p));
            var whereFields = string.Empty;
            var rowVersion = "";
            if (ReflectionHelper.GetPropertyInfos(modelType).Exists(p => p.Name == "RowVersion" && p.PropertyType == typeof(byte[])))
            {
                rowVersion = string.Format("and [RowVersion]=@RowVersion");
            }
            whereFields = string.Format("where {0}=@{1} {2}", GetKey(modelType), ReflectionHelper.GetKeyName(modelType), rowVersion);
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
            var sql = string.Format("delete from {0} where {1}=@{2} {3}", table, GetKey(modelType), ReflectionHelper.GetKeyName(modelType), rowVersion);
            _SqlCache[sqlKey] = sql;
            return sql;
        }

        public string BuildWhere<TEntity>(System.Linq.Expressions.Expression<Func<TEntity, bool>> expression, out object arguments) where TEntity : IEntity
        {
            var queryArgments = LamdaHelper.GetWhere<TEntity>(expression);
            //  Dictionary<string, object> dic = new Dictionary<string, object>();
            dynamic args = new ExpandoObject();
            StringBuilder sql = new StringBuilder();
            sql.Append("where ");
            foreach (QueryArgument argument in queryArgments)
            {
                ((IDictionary<string, object>)args)[argument.Name] = argument.Value;
                sql.AppendFormat("{0} {1} @{2} {3} ", getColumn(argument.Name), argument.Operator, argument.ArgumentName, argument.Link);
            }
            arguments = args;

            return sql.ToString().Trim();
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
                columns = string.Join(",", properties.Select(p => string.Format(this._dialect.WrapFormat, p)));
            }
            return string.Format("select {0} from {1} ", columns, table);
        }
        public string GetKeyName(Type modelType, bool isWrapDialect)
        {
            if (isWrapDialect)
            {
                return GetKey(modelType);
            }
            else
            {
                return ReflectionHelper.GetKeyName(modelType);
            }
        }
       

        #endregion
    

        #region private method

        private string GetModelSqlKey(Type modelType, Operator operate)
        {
            return string.Format("{0}.{1}", modelType.Name, operate.ToString());
        }

        private string GetTable(Type modelType)
        {
            return string.Format(this._dialect.WrapFormat, ReflectionHelper.GetTableName(modelType));
        }

        private string GetForeignKey(Type modelType, bool isWrapDialect)
        {
            string foreignKey = ReflectionHelper.GetTableName(modelType) + GetKeyName(modelType, false); ;
            if (isWrapDialect)
            {
                foreignKey = string.Format(this._dialect.WrapFormat, foreignKey);
            }
            return foreignKey;
        }

        private string GetKey(Type modelType)
        {
            return string.Format(this._dialect.WrapFormat, ReflectionHelper.GetKeyName(modelType));
        }

        private string getColumn(string name)
        {
            return string.Format(this._dialect.WrapFormat, name);
        }

        #endregion



    }
}
