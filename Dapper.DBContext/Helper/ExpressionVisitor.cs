using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
namespace Dapper.DBContext.Helper
{
    public abstract class ExpressionVisitor
    {
        protected Type _entityType;
        protected List<string> propList = new List<string>();
        protected virtual Expression Visit(Expression exp)
        {
            if (exp == null)
                return exp;
            switch (exp.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return this.VisitUnary((UnaryExpression)exp);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    return this.VisitBinary((BinaryExpression)exp);
                case ExpressionType.TypeIs:
                    return this.VisitTypeIs((TypeBinaryExpression)exp);
                case ExpressionType.Conditional:
                    return this.VisitConditional((ConditionalExpression)exp);
                case ExpressionType.Constant:
                    return this.VisitConstant((ConstantExpression)exp);
                case ExpressionType.Parameter:
                    return this.VisitParameter((ParameterExpression)exp);
                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess((MemberExpression)exp);
                case ExpressionType.Call:
                    return this.VisitMethodCall((MethodCallExpression)exp);
                case ExpressionType.Lambda:
                    return this.VisitLambda((LambdaExpression)exp);
                case ExpressionType.New:
                    return this.VisitNew((NewExpression)exp);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return this.VisitNewArray((NewArrayExpression)exp);
                case ExpressionType.Invoke:
                    return this.VisitInvocation((InvocationExpression)exp);
                case ExpressionType.MemberInit:
                    return this.VisitMemberInit((MemberInitExpression)exp);
                case ExpressionType.ListInit:
                    return this.VisitListInit((ListInitExpression)exp);
                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.NodeType));
            }
        }





        protected virtual Expression VisitUnary(UnaryExpression node)
        {
            Console.WriteLine("VisitUnary:" + node.ToString() + " | nodeType:" + node.NodeType.ToString());
            var exp = this.Visit(node.Operand);
            if (exp != node.Operand)
            {
                return Expression.MakeUnary(node.NodeType, exp, node.Type, node.Method);
            }
            return node;
        }
        protected virtual Expression VisitBinary(BinaryExpression node)
        {
            Console.WriteLine("VisitBinary:" + node.ToString() + " | nodeType:" + node.NodeType.ToString());
            //Expression l = this.Visit(node.Left);    
            //Expression r = this.Visit(node.Right);

           // this.list.Push(node.NodeType.ToString());
            return node;
        }

        protected virtual Expression VisitTypeIs(TypeBinaryExpression node)
        {
            Console.WriteLine("VisitTypeIs:" + node.ToString());
            return node;
        }

        protected virtual Expression VisitListInit(ListInitExpression node)
        {
            Console.WriteLine("VisitListInit:" + node.ToString());
            return node;
        }

        protected virtual Expression VisitMemberInit(MemberInitExpression node)
        {


            Console.WriteLine("VisitMemberInit:" + node.ToString());
            //foreach (var exp in node.Bindings)
            //{
            //    var ma = exp as MemberAssignment;
            //    Expression.Bind(exp.Member, ma.Expression);

            //    this.Visit(ma.Expression);
            //    // this.Visit(exp as MemberInitExpression);
            //    var item = string.Format("[{0}].[{1}] = {2}", ma.Member.ReflectedType.Name, ma.Member.Name, GetValue(ma.Expression).ToString());
            //    Console.WriteLine("MemberAssignment:" + item);
            //}

            return node;
        }

        protected virtual Expression VisitInvocation(InvocationExpression node)
        {
            Console.WriteLine("VisitInvocation:" + node.ToString());
            return node;
        }

        protected virtual Expression VisitNewArray(NewArrayExpression node)
        {
            Console.WriteLine("VisitNewArray:" + node.ToString());
            return node;
        }

        protected virtual Expression VisitNew(NewExpression node)
        {
            Console.WriteLine("VisitNew:" + node.ToString());
            //for (var i = 0; i < node.Arguments.Count; i++)
            //{
            //    if (i <= 0)
            //    {
            //        Console.WriteLine(" , ");
            //    }

            //    if (node.Arguments[i] is MemberExpression)
            //    {
            //        var member = node.Arguments[i] as MemberExpression;
            //        Console.WriteLine("[{2}].{1} AS {0} ", node.Members[i].Name, member.Member.Name, member.Member.ReflectedType.Name);
            //    }
            //    if (node.Arguments[i] is ConstantExpression)
            //    {
            //        var constant = node.Arguments[i] as ConstantExpression;
            //        Console.WriteLine("{1} AS {0} ", node.Members[i].Name, constant.Value);
            //    }
            //}
            return node;
        }

        protected virtual Expression VisitLambda(LambdaExpression node)
        {
            Console.WriteLine("VisitLambda:" + node.ToString());

            var entityType = node.Parameters[0].Type;
            _entityType = entityType;
            this.propList = entityType.GetProperties().Where(pi=>pi.PropertyType.IsSimpleType()).Select(n => n.Name).ToList();
            this.Visit(node.Body);
            return node;
        }

        protected virtual Expression VisitMethodCall(MethodCallExpression node)
        {
            Console.WriteLine("VisitMethodCall:" + node.ToString());
            var exp = this.Visit(node.Object);
            return node;
        }

        protected virtual Expression VisitMemberAccess(MemberExpression node)
        {
            Console.WriteLine("VisitMemberAccess:" + node.ToString());
            //Expression exp = this.Visit(node.Expression);
            //if (exp != node.Expression)
            //{
            //    return Expression.MakeMemberAccess(exp, node.Member);
            //}
            //var mi = node.Member.Name;
            //this.list.Push(mi);
            return node;
        }

        protected virtual Expression VisitParameter(ParameterExpression node)
        {
            Console.WriteLine("VisitParameter:" + node.ToString());
            return node;
        }

        protected virtual Expression VisitConstant(ConstantExpression node)
        {
            Console.WriteLine("VisitConstant:" + node.ToString());
            //var value = node.Value.ToString();
            //this.list.Push(value);
            return node;
        }

        protected virtual Expression VisitConditional(ConditionalExpression node)
        {
            Console.WriteLine("VisitConditional:" + node.ToString());
            return node;
        }

        /// <summary>
        ///  获取表达式值
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        protected static object GetValue(Expression member)
        {
            var objectMember = Expression.Convert(member, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();
            return getter();
        }

        /// <summary>
        /// 转换操作
        /// </summary>
        /// <param name="nodeType"></param>
        /// <returns></returns>
        protected string ConvertNodeTypeToSql(ExpressionType nodeType)
        {
            string opr = "";
            switch (nodeType)
            {
                case ExpressionType.Equal:
                    opr = "=";
                    break;
                case ExpressionType.NotEqual:
                    opr = "<>";
                    break;
                case ExpressionType.GreaterThan:
                    opr = ">";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    opr = ">=";
                    break;
                case ExpressionType.LessThan:
                    opr = "<";
                    break;
                case ExpressionType.LessThanOrEqual:
                    opr = "<=";
                    break;
                case ExpressionType.AndAlso:
                case ExpressionType.And:
                    opr = "AND";
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    opr = "OR";
                    break;
                case ExpressionType.Add:
                    opr = "+";
                    break;
                case ExpressionType.Subtract:
                    opr = "-";
                    break;
                case ExpressionType.Multiply:
                    opr = "*";
                    break;
                case ExpressionType.Divide:
                    opr = "/";
                    break;
                case ExpressionType.Default:
                    opr = string.Empty;
                    break;
                default:
                    throw new NotSupportedException(nodeType + "is not supported.");
            }
            return opr;
        }
    }
}
