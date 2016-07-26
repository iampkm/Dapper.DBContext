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
    public class SqlBuilder : ISqlBuilder, IJoinQuery
    {
        private static readonly ConcurrentDictionary<string, string> _SqlCache = new ConcurrentDictionary<string, string>();
        IDataBaseDialect _dialect;
        JoinBuilderContext _joinBuilder;
        // string _joinFormat = "{Table} {tableAlias} on {PreTableAlias}.{PreTableKey} = {tableAlias}.{TableKey}";
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

        public IJoinQuery BuildJoin<TEntity>()
        {
            this._joinBuilder = new JoinBuilderContext(false);
            var entityType = typeof(TEntity);          
            this._joinBuilder.Add(entityType);
            return this;
        }
        public IJoinQuery BuildPage<TEntity>(int pageIndex, int pageSize)
        {
            this._joinBuilder = new JoinBuilderContext(true);
            this._joinBuilder.SetPageInfo(pageIndex, pageSize);
            var entityType = typeof(TEntity);
            this._joinBuilder.Add(entityType);
            return this;
        }

        #endregion

        #region IJoinQuery Implement

        public IJoinQuery InnerJoin<TEntity>()
        {
            if (this._joinBuilder == null) { throw new Exception("InnerJoin can be called after BuildJoin or BuildPage method"); }
            this._joinBuilder.Add(typeof(TEntity), "inner join");
            return this;
        }

        public IJoinQuery LeftJoin<TEntity>()
        {
            if (this._joinBuilder == null) { throw new Exception("LeftJoin can be called after BuildJoin or BuildPage method"); }
            this._joinBuilder.Add(typeof(TEntity), "left join");
            return this;
        }

        public IJoinQuery RightJoin<TEntity>()
        {
            if (this._joinBuilder == null) { throw new Exception("RightJoin can be called after BuildJoin or BuildPage method"); }
            this._joinBuilder.Add(typeof(TEntity), "right join");
            return this;
        }

        public IEnumerable<TResult> Where<TEntity, TResult>(System.Linq.Expressions.Expression<Func<TEntity, bool>> expression)
        {
            if (this._joinBuilder == null) { throw new Exception("join builder is null"); }
            Dictionary<Type, string> aliasDic = new Dictionary<Type, string>();
            Dictionary<Type, List<string>> entityColumnDic = new Dictionary<Type, List<string>>();
            string sqlTemplate = "";
            if (this._joinBuilder.IsPage)
            {
                // page sql
                sqlTemplate = this._dialect.PageFormat;
                sqlTemplate = sqlTemplate.Replace("{PageIndex}", this._joinBuilder.PageIndex.ToString());
                sqlTemplate = sqlTemplate.Replace("{PageSize}", this._joinBuilder.PageSize.ToString());
                // replace table 
                string joinFormat = "{JoinMethod} {TableName} {TableAlias} on {PreTableAlias}.{PreTableKey} = {TableAlias}.{TableForeignKey}";
                int index = 0;
                foreach (var entity in this._joinBuilder.JoinTables)
                {
                    if (string.IsNullOrEmpty(entity.JoinMethod) && index == 0)
                    {
                        // first table  
                        sqlTemplate = sqlTemplate.Replace("{TableName}", GetTable(entity.EntityType));
                        sqlTemplate = sqlTemplate.Replace("{TableAlias}", entity.Alias);
                        sqlTemplate = sqlTemplate.Replace("{OrderBy}", GetKeyName(entity.EntityType,true));
                    }
                    else
                    {
                        var preEntity = this._joinBuilder.JoinTables[index - 1];
                        var joinSection = joinFormat.Replace("{JoinMethod}", entity.JoinMethod);
                        joinSection = joinSection.Replace("{TableName}", GetTable(entity.EntityType));
                        joinSection = joinSection.Replace("{TableAlias}", entity.Alias);
                        joinSection = joinSection.Replace("{TableForeignKey}", GetForeignKey(preEntity.EntityType, true));
                        joinSection = joinSection.Replace("{PreTableAlias}", preEntity.Alias);
                        joinSection = joinSection.Replace("{PreTableKey}", GetKeyName(preEntity.EntityType, true));
                        joinSection += "{JoinClause}";  // 为下一个连接预留占位符

                        sqlTemplate = sqlTemplate.Replace("{JoinClause}", joinSection);

                    }
                    index = index + 1;
                    if (!aliasDic.ContainsKey(entity.EntityType))
                    {
                        aliasDic.Add(entity.EntityType, entity.Alias);
                        entityColumnDic.Add(entity.EntityType, ReflectionHelper.GetPropertyInfos(entity.EntityType).Select(n => n.Name).ToList());
                    }
                }
                sqlTemplate = sqlTemplate.Replace("{JoinClause}", "");
            }
            else
            {
                //not page
            }
            // get return column
            var columnInfos = ReflectionHelper.GetPropertyInfos(typeof(TResult));
            List<string> selectColumns = new List<string>();
            foreach (var column in columnInfos)
            {
                foreach (var entityType in entityColumnDic.Keys)
                {
                    if (entityColumnDic[entityType].Exists(name => name.ToLower() == column.Name.ToLower()))
                    {
                        selectColumns.Add(string.Format("{0}.{1}", aliasDic[entityType], getColumn(column.Name)));
                        break;
                    }
                }
            }

            sqlTemplate = sqlTemplate.Replace("{SelectColumns}", string.Join(",", selectColumns));


            var queryArgments = LamdaHelper.GetWhere<TEntity>(expression);
            //  Dictionary<string, object> dic = new Dictionary<string, object>();
            dynamic args = new ExpandoObject();
            StringBuilder where = new StringBuilder();
            where.Append("where ");
            object arguments = new object();
            string template = "{TableAlias}.{ColumnName} {Operator} @{ArgumentName} {Link}";
            foreach (QueryArgument argument in queryArgments)
            {
                ((IDictionary<string, object>)args)[argument.Name] = argument.Value;
                string temp = template.Replace("{TableAlias}", aliasDic[argument.EntityType]);
                temp = temp.Replace("{ColumnName}", getColumn(argument.Name));
                temp = temp.Replace("{Operator}", argument.Operator);
                temp = temp.Replace("{ArgumentName}", argument.ArgumentName);
                temp = temp.Replace("{Link}", argument.Link);
                where.Append(temp);
            }
            arguments = args;

            sqlTemplate = sqlTemplate.Replace("{WhereClause}", where.ToString());

            IEnumerable<TResult> lll = new List<TResult>();
            return lll;
            //connection.Open();
            //var result = connection.Query<TResult>(sql.ToString(), arguments);
            //connection.Close();
            // return result;

            // return sql.ToString().Trim();
        }

        public IEnumerable<TResult> Where<TEntity1, TEntity2, TResult>(System.Linq.Expressions.Expression<Func<TEntity1, TEntity2, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TResult> Where<TEntity1, TEntity2, TEntity3, TResult>(System.Linq.Expressions.Expression<Func<TEntity1, TEntity2, TEntity3, bool>> expression)
        {
            throw new NotImplementedException();
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
