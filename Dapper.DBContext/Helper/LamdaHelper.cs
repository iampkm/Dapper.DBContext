using Dapper.DBContext.Dialect;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext.Helper
{
   public class LamdaHelper
    {

       public static List<QueryArgument> GetWhere<T>(Expression<Func<T, bool>> where)
       {
           var bExpr = GetBinaryExpression(where.Body);
           List<QueryArgument> sqls = new List<QueryArgument>();
           GetWhere(bExpr, sqls);

            return sqls;
       }

       public static void GetWhere(BinaryExpression body,  List<QueryArgument> queryProperties,string link="")
       {
          
           if (body.NodeType != ExpressionType.AndAlso && body.NodeType != ExpressionType.OrElse)
           {
               var propertyName = GetPropertyName(body);

               // 检查字段查询中，是否存在 基础属性
              
               var propertyValue = GetValue(body.Right);
               var opr = GetSqlOperator(body.NodeType);
              

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
                       default:
                           throw new Exception(string.Format("sql不支持此方法[{0}]", callExp.Method.Name));

                   }
               }
               var entityType = body.Left.Type;
               if (body.Left.NodeType == ExpressionType.Parameter)
               {
                   ParameterExpression paraExp = body.Left as ParameterExpression;
                   entityType = paraExp.Type;
               }
               // 变量参数名          
                var argumentName = propertyName;
                if (queryProperties.Exists(n => n.Name.Contains(propertyName)))
                {
                    argumentName += queryProperties.Count(n => n.Name.Contains(propertyName)).ToString();
                }

                queryProperties.Add(new QueryArgument(entityType, propertyName, argumentName, propertyValue, opr, link));

                //if (propertyValue is string || propertyValue is DateTime)
                //{
                //    queryProperties.Add(string.Format("{0} {1} '{2}'", propertyName, opr, propertyValue));
                //}
                //else {
                //    queryProperties.Add(string.Format("{0} {1} {2}", propertyName, opr, propertyValue));
                //}              
           }
           else
           {
               //递归解析
               GetWhere(GetBinaryExpression(body.Left),  queryProperties, GetSqlOperator(body.NodeType));
           
               GetWhere(GetBinaryExpression(body.Right),  queryProperties);

           }
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
                   return "and";

               case ExpressionType.Or:
               case ExpressionType.OrElse:
                   return "or";

               case ExpressionType.Default:
                   return string.Empty;

               default:
                   throw new NotImplementedException();
           }
       }

       public static object GetValue(Expression member)
       {
           var objectMember = Expression.Convert(member, typeof(object));
           var getterLambda = Expression.Lambda<Func<object>>(objectMember);
           var getter = getterLambda.Compile();
           return getter();
       }

       public static BinaryExpression GetBinaryExpression(Expression expression)
       {
           var binaryExpression = expression as BinaryExpression;
           var body = binaryExpression ?? Expression.MakeBinary(ExpressionType.Equal, expression, Expression.Constant(true));
           return body;
       }

       /// <summary>
       /// Gets the name of the property.
       /// </summary>
       /// <param name="body">The body.</param>
       /// <returns>The property name for the property expression.</returns>
       public static string GetPropertyName(BinaryExpression body)
       {
           string propertyName = body.Left.ToString().Split('.')[1];

           if (body.Left.NodeType == ExpressionType.Convert)
           {
               // remove the trailing ) when convering.
               propertyName = propertyName.Replace(")", string.Empty);
           }

           return propertyName;
       }

      
    }
}
