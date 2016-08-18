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
using System.Linq.Expressions;
namespace Dapper.DBContext.Builder
{
    public class SqlBuilder : ISqlBuilder
    {
        private static readonly ConcurrentDictionary<string, string> _SqlCache = new ConcurrentDictionary<string, string>();

        IDialectBuilder _dialectBuilder;
        public SqlBuilder(IDialectBuilder dialect)
        {
            this._dialectBuilder = dialect;
        }
        #region ISqlBuilder Implement
        public string BuildInsert(Type modelType)
        {
            var sqlKey = GetModelSqlKey(modelType, Operator.Insert);
            if (_SqlCache.ContainsKey(sqlKey))
            {
                return _SqlCache[sqlKey];
            }
            string table = this._dialectBuilder.GetTable(modelType);
            var properties = ReflectionHelper.GetBuildSqlProperties(modelType);
            var values = string.Join(",", properties.Select(name => "@" + name));
            var columns = string.Join(",", properties.Select(name => this._dialectBuilder.GetColumn(name)));
            var sql = string.Format("insert into {0} ({1}) values ({2}) {3}", table, columns, values, this._dialectBuilder.DBDialect.IdentityFromat);
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
            string table = this._dialectBuilder.GetTable(modelType);
            var updateProperties = ReflectionHelper.GetBuildSqlProperties(modelType);
            var updateFields = string.Join(",", updateProperties.Select(name => this._dialectBuilder.GetColumn(name) + " = @" + name));
            var whereFields = string.Empty;
            var rowVersion = "";
            if (ReflectionHelper.GetPropertyInfos(modelType).Exists(p => p.Name == "RowVersion" && p.PropertyType == typeof(byte[])))
            {
                rowVersion = string.Format("and [RowVersion]=@RowVersion");
            }
            whereFields = string.Format("where {0}=@{1} {2}", this._dialectBuilder.GetKey(modelType), this._dialectBuilder.GetKey(modelType, false), rowVersion);
            var sql = string.Format("update {0} set {1} {2}", table, updateFields, whereFields);
            _SqlCache[sqlKey] = sql;
            return sql;
        }

        public string BuildDelete(Type modelType)
        {
            var sqlKey = GetModelSqlKey(modelType, Operator.Delete);
            if (_SqlCache.ContainsKey(sqlKey))
            {
                return _SqlCache[sqlKey];
            }
            string table = this._dialectBuilder.GetTable(modelType);
            var rowVersion = "";
            if (ReflectionHelper.GetPropertyInfos(modelType).Exists(p => p.Name == "RowVersion" && p.PropertyType == typeof(byte[])))
            {
                rowVersion = string.Format("and [RowVersion]=@RowVersion");
            }
            var sql = string.Format("delete from {0} where {1}=@{2} {3}", table, this._dialectBuilder.GetKey(modelType), this._dialectBuilder.GetKey(modelType, false), rowVersion);
            _SqlCache[sqlKey] = sql;
            return sql;
        }

        public string buildSelectById<TEntity>(bool isOnlyOneId = true)
        {
            var Operation = "in";
            var sqlKey = GetModelSqlKey(typeof(TEntity), Operator.SelectByIdArray);
            if (isOnlyOneId)
            {
                Operation = "=";
                sqlKey = GetModelSqlKey(typeof(TEntity), Operator.SelectById);
            }

            if (_SqlCache.ContainsKey(sqlKey))
            {
                return _SqlCache[sqlKey];
            }

            string table = this._dialectBuilder.GetTable(typeof(TEntity));
            string columnNames = GetColumnNames(typeof(TEntity));
            string key = this._dialectBuilder.GetKey(typeof(TEntity));
            string keyParameter = this._dialectBuilder.GetKey(typeof(TEntity), false);
            string sql = string.Format("select {0} from {1} where {2} {3} @{4}", columnNames, table, key, Operation, keyParameter);
            _SqlCache[sqlKey] = sql;
            return sql;
        }

        public string BuildSelectByLamda<TEntity>(System.Linq.Expressions.Expression<Func<TEntity, bool>> expression, out object arguments, string columns = "")
        {
            string columnNames = string.IsNullOrEmpty(columns) == true ? GetColumnNames(typeof(TEntity)) : columns;
            string table = this._dialectBuilder.GetTable(typeof(TEntity));
            string where = "";
            if (expression != null)
            {
                where = string.Format("where {0}", BuildWhere<TEntity>(expression, out arguments));
            }
            else {
                arguments = new object();
            }          
            string sql = string.Format("select {0} from {1} {2}", columnNames, table, where);
            return sql;
        }

        public string BuildSelectByLamda<TEntity, TResult>(Expression<Func<TEntity, bool>> expression, out object arguments, Expression<Func<TEntity, TResult>> select, string function)
        {
            List<string> columnExpression = new List<string>();
            LamdaHelper.ParseColumn(select.Body, columnExpression, "");
            string columnNames = string.Format("{0}({1})", function, string.Join(" ", columnExpression).Trim());
            string table = this._dialectBuilder.GetTable(typeof(TEntity));
            string where = "";
            if (expression != null)
            {
                where = string.Format("where {0}", BuildWhere<TEntity>(expression, out arguments));
            }
            else
            {
                arguments = new object();
            }         
            string sql = string.Format("select {0} from {1} {2}", columnNames, table, where);
            return sql;
        }

        public string buildSelect<TEntity>()
        {
            var sqlKey = GetModelSqlKey(typeof(TEntity), Operator.SelectAll);
            if (_SqlCache.ContainsKey(sqlKey))
            {
                return _SqlCache[sqlKey];
            }
            string columnNames = GetColumnNames(typeof(TEntity));
            string table = this._dialectBuilder.GetTable(typeof(TEntity));
            string sql = string.Format("select {0} from {1} ", columnNames, table);
            _SqlCache[sqlKey] = sql;
            return sql;
        }

        private string BuildWhere<TEntity>(System.Linq.Expressions.Expression<Func<TEntity, bool>> expression, out object arguments)
        {
            var queryArgments = LamdaHelper.GetWhere<TEntity>(expression);
            dynamic args = new ExpandoObject();
            StringBuilder sql = new StringBuilder();
            foreach (QueryArgument argument in queryArgments)
            {
                ((IDictionary<string, object>)args)[argument.ArgumentName] = argument.Value;
                sql.AppendFormat("{0} {1} @{2} {3} ", this._dialectBuilder.GetColumn(argument.Name), argument.Operator, argument.ArgumentName, argument.Link);
            }
            arguments = args;

            return sql.ToString().Trim();
        }



        #endregion


        #region private method

        private string GetColumnNames(Type entityType)
        {
            var properties = ReflectionHelper.GetSelectSqlProperties(entityType);
            string columns = string.Join(",", properties.Select(name => this._dialectBuilder.GetColumn(name)));
            return columns;
        }

        private string GetModelSqlKey(Type modelType, Operator operate)
        {
            return string.Format("{0}.{1}", modelType.Name, operate.ToString());
        }

        #endregion


    }
}
