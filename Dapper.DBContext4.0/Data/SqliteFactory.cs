using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper.DBContext.Builder;
using System.Data;
using System.Configuration;
using System.Data.SQLite;
namespace Dapper.DBContext.Data
{
    public class SqliteFactory : IConnectionFactory, IDataBaseDialect
    {
          IDbConnection _connection;
        string _connectionStringName;
        public SqliteFactory(string connectionStringName)
        {
            this._connectionStringName = connectionStringName;
        } 

        public string WrapFormat
        {
            get { return "[{0}]"; }
        }

        public string PageFormat
        {
            get { return "Select {SelectColumns} from {TableName} {TableAlias} {JoinClause} where 1=1 {WhereClause} Order By  {TableAlias}.{OrderBy} LIMIT {Offset},{PageSize}"; }
        }

        public string IdentityFromat
        {
            get { return "SELECT LAST_INSERT_ROWID() AS id"; }
        }

        public override IDbConnection CreateConnection()
        {           
            if (_connection == null)
            {
                string connectionString = ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString;
                _connection = new SQLiteConnection(connectionString);
            }
            return this._connection;
        }

        public override ISqlBuilder CreateBuilder()
        {
            return new SqlBuilder(new DialectBuilder(this));
        }

        public override IJoinQuery CreateJoinBuilder()
        {
            return new JoinQueryBuilder( new DialectBuilder(this), new ExecuteQuery(this)); 
        }
    }
}
