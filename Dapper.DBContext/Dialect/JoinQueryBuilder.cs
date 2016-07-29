using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.DBContext.Transaction;
using Dapper.DBContext.Helper;
using System.Dynamic;
namespace Dapper.DBContext.Dialect
{
   public class JoinQueryBuilder :IJoinQuery
    {
      // IDataBaseDialect _dialect;
       IDialectBuilder _dialectBuilder;
       IExecuteQuery _executeQuery;
       JoinBuilderContext _joinBuilder;
       
        public JoinQueryBuilder(IDialectBuilder dialectBuilder,IExecuteQuery executeQuery)
        {
           // this._dialect = dialect;
            this._dialectBuilder = dialectBuilder;
            this._executeQuery = executeQuery;
            _joinBuilder = new JoinBuilderContext();
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

        public IEnumerable<TResult> Where<TEntity, TResult>(System.Linq.Expressions.Expression<Func<TEntity, bool>> expression)
        {
            if (this._joinBuilder == null) { throw new Exception("join builder is null"); }
            Dictionary<Type, string> aliasDic = new Dictionary<Type, string>();
            Dictionary<Type, List<string>> entityColumnDic = new Dictionary<Type, List<string>>();
            string sqlTemplate = "";
            if (this._joinBuilder.IsPage)
            {
                // page sql
                sqlTemplate = this._dialectBuilder.DBDialect.PageFormat;
                sqlTemplate = sqlTemplate.Replace("{PageIndex}", this._joinBuilder.PageIndex.ToString());
                sqlTemplate = sqlTemplate.Replace("{PageSize}", this._joinBuilder.PageSize.ToString());                         
            }
            else
            {
                sqlTemplate = "select {SelectColumns} from {TableName} {TableAlias} {JoinClause} {WhereClause}";
            }
            // build join 
            // replace table 
            string joinFormat = "{JoinMethod} {TableName} {TableAlias} on {PreTableAlias}.{PreTableKey} = {TableAlias}.{TableForeignKey}";      
            int index = 0;
            foreach (var entity in this._joinBuilder.JoinTables)
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
                    entityColumnDic.Add(entity.EntityType, ReflectionHelper.GetPropertyInfos(entity.EntityType).Select(n => n.Name).ToList());
                }
            }
            sqlTemplate = sqlTemplate.Replace("{JoinClause}", "");


            // get return column
            var columnInfos = ReflectionHelper.GetSelectSqlProperties(typeof(TResult));
            List<string> selectColumns = new List<string>();
            foreach (var columnName in columnInfos)
            {
                foreach (var entityType in entityColumnDic.Keys)
                {
                    if (entityColumnDic[entityType].Exists(name => name.ToLower() == columnName.ToLower()))
                    {
                        selectColumns.Add(string.Format("{0}.{1}", aliasDic[entityType], _dialectBuilder.GetColumn(columnName)));
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

            IEnumerable<TResult> lll = new List<TResult>();
           
            return  _executeQuery.Query<TResult>(sqlTemplate,arguments);
        }

        public IEnumerable<TResult> Where<TEntity1, TEntity2, TResult>(System.Linq.Expressions.Expression<Func<TEntity1, TEntity2, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TResult> Where<TEntity1, TEntity2, TEntity3, TResult>(System.Linq.Expressions.Expression<Func<TEntity1, TEntity2, TEntity3, bool>> expression)
        {
            throw new NotImplementedException();
        }

      
    }
}
