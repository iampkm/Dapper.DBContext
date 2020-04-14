using Dapper.DBContext.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext.Helper
{
    public class SelectUpdateBuilder : ExpressionVisitor
    {
        protected Dictionary<string, object> _args;
        protected List<string> _columns;
        IDialectBuilder _dialect;
        public SelectUpdateBuilder(IDialectBuilder dialect)
        {
            _dialect = dialect;
            _args = new Dictionary<string, object>();
            _columns = new List<string>();
        }
        public string BuildSelect(Expression expression, out Dictionary<string, object> arguments)
        {
            this._entityType = expression.Type.GetGenericArguments().FirstOrDefault();  // 第一个参数就是返回类型
            if (_entityType == null) throw new Exception("参数异常");
            // 设置查询实体
            this.propList = _entityType.GetProperties().Where(pi => pi.PropertyType.IsSimpleType()).Select(n => n.Name).ToList();
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
            arguments = _args;
            return sql;
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            foreach (var binding in node.Bindings)
            {
                if (binding.BindingType != MemberBindingType.Assignment)
                {
                    throw new NotSupportedException("成员赋值类型不支持");
                }
                var memberAssignment = binding as MemberAssignment;
                // 参数
                var value = GetValue(memberAssignment.Expression);
                if (value != null && value.GetType().IsEnum)  // 枚举要转换成int 型
                {
                    value = (int)value;
                }
                // 成员
                var columnName = ReflectionHelper.GetColumnName(memberAssignment.Member.Name, this._entityType);
                var column = _dialect.GetColumn(columnName);
                var argName = GetArgumentName(columnName);
                this._columns.Add(string.Format("{0} = {1}", column, argName));
                PutValue(argName, value);
            }
            return node;
        }

        private string GetArgumentName(string columnName)
        {
            return string.Format("{0}{1}", this._dialect.DBDialect.VariableFormat, columnName);
        }

        public void PutValue(string argumentName, object value)
        {
            if (!this._args.ContainsKey(argumentName))
            {
                this._args.Add(argumentName, value);
            }
        }
    }
}
