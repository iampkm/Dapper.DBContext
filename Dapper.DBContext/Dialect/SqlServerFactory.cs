using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext.Dialect
{
   public class SqlServerFactory :IConnectionFactory,IDataBaseDialect
    {
        IDbConnection _connection;
        string _connectionStringName;
        public SqlServerFactory(string connectionStringName)
        {
            this._connectionStringName = connectionStringName;
        } 
        public override IDbConnection CreateConnection()
        {
            if (_connection == null)
            {
                _connection=new SqlConnection(_connectionStringName);
            }
            return _connection;
        }

        public override ISqlBuilder CreateBuilder()
        {
            return new SqlBuilder(this);
        }

        public override IJoinQuery CreateJoinBuilder()
        {
            throw new NotImplementedException();
        }

        public DataBaseEnum DataBaseType
        {
            get { return DataBaseEnum.SqlServer; }
        }

        public string WrapFormat
        {
            get { return "[{0}]"; }
        }

        public string PageFormat
        {
            get { return "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY {OrderBy}) AS RowIndex, {SelectColumns} FROM {TableName} {TableAlias} {JoinClause} {WhereClause}) AS u WHERE RowIndex BETWEEN (({PageIndex}-1) * {PageSize} + 1) AND ({PageIndex} * {PageSize})"; }
            //get { return "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY {OrderBy}) AS PagedNumber, {SelectColumns} FROM {TableName} {WhereClause}) AS u WHERE PagedNUMBER BETWEEN (({PageNumber}-1) * {RowsPerPage} + 1) AND ({PageNumber} * {RowsPerPage})"; }
        }

        public string IdentityFromat
        {
            get { return "SELECT SCOPE_IDENTITY()"; }
        }


        //case Dialect.PostgreSQL:
        //    _dialect = Dialect.PostgreSQL;
        //    _encapsulation = "{0}";
        //    _getIdentitySql = string.Format("SELECT LASTVAL() AS id");
        //    _getPagedListSql = "Select {SelectColumns} from {TableName} {WhereClause} Order By {OrderBy} LIMIT {RowsPerPage} OFFSET (({PageNumber}-1) * {RowsPerPage})";
        //    break;
        //case Dialect.SQLite:
        //    _dialect = Dialect.SQLite;
        //    _encapsulation = "{0}";
        //    _getIdentitySql = string.Format("SELECT LAST_INSERT_ROWID() AS id");
        //    _getPagedListSql = "Select {SelectColumns} from {TableName} {WhereClause} Order By {OrderBy} LIMIT {RowsPerPage} OFFSET (({PageNumber}-1) * {RowsPerPage})";
        //    break;
      
    }
}
