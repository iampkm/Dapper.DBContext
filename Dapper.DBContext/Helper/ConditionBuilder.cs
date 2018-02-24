using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Dapper.DBContext.Builder;
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

        

        /// <summary>
        ///  二元表达式
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            var operate = ConvertNodeTypeToSql(node.NodeType);
            Console.WriteLine("VisitBinary:" + node.ToString() + " | nodeType:" + node.NodeType.ToString());

            Console.WriteLine("VisitBinary- right: nodeType= " + node.Right.NodeType.ToString());
            this.Visit(node.Right);
            this._convertElements.Push(operate);
            this.Visit(node.Left);

            //this.Visit(node.Left);
            //this.Visit(node.Right);

            //string right = this.convertElements.Pop();
            //string left = this.convertElements.Pop();

            //string condition = String.Format(" {0} {1} {2} ", left, operate, right);
            //this.convertElements.Push(condition);



            return node;
            // return base.VisitBinary(node);
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
            var memberName = node.Member.Name;
            if (this.propList.Count > 0 && this.propList.Contains(memberName))
            {
                var columnName = ReflectionHelper.GetColumnName(node.Member.Name, this._entityType);
                this._convertElements.Push(_dialect.GetColumn(columnName));
                // this._convertElements.Push(string.Format("[{0}].[{1}]", this._entityType.Name, node.Member.Name));
            }
            else
            {
                // this._convertElements.Push(GetValue(node).ToString());
                PushValue("{0}", GetValue(node));
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
            if (node.Method.Name == "Like")
            {
                PushValue("LIKE {0}", GetValue(node.Arguments[1]));
                this.Visit(node.Arguments[0]);
            }

            if (node.Method.Name == "In")
            {
                PushValue("IN {0}", GetValue(node.Arguments[1]));

                this.Visit(node.Arguments[0]);
            }
            if (node.Method.Name == "Between")
            {
                var from = GetValue(node.Arguments[1]);
                var to = GetValue(node.Arguments[2]);

                PushValue("BETWEEN {0} AND {1}", from, to);

                // this._convertElements.Push(string.Format("BETWEEN {0} AND {1}", from, to));

                this.Visit(node.Arguments[0]);
            }

            if (node.Object != null)
            {
                this.Visit(node.Object);
            }
            return node;
            //return base.VisitMethodCall(node);
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
                //var argumentName = GetArgumentName();
                //this._args.Add(argumentName, node.Value);
                //this._convertElements.Push(argumentName);
                // this._convertElements.Push(node.Value.ToString());
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
