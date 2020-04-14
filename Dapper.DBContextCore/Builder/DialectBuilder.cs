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
            return isWrapDialect ? string.Format(this._dialect.WrapFormat, key) : key;
        }

        public string GetForeignKey(Type modelType, bool isWrapDialect = true)
        {
            string foreignKey = ReflectionHelper.GetTableName(modelType) + ReflectionHelper.GetKeyName(modelType);
            return isWrapDialect ? string.Format(this._dialect.WrapFormat, foreignKey) : foreignKey;            
        }

        public string GetTable(Type modelType, bool isWrapDialect = true)
        {
            string table = ReflectionHelper.GetTableName(modelType);
            return isWrapDialect ? string.Format(this._dialect.WrapFormat, table) : table;
        }

        public string GetColumn(string columnName, bool isWrapDialect = true)
        {
            return isWrapDialect ? string.Format(this._dialect.WrapFormat, columnName) : columnName;            
        }


       
    }
}
