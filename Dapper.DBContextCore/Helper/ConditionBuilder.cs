using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Dapper.DBContext.Builder;
using System.Reflection;
using System.Diagnostics;

namespace Dapper.DBContext.Helper
{
    public class ConditionBuilder : ExpressionVisitor
    {
        protected Stack<string> _convertElements;
        protected Dictionary<string, object> _args;
        IDialectBuilder _dialect;
      
        public ConditionBuilder(IDialectBuilder dialect)
        {
            _dialect = dialect;
            _convertElements = new Stack<string>();
            _args = new Dictionary<string, object>();
        }
        

        public string BuildWhere(Expression expression, out  Dictionary<string, object> arguments)
        {
            this._entityType = expression.Type.GetGenericArguments().FirstOrDefault();  // 第一个参数就是返回类型
            if (_entityType == null) throw new Exception("参数异常");
            // 设置查询实体
            this.propList = _entityType.GetProperties().Where(pi => pi.PropertyType.IsSimpleType()).Select(n => n.Name).ToList();
            this.Visit(expression);
            var sql = "";
            var first = true;
            while (this._convertElements.Count != 0)
            {
                if (first)
                {
                    sql += this._convertElements.Pop();
                    first = false;
                }
                else {
                    sql += " " + this._convertElements.Pop() ;
                }               
            }
            arguments = this._args;
            return sql;
        }

        protected override Expression VisitLambda(LambdaExpression node)
        {
            Console.WriteLine("VisitLambda:" + node.ToString());

            //var entityType = node.Parameters[0].Type;
            //_entityType = entityType;
            //this.propList = entityType.GetProperties().Where(pi => pi.PropertyType.IsSimpleType()).Select(n => n.Name).ToList();
            this.Visit(node.Body);
            return node;
        }

        /// <summary>
        ///  二元表达式
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            var operate = ConvertNodeTypeToSql(node.NodeType);
        
            // 构建带NULL 的语句
            if (IsNullExpression(node))
            {
                this._convertElements.Push("IS NULL");
            }
            else if (IsNotNullExpression(node)) {
                this._convertElements.Push("IS NOT NULL");
            }
            else
            {
                this.Visit(node.Right);
                this._convertElements.Push(operate);
            }          
            this.Visit(node.Left);                    

            return node;
        }



        /// <summary>
        ///  一元表达式
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitUnary(UnaryExpression node)
        {
            return base.VisitUnary(node);
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            return base.VisitMemberInit(node);
        }
        /// <summary>
        ///  成员访问
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMemberAccess(MemberExpression node)
        { 
            var result = GetValue(node);
            if (result == null)
            {
                    var columnName = ReflectionHelper.GetColumnName(node.Member.Name, this._entityType);
                   this._convertElements.Push(_dialect.GetColumn(columnName));  
            }
            else {
                   PushValue("{0}", result); 
            }

            return node;
        }
        /// <summary>
        ///  方法调用
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            switch (node.Method.Name)
            {
                case "Like":
                    PushValue("LIKE {0}", GetValue(node.Arguments[1]));
                    this.Visit(node.Arguments[0]);
                    break;
                case "In":
                    PushValue("IN {0}", GetValue(node.Arguments[1]));
                    this.Visit(node.Arguments[0]);
                    break;
                case "Between":
                    var from = GetValue(node.Arguments[1]);
                    var to = GetValue(node.Arguments[2]);
                    PushValue("BETWEEN {0} AND {1}", from, to);
                    this.Visit(node.Arguments[0]);
                    break;
                default:
                    // 非约定方法,计算方法执行结果
                    // object result = Expression.Lambda(node).Compile().DynamicInvoke();
                    object result = GetValue(node);
                    PushValue("{0}", result);
                    break;
            }
            return node;
        }

        /// <summary>
        ///  常量表达式
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value == null)
            {
                this._convertElements.Push("NULL");
            }
            else
            {
                PushValue("{0}", node.Value);
            }

            return node;
        }        

        private string GetArgumentName()
        {
            return string.Format("{0}P{1}", this._dialect.DBDialect.VariableFormat, this._args.Count.ToString());
        }

        /// <summary>
        ///Like,In,Between 将值放入队列
        /// </summary>
        /// <param name="formate">Like,In,Between</param>
        /// <param name="value"></param>
        private void PushValue(string formate, params object[] value)
        {
            // 有一个值，就生成一个参数名
            List<string> argumentNames = new List<string>();
            foreach (var item in value)
            {
                var argumentName = GetArgumentName();
                argumentNames.Add(argumentName);
                if (!this._args.ContainsKey(argumentName))
                {
                    this._args.Add(argumentName, item);
                }
            }
            //参数变量放入堆栈
            this._convertElements.Push(string.Format(formate, argumentNames.ToArray()));
        }
    }
}
