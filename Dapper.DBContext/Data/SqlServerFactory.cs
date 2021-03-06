﻿using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper.DBContext.Builder;
namespace Dapper.DBContext.Data
{
    public class SqlServerFactory :IConnectionFactory,IDataBaseDialect
    {
       
        string _connectionStringName;
        public SqlServerFactory(string connectionStringName)
        {
            this._connectionStringName = connectionStringName;
        } 
        public override IDbConnection CreateConnection()
        {           
            return new SqlConnection(ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString);
        }

        public override ISqlBuilder CreateBuilder()
        {
            return new SqlBuilder(new DialectBuilder(this));
        }

        public override IJoinQuery CreateJoinBuilder()
        {
            return new JoinQueryBuilder(new DialectBuilder(this), new ExecuteQuery(this));
        }

        public string WrapFormat
        {
            get { return "[{0}]"; }
        }
        /// <summary>
        /// SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY {OrderBy}) AS RowIndex, {SelectColumns} FROM {TableName} {TableAlias} {JoinClause} {WhereClause}) AS u WHERE RowIndex BETWEEN (({PageIndex}-1) * {PageSize} + 1) AND ({PageIndex} * {PageSize})
        /// </summary>
        public string PageFormat
        {
            get { return "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY {TableAlias}.{OrderBy}) AS RowIndex, {SelectColumns} FROM {TableName} {TableAlias} {JoinClause} where 1=1 {WhereClause}) AS u WHERE RowIndex BETWEEN (({PageIndex}-1) * {PageSize} + 1) AND ({PageIndex} * {PageSize})"; }
            //get { return "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY {OrderBy}) AS PagedNumber, {SelectColumns} FROM {TableName} {WhereClause}) AS u WHERE PagedNUMBER BETWEEN (({PageNumber}-1) * {RowsPerPage} + 1) AND ({PageNumber} * {RowsPerPage})"; }
        }

        public string IdentityFormat
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



        public string VariableFormat
        {
            get { return "@"; }
        }
    }
}
