using Dapper.DBContext.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.DBContext.Data;
namespace Dapper.DBContext.Builder
{
    public class DialectBuilder : IDialectBuilder
    {
        IDataBaseDialect _dialect;

        public DialectBuilder(IDataBaseDialect dialect)
        {
            this._dialect = dialect;
        }

        public IDataBaseDialect DBDialect
        {
            get
            {
               return this._dialect;
            }
        }     

        public string GetKey(Type modelType, bool isWrapDialect = true)
        {
            string key = ReflectionHelper.GetKeyName(modelType);
            if (isWrapDialect)
            {
                key = string.Format(this._dialect.WrapFormat, key);
            }
            return key;
        }

        public string GetForeignKey(Type modelType, bool isWrapDialect = true)
        {
            string foreignKey = ReflectionHelper.GetTableName(modelType) + ReflectionHelper.GetKeyName(modelType);
            if (isWrapDialect)
            {
                foreignKey = string.Format(this._dialect.WrapFormat, foreignKey);
            }
            return foreignKey;
        }

        public string GetTable(Type modelType, bool isWrapDialect = true)
        {
            string table = ReflectionHelper.GetTableName(modelType);
            if (isWrapDialect)
            {
                table = string.Format(this._dialect.WrapFormat, table);
            }
            return table;
        }

        public string GetColumn(string columnName, bool isWrapDialect = true)
        {
            if (isWrapDialect)
            {
                columnName = string.Format(this._dialect.WrapFormat, columnName);
            }
            return columnName;
        }
    }
}
