using Dapper.DBContext.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext.Helper
{
   public class SelectBuilder:ExpressionVisitor
    {
        protected Dictionary<string, object> _args;
        protected List<string> _columns;
        IDialectBuilder _dialect;
        public SelectBuilder(IDialectBuilder dialect)
        {
            _dialect = dialect;
            _args = new Dictionary<string, object>();
            _columns = new List<string>();
        }
        public string BuildSelect(Expression expression)
        {
            this.Visit(expression);
            var sql = "";
            for (var i = 0; i < this._columns.Count; i++)
            {
                var column = this._columns[i];
                if (i <= 0)
                {
                    sql += column;
                }
                else
                {
                    sql += string.Format(",{0}", column);
                }
            }

            return sql;
        }
        //public string BuildUpdateColumn(Expression expression)
        //{
        //    this.Visit(expression);
        //    var sql = "";
        //    for (var i = 0; i < this._columns.Count; i++)
        //    {
        //        var column = this._columns[i];
        //        if (i <= 0)
        //        {
        //            sql += string.Format("{0} = {1}", column.MemberName, column.ArgumentName);
        //        }
        //        else
        //        {
        //            sql += string.Format(",{0} = {1}", column.MemberName, column.ArgumentName);
        //        }
        //    }

        //    return sql;
        //}


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
                this._columns.Add(_dialect.GetColumn(columnName));
                // this._convertElements.Push(string.Format("[{0}].[{1}]", this._entityType.Name, node.Member.Name));
            }
            else
            {
                // this._convertElements.Push(GetValue(node).ToString());
               // PushValue("{0}", GetValue(node));
            }


            return node;
        }

        protected override Expression VisitNew(NewExpression node)
        {
            for (var i = 0; i < node.Arguments.Count; i++)
            {
                var arg = node.Arguments[i];
                // 参数
                var filed = "";
                if (arg is MemberExpression)
                {
                    var argMember = arg as MemberExpression;
                    var columnName = ReflectionHelper.GetColumnName(argMember.Member.Name, this._entityType);
                    filed = _dialect.GetColumn(columnName);
                }
                else
                {
                    filed = GetValue(arg).ToString();
                }

                // 成员
                this._columns.Add(string.Format("{0} AS {1}", filed, node.Members[i].Name));
            }
            return node;
        }
    }
}
