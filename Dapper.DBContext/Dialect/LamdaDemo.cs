using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
namespace LamdaTest
{
    public class LamdaDemo
    {
        //public void Find<T>(Expression<Func<T, bool>> where)
        //{
        //    BinaryExpression be = GetBinaryExpression(where.Body);

        //    //List<QueryParameter> queryProperties = new List<QueryParameter>();
        //    StringBuilder builder = new StringBuilder(1000);
        //    GetWhere(be, ExpressionType.Default, list);
        //    GetWhere(be, be.NodeType, queryProperties);

        //    for (int i = 0; i < queryProperties.Count; i++)
        //    {
        //        var item = queryProperties[i];
        //        //  var columnName = BaseSqlProperties.First(x => x.Name == item.PropertyName).ColumnName;

        //        if (!string.IsNullOrEmpty(item.LinkingOperator) && i > 0)
        //        {
        //            builder.AppendFormat("{0} {1} {2} @{3} ", item.LinkingOperator, item.PropertyName, item.QueryOperator, item.PropertyValue);
        //        }
        //        else
        //        {
        //            builder.AppendFormat("{0} {1} @{2} ", item.PropertyName, item.QueryOperator, item.PropertyValue);
        //            // builder.AppendFormat($"{TableName}.{columnName} {item.QueryOperator} @{item.PropertyName} ");
        //        }

        //        // expando[item.PropertyName] = item.PropertyValue;
        //    }
        //    Console.WriteLine("sql=" + builder.ToString());
        //    Console.WriteLine();
        //}

        public void Find<T>(Expression<Func<T, bool>> where)
        {
            BinaryExpression be = GetBinaryExpression(where.Body);

            List<string> sqls = new List<string>();

            GetWhere(be, ExpressionType.Default, sqls);
            string sql="";
            foreach(var item in sqls)
            {
                sql+= item;
            }

            Console.WriteLine("sql:" + sql);
            Console.WriteLine();
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <returns>The property name for the property expression.</returns>
        public string GetPropertyName(BinaryExpression body)
        {
            string propertyName = body.Left.ToString().Split('.')[1];

            if (body.Left.NodeType == ExpressionType.Convert)
            {
                // remove the trailing ) when convering.
                propertyName = propertyName.Replace(")", string.Empty);
            }

            return propertyName;
        }

        public BinaryExpression GetBinaryExpression(Expression expression)
        {
            var binaryExpression = expression as BinaryExpression;
            var body = binaryExpression ?? Expression.MakeBinary(ExpressionType.Equal, expression, Expression.Constant(true));
            return body;
        }

        public void GetWhere(BinaryExpression body, ExpressionType linkingType, List<QueryParameter> queryProperties)
        {

            if (body.NodeType != ExpressionType.AndAlso && body.NodeType != ExpressionType.OrElse)
            {
                var propertyName = GetPropertyName(body);

                // 检查字段查询中，是否存在 基础属性
                Console.WriteLine("lamda:" + body.Left.ToString()+" nodeType:"+body.NodeType.ToString());
                // 获取值 特例： like in ，between

                var propertyValue = GetValue(body.Right);
                var opr = GetSqlOperator(body.NodeType);
                var link = GetSqlOperator(linkingType);

                if (body.Left.NodeType == ExpressionType.Call)
                {
                    MethodCallExpression callExp = body.Left as MethodCallExpression;
                    switch (callExp.Method.Name)
                    {
                        case "Like":
                            ConstantExpression pvExp = callExp.Arguments[1] as ConstantExpression;
                            propertyValue = pvExp.Value;
                            opr = "Like";
                            break;
                        default:
                            break;
                    }
                }

                // queryProperties.Add(string.Format("{0} {1} {2} {3}", propertyName, opr, propertyValue, link));
                queryProperties.Add(new QueryParameter(link, propertyName, propertyValue, opr));
            }
            else
            {
                //递归解析
                GetWhere(GetBinaryExpression(body.Left), body.NodeType, queryProperties);

                GetWhere(GetBinaryExpression(body.Right), body.NodeType, queryProperties);

            }
        }

        public void GetWhere(BinaryExpression body, ExpressionType linkingType, List<string> queryProperties)
        {

            if (body.NodeType != ExpressionType.AndAlso && body.NodeType != ExpressionType.OrElse)
            {
                var propertyName = GetPropertyName(body);

                // 检查字段查询中，是否存在 基础属性
                Console.WriteLine("lamda:" + body.Left.ToString() + " nodeType:" + body.NodeType.ToString());
                // 获取值 特例： like in ，between

                var propertyValue = GetValue(body.Right);
                var opr = GetSqlOperator(body.NodeType);
                var link = GetSqlOperator(linkingType);

                if (body.Left.NodeType == ExpressionType.Call)
                {
                    MethodCallExpression callExp = body.Left as MethodCallExpression;
                    ConstantExpression pvExp = null;
                    switch (callExp.Method.Name)
                    {
                        case "Like":
                            pvExp = callExp.Arguments[1] as ConstantExpression;
                            propertyValue = pvExp.Value;
                            opr = "Like";
                            break;
                        //case "In":
                        //     pvExp = callExp.Arguments[1] as ConstantExpression;
                        //    propertyValue = pvExp.Value;
                        //    opr = "In";
                        //    break;
                        //case "NotIn":
                        //    pvExp = callExp.Arguments[1] as ConstantExpression;
                        //    propertyValue = pvExp.Value;
                        //    opr = "NotIn";
                        //    break;
                        //case "Between":
                        //     pvExp = callExp.Arguments[1] as ConstantExpression;
                        //    propertyValue = pvExp.Value;
                        //    opr = "NotIn";
                        //    break;
                        default:
                            throw new Exception(string.Format("sql不支持此方法[{0}]",callExp.Method.Name));
                          
                    }
                }

                 queryProperties.Add(string.Format("{0} {1} {2} ", propertyName, opr, propertyValue ));
              //  queryProperties.Add(new QueryParameter(link, propertyName, propertyValue, opr));
            }
            else
            {
                //递归解析
                GetWhere(GetBinaryExpression(body.Left), body.NodeType, queryProperties);
              // 连接中间
                queryProperties.Add(string.Format(" {0} " ,GetSqlOperator(body.NodeType)));
                GetWhere(GetBinaryExpression(body.Right), body.NodeType, queryProperties);

            }
        }



        //private void FillQueryProperties(BinaryExpression body, ExpressionType linkingType, ref List<QueryParameter> queryProperties)
        //{
        //    if (body.NodeType != ExpressionType.AndAlso && body.NodeType != ExpressionType.OrElse)
        //    {
        //        var propertyName = GetPropertyName(body);

        //        if (!BaseSqlProperties.Select(x => x.Name).Contains(propertyName))
        //        {
        //            throw new NotImplementedException("predicate can't parse");
        //        }

        //        var propertyValue = GetValue(body.Right);
        //        var opr = GetSqlOperator(body.NodeType);
        //        var link = GetSqlOperator(linkingType);

        //        queryProperties.Add(new QueryParameter(link, propertyName, propertyValue, opr));
        //    }
        //    else
        //    {
        //        FillQueryProperties(GetBinaryExpression(body.Left), body.NodeType, ref queryProperties);
        //        FillQueryProperties(GetBinaryExpression(body.Right), body.NodeType, ref queryProperties);
        //    }
        //}

        public static object GetValue(Expression member)
        {
            var objectMember = Expression.Convert(member, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();
            return getter();
        }


        public static string GetSqlOperator(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.Equal:
                    return "=";

                case ExpressionType.NotEqual:
                    return "!=";

                case ExpressionType.LessThan:
                    return "<";

                case ExpressionType.LessThanOrEqual:
                    return "<=";

                case ExpressionType.GreaterThan:
                    return ">";

                case ExpressionType.GreaterThanOrEqual:
                    return ">=";

                case ExpressionType.AndAlso:
                case ExpressionType.And:
                    return "AND";

                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return "OR";

                case ExpressionType.Default:
                    return string.Empty;

                default:
                    throw new NotImplementedException();
            }
        }
    }

    public class Order
    {
        public string Code { get; set; }

        public int Id { get; set; }

        public decimal Total { get; set; }
    }

    public class QueryParameter
    {
        public string LinkingOperator { get; set; }
        public string PropertyName { get; set; }
        public object PropertyValue { get; set; }
        public string QueryOperator { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParameter" /> class.
        /// </summary>
        /// <param name="linkingOperator">The linking operator.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <param name="queryOperator">The query operator.</param>
        public QueryParameter(string linkingOperator, string propertyName, object propertyValue, string queryOperator)
        {
            LinkingOperator = linkingOperator;
            PropertyName = propertyName;
            PropertyValue = propertyValue;
            QueryOperator = queryOperator;
        }
    }

    public static class StringExtension
    {
        public static bool Like(this string name, string value)
        {
            return true;
        }

        //public static bool In(this string name, string[] values)
        //{
        //    return true;
        //}

        //public static bool NotIn(this string name, string[] values)
        //{
        //    return false;
        //}
    }

    //public static class Int32Extension
    //{
    //    public static bool In(this int name, int[] values)
    //    {
    //        return true;
    //    }

    //    public static bool NotIn(this int name, int[] values)
    //    {
    //        return false;
    //    }

    //    public static bool Between(this int name, int from, int to)
    //    {
    //        return true;
    //    }
    //}




    // sql 方言 
    //  string.like('abc%') type any in(a,b,c)   notIn(a,b,c) type any between(from,to)

    // orm  支持 sqlserver,mysql,oracle  // 提供扩展，可自己实现
    // 查询方法

    // T Find<T>(int Id)
    // T Find<T>(string Id)
    // T Find<T>(Expression<Func<TEntity, bool>> expression)
    // IEnumerable<TEntity> Find<T>(string[] Id)
    // IEnumerable<TEntity> Find<T>(int[] Id)
    // IEnumerable<TEntity> FindAll<T>();
    // IEnumerable<TEntity> FindAll<T>(Expression<Func<TEntity, bool>> expression); 
    // bool Exists<T>(Expression<Func<TEntity, bool>> expression)

    // 只提供基础查询，复杂查询，自己手写sql

    // IEnumerable<TEntity> FindPage<T>(Expression<Func<TEntity, bool>> expression,int pageSize,int pageIndex,out int totalRecords);
    // IEnumerable<TEntity> FindPage<T>(object condition,int pageSize,int pageIndex);

    // QueryCondition
    //  Name
    //  Operateor  ：  like in notIn between
    //  Value
    //  ValueType
    //  Link :  And  Or

    // 多条件，连接方式，And 还是 Or

}
