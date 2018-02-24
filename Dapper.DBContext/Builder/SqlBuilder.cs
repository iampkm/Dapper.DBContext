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
        ConditionBuilder _lamdaBuilder;
        SelectBuilder _selectBuilder;
        SelectUpdateBuilder _selectUpdateBuilder;
        /// <summary>
        ///  mysql,sql=@   oracle = :
        /// </summary>
        string _AT;
        string _RowVersion;
        public SqlBuilder(IDialectBuilder dialect)
        {
            this._dialectBuilder = dialect;
            this._AT = _dialectBuilder.DBDialect.VariableFormat;
            _RowVersion = Settings.Timestamp;
            _lamdaBuilder = new ConditionBuilder(dialect);
            _selectBuilder = new SelectBuilder(dialect);
            _selectUpdateBuilder = new SelectUpdateBuilder(dialect);


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
            var values = string.Join(",", properties.Select(name => _AT + name));
            var columns = string.Join(",", properties.Select(name => this._dialectBuilder.GetColumn(name)));
            var sql = string.Format("INSERT INTO {0} ({1}) VALUES ({2});{3}", table, columns, values, this._dialectBuilder.DBDialect.IdentityFormat);
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
            var updateFields = string.Join(",", updateProperties.Select(name => BuilderConditionalEquality(_dialectBuilder.GetColumn(name), _AT, name)));
            var rowVersion = "";
            if (ReflectionHelper.ExistsRowVersion(modelType))
            {
                rowVersion = " AND " + BuilderConditionalEquality(this._dialectBuilder.GetColumn(_RowVersion), _AT, _RowVersion);                
            }
            var idCondition = BuilderConditionalEquality(this._dialectBuilder.GetKey(modelType), _AT, this._dialectBuilder.GetKey(modelType, false));

            var sql = string.Format("UPDATE {0} SET {1} WHERE {2}", table, updateFields, idCondition+rowVersion);
            _SqlCache[sqlKey] = sql;
            return sql;
        }

        public string BuildUpdate<TEntity>(TEntity entity, Expression<Func<TEntity, bool>> where, out object argumnets)
        {
            var table = this._dialectBuilder.GetTable(typeof(TEntity));
            // columns
            var updateProperties = ReflectionHelper.GetBuildSqlProperties(typeof(TEntity));
            var updateFields = string.Join(",", updateProperties.Select(name => BuilderConditionalEquality(_dialectBuilder.GetColumn(name), _AT, name)));
            // where
            Dictionary<string, object> queryArgments = new Dictionary<string, object>();
            var condition = _lamdaBuilder.BuildWhere(where, out queryArgments);
            var sql = string.Format("UPDATE {0} SET {1} WHERE {2} ", table, updateFields, condition);
            // Merger args
            var columnInfos = ReflectionHelper.GetUpdateColumns(typeof(TEntity));
            foreach (var pi in columnInfos)
            {
                var key = _dialectBuilder.DBDialect.VariableFormat + ReflectionHelper.GetColumnName(pi);
                if (!queryArgments.ContainsKey(key))
                {
                    object value = pi.PropertyType.IsEnum ? (int)pi.GetValue(entity) : pi.GetValue(entity);
                    queryArgments.Add(key, value);
                }
            }
            argumnets = queryArgments;
            return sql;
        }

        public string BuildUpdate<TEntity>(Expression<Func<TEntity, TEntity>> columns, Expression<Func<TEntity, bool>> where, out object argumnets)
        {
            var table = this._dialectBuilder.GetTable(typeof(TEntity));
            // set column
            Dictionary<string, object> columnsArgments = new Dictionary<string, object>();
            var updateFields = _selectUpdateBuilder.BuildSelect(columns, out columnsArgments);
            Dictionary<string, object> queryArgments = new Dictionary<string, object>();
            var condition = _lamdaBuilder.BuildWhere(where, out queryArgments);
            var args = columnsArgments.Concat(queryArgments).ToDictionary(n => n.Key, n => n.Value);
            argumnets = args;
            var sql = string.Format("UPDATE {0} SET {1} WHERE {2} ", table, updateFields, condition);
            return sql;
        }

        public string BuildDelete(Type modelType, bool isOnlyOneId = true)
        {
            var sqlKey = GetModelSqlKey(modelType, Operator.Delete);
            if (_SqlCache.ContainsKey(sqlKey))
            {
                return _SqlCache[sqlKey];
            }
            string table = this._dialectBuilder.GetTable(modelType);
            var rowVersion = "";
            if (ReflectionHelper.ExistsRowVersion(modelType))
            {
                rowVersion = " AND " + BuilderConditionalEquality(this._dialectBuilder.GetColumn(_RowVersion), _AT, _RowVersion);              
            }
            var operate = isOnlyOneId ? "=" : "IN";
            var idCondition = BuilderConditionalEquality(this._dialectBuilder.GetKey(modelType), _AT, this._dialectBuilder.GetKey(modelType, false), operate);
            var sql = string.Format("DELETE FROM {0} WHERE {1}", table, idCondition+rowVersion);
            //暂时只缓存  =  号的删除sql
            if (isOnlyOneId) { _SqlCache[sqlKey] = sql; }
            return sql;
        }



        public string BuildDeleteByLamda<TEntity>(Expression<Func<TEntity, bool>> expression, out object arguments)
        {
            string table = this._dialectBuilder.GetTable(typeof(TEntity));            
            var where = string.Format("WHERE {0}", BuildWhere<TEntity>(expression, out arguments));
            var sql = string.Format("DELETE FROM {0} {1}", table, where);
            return sql;
        }

        public string BuildSelectById<TEntity>(bool isOnlyOneId = true)
        {
            var operation = "IN";
            var sqlKey = GetModelSqlKey(typeof(TEntity), Operator.SelectByIdArray);
            if (isOnlyOneId)
            {
                operation = "=";
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
            var condition = BuilderConditionalEquality(key, _AT, keyParameter, operation);
            string sql = string.Format("SELECT {0} FROM {1} WHERE {2}", columnNames, table, condition);
            _SqlCache[sqlKey] = sql;
            return sql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <param name="arguments"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public string BuildSelectByLamda<TEntity>(System.Linq.Expressions.Expression<Func<TEntity, bool>> expression, out object arguments, string columns = "")
        {
            string columnNames = string.IsNullOrEmpty(columns) == true ? GetColumnNames(typeof(TEntity)) : columns;
            string table = this._dialectBuilder.GetTable(typeof(TEntity));
            string where = "";
            if (expression != null)
            {
                where = string.Format("WHERE {0}", BuildWhere<TEntity>(expression, out arguments));
            }
            else
            {
                arguments = new object();
            }
            string sql = string.Format("SELECT {0} FROM {1} {2}", columnNames, table, where);
            return sql;
        }

        public string BuildSelectByLamda<TEntity, TResult>(Expression<Func<TEntity, bool>> expression, out object arguments, Expression<Func<TEntity, TResult>> select, string function)
        {
            List<string> columnExpression = new List<string>();           
            string columnNames = _selectBuilder.BuildSelect(select);


            string table = this._dialectBuilder.GetTable(typeof(TEntity));
            string where = "";
            if (expression != null)
            {
                where = string.Format("WHERE {0}", BuildWhere<TEntity>(expression, out arguments));
            }
            else
            {
                arguments = new object();
            }
            string sql = string.Format("SELECT {0} FROM {1} {2}", columnNames, table, where);
            return sql;
        }

        public string BuildSelect<TEntity>()
        {
            var sqlKey = GetModelSqlKey(typeof(TEntity), Operator.SelectAll);
            if (_SqlCache.ContainsKey(sqlKey))
            {
                return _SqlCache[sqlKey];
            }
            string columnNames = GetColumnNames(typeof(TEntity));
            string table = this._dialectBuilder.GetTable(typeof(TEntity));
            string sql = string.Format("SELECT {0} FROM {1}", columnNames, table);
            _SqlCache[sqlKey] = sql;
            return sql;
        }
        public string BuildSelect<TEntity>(string columns)
        {
            if (string.IsNullOrEmpty(columns)) { return BuildSelect<TEntity>(); }
            string table = this._dialectBuilder.GetTable(typeof(TEntity));
            string sql = string.Format("SELECT {0} FROM {1}", columns, table);  
            return sql;
        }

        private string BuildWhere<TEntity>(System.Linq.Expressions.Expression<Func<TEntity, bool>> expression, out object arguments)
        {
            //var queryArgments = LamdaHelper.GetWhere<TEntity>(expression);
            //dynamic args = new ExpandoObject();
            //StringBuilder sql = new StringBuilder();
            //foreach (QueryArgument argument in queryArgments)
            //{
            //    ((IDictionary<string, object>)args)[argument.ArgumentName] = argument.Value;
            //    sql.AppendFormat("{0} {1} @{2} {3} ", this._dialectBuilder.GetColumn(argument.Name), argument.Operator, argument.ArgumentName, argument.Link);
            //}
            //arguments = args;

            //return sql.ToString().Trim();
            Dictionary<string, object> queryArgments = new Dictionary<string, object>();
            var condition = _lamdaBuilder.BuildWhere(expression, out queryArgments);
            // arguments = queryArgments as ExpandoObject();
            dynamic args = new ExpandoObject();
            args = queryArgments;
            arguments = args;
            return condition;
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

        /// <summary>
        /// 条件等式  left = @right
        /// </summary>
        /// <param name="left"></param>
        /// <param name="variableFormat"></param>
        /// <param name="right"></param>
        /// <param name="operate"></param>
        /// <returns></returns>
        private string BuilderConditionalEquality(string left, string variableFormat, string right, string operate = "=")
        {
            return string.Format("{0} {1} {2}{3}", left, operate, variableFormat, right);
        }

        #endregion




        public string BuildSelectByContext<TEntity>(QueryContext context, out object arguments)
        {
            string where = "";
            string select = "";
            string order = "";
            Dictionary<string, object> queryArgments = new Dictionary<string, object>();
            foreach (var query in context.Nodes)
            {
                if (query.NodeType == QueryNodeType.Where)
                {
                    var condition = _lamdaBuilder.BuildWhere(query.Condition, out queryArgments);
                    where += string.IsNullOrEmpty(where) ? " " + condition : " AND " + condition;
                }
                if (query.NodeType == QueryNodeType.Select)
                {
                    select += _selectBuilder.BuildSelect(query.Condition);
                }
                if (query.NodeType == QueryNodeType.OrderBy)
                {
                    order += string.IsNullOrEmpty(order) ? _selectBuilder.BuildSelect(query.Condition) : _selectBuilder.BuildSelect(query.Condition) + ",";
                }
                if (query.NodeType == QueryNodeType.OrderByDesc)
                {
                    order += string.IsNullOrEmpty(order) ? _selectBuilder.BuildSelect(query.Condition) : _selectBuilder.BuildSelect(query.Condition) + " DESC,";
                }
            }

            var table = _dialectBuilder.GetTable(typeof(TEntity));
            if (string.IsNullOrEmpty(select))
            {

                select = GetColumnNames(typeof(TEntity));
            }
            var sql = "SELECT " + select + " FROM " + table;
            if (!string.IsNullOrEmpty(where))
            {
                sql += " WHERE" + where;
            }
            if (!string.IsNullOrEmpty(order))
            {
                sql += " ORDER BY " + order;
            }
            dynamic args = new ExpandoObject();
            args = queryArgments;
            arguments = args;
            return sql;
        }
    }
}
