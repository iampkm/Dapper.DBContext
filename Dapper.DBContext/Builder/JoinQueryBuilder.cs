using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.DBContext.Data;
using Dapper.DBContext.Helper;
using System.Dynamic;
using System.Linq.Expressions;

namespace Dapper.DBContext.Builder
{
   public class JoinQueryBuilder :IJoinQuery
    {
       IDialectBuilder _dialectBuilder;
       IExecuteQuery _executeQuery;
       JoinBuilderContext _joinBuilder;
       Dictionary<Type, string> aliasDic ;
       Dictionary<Type, List<string>> entityColumnDic ;
       
        public JoinQueryBuilder(IDialectBuilder dialectBuilder,IExecuteQuery executeQuery)
        {
            this._dialectBuilder = dialectBuilder;
            this._executeQuery = executeQuery;
            _joinBuilder = new JoinBuilderContext();
            aliasDic = new Dictionary<Type, string>();
            entityColumnDic = new Dictionary<Type, List<string>>();
        }

        public JoinBuilderContext JoinContext {
            get { return this._joinBuilder; }
        }
        
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

        public IEnumerable<TResult> Where<TResult>(System.Linq.Expressions.Expression<Func<TResult, bool>> expression)
        {
            object arguments = new object();
            string sqlTemplate = BuildJoinSelect(typeof(TResult));
            sqlTemplate = BuildWhere<TResult>(sqlTemplate, expression, out arguments);
            return _executeQuery.Query<TResult>(sqlTemplate, arguments);
        }
        public IEnumerable<TResult> Where<TResult>(string where, object arguments)
        {

            string sqlTemplate = BuildJoinSelect(typeof(TResult));          
            // build where
            sqlTemplate = sqlTemplate.Replace("{WhereClause}", where);
            // execute sql
            return this._executeQuery.Query<TResult>(sqlTemplate, arguments);
        }
        private string BuildJoinSelect(Type resultType)
        {
            if (this._joinBuilder == null) { throw new Exception("join builder is null"); }
            string sqlTemplate = CreateSelectTemplate();
            if (this._joinBuilder.IsPage)
            {
                sqlTemplate = CreatePageTemplate();
            }
            // build Join
            sqlTemplate = BuildTableJoin(sqlTemplate, this._joinBuilder.JoinTables);
            // build return column
            sqlTemplate = BuildReturnColumn(sqlTemplate, resultType);
            return sqlTemplate;
        }

        private string BuildTableJoin(string sqlTemplate, IList<JoinArgument> joinTables)
        {
            string joinFormat = "{JoinMethod} {TableName} {TableAlias} on {PreTableAlias}.{PreTableKey} = {TableAlias}.{TableForeignKey}";
            int index = 0;
            foreach (var entity in joinTables)
            {
                if (string.IsNullOrEmpty(entity.JoinMethod) && index == 0)
                {
                    // first table  
                    sqlTemplate = sqlTemplate.Replace("{TableName}", _dialectBuilder.GetTable(entity.EntityType));
                    sqlTemplate = sqlTemplate.Replace("{TableAlias}", entity.Alias);
                    sqlTemplate = sqlTemplate.Replace("{OrderBy}", _dialectBuilder.GetKey(entity.EntityType));
                }
                else
                {
                    var preEntity = this._joinBuilder.JoinTables[index - 1];
                    var joinSection = joinFormat.Replace("{JoinMethod}", entity.JoinMethod);
                    joinSection = joinSection.Replace("{TableName}", _dialectBuilder.GetTable(entity.EntityType));
                    joinSection = joinSection.Replace("{TableAlias}", entity.Alias);
                    joinSection = joinSection.Replace("{TableForeignKey}", _dialectBuilder.GetForeignKey(preEntity.EntityType));
                    joinSection = joinSection.Replace("{PreTableAlias}", preEntity.Alias);
                    joinSection = joinSection.Replace("{PreTableKey}", _dialectBuilder.GetKey(preEntity.EntityType));
                    joinSection += "{JoinClause}";  // 为下一个连接预留占位符

                    sqlTemplate = sqlTemplate.Replace("{JoinClause}", joinSection);

                }
                index = index + 1;
                if (!aliasDic.ContainsKey(entity.EntityType))
                {
                    aliasDic.Add(entity.EntityType, entity.Alias);
                    entityColumnDic.Add(entity.EntityType, ReflectionHelper.GetSelectSqlProperties(entity.EntityType));
                }
            }
            sqlTemplate = sqlTemplate.Replace("{JoinClause}", "");
            return sqlTemplate;
        }

        private string BuildReturnColumn(string sqlTemplate, Type resultType)
        {
            // get return column
            var columnInfos = ReflectionHelper.GetSelectSqlProperties(resultType);
            List<string> selectColumns = new List<string>();
            foreach (var columnName in columnInfos)
            {
                bool isColumnExists = false;
                foreach (var entityType in entityColumnDic.Keys)
                {
                    if (entityColumnDic[entityType].Exists(name => name.ToLower() == columnName.ToLower()))
                    {
                        selectColumns.Add(string.Format("{0}.{1}", aliasDic[entityType], _dialectBuilder.GetColumn(columnName)));
                        isColumnExists = true;
                        break;
                    }
                }
                if (!isColumnExists) { throw new Exception(string.Format("The column [{0}] does not exist in the table.", columnName)); }
            }

            sqlTemplate = sqlTemplate.Replace("{SelectColumns}", string.Join(",", selectColumns));
            return sqlTemplate;
        }

        private string BuildWhere<TResult>(string sqlTemplate, Expression<Func<TResult, bool>> expression, out object arguments)
        {
            var queryArgments = LamdaHelper.GetWhere<TResult>(expression);
            //  Dictionary<string, object> dic = new Dictionary<string, object>();
            dynamic args = new ExpandoObject();
            StringBuilder where = new StringBuilder();
           // where.Append("where ");
            // object arguments = new object();
            string template = "{TableAlias}.{ColumnName} {Operator} @{ArgumentName} {Link} ";
            foreach (QueryArgument argument in queryArgments)
            {
                ((IDictionary<string, object>)args)[argument.Name] = argument.Value;
                string temp = template.Replace("{TableAlias}", aliasDic[argument.EntityType]);
                temp = temp.Replace("{ColumnName}", _dialectBuilder.GetColumn(argument.Name));
                temp = temp.Replace("{Operator}", argument.Operator);
                temp = temp.Replace("{ArgumentName}", argument.ArgumentName);
                temp = temp.Replace("{Link}", argument.Link);
                where.Append(temp);
            }
            arguments = args;

            sqlTemplate = sqlTemplate.Replace("{WhereClause}", where.ToString());
            return sqlTemplate;
        }

        private string CreatePageTemplate()
        {
            string sqlTemplate = "";           
            // page sql
            sqlTemplate = this._dialectBuilder.DBDialect.PageFormat;
            sqlTemplate = sqlTemplate.Replace("{PageIndex}", this._joinBuilder.PageIndex.ToString());
            sqlTemplate = sqlTemplate.Replace("{PageSize}", this._joinBuilder.PageSize.ToString());
            sqlTemplate = sqlTemplate.Replace("{Offset}", ((this._joinBuilder.PageIndex - 1) * this._joinBuilder.PageSize).ToString());
            
            return sqlTemplate;           
        }

        private string CreateSelectTemplate()
        {
            return "select {SelectColumns} from {TableName} {TableAlias} {JoinClause} {WhereClause}";
        }
    }
}
