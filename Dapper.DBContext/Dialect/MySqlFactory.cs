using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;
namespace Dapper.DBContext.Dialect
{
    public class MySqlFactory : IConnectionFactory, IDataBaseDialect
    {
         IDbConnection _connection;
        string _connectionStringName;
        public MySqlFactory(string connectionStringName)
        {
            this._connectionStringName = connectionStringName;
        }      
    

        public DataBaseEnum DataBaseType
        {
            get { throw new NotImplementedException(); }
        }

        public string WrapFormat
        {
            get { return "`{0}`"; }
        }

        public string PageFormat
        {
            get { return "Select {SelectColumns} from {TableName} {WhereClause} Order By {OrderBy} LIMIT (({PageIndex}-1) * {PageSize} + 1),{PageSize}"; }
        }

        public string IdentityFromat
        {
            get { return "SELECT LAST_INSERT_ID()"; }
        }
              
        public ISqlBuilder Builder
        {
            get { return new SqlBuilder(this); }
        }

        public IJoinQuery JoinBuilder
        {
            get { throw new NotImplementedException(); }
        }

        public override IDbConnection CreateConnection()
        {           
            if (_connection == null)
            {
                string connectionString = ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString;
                _connection = new MySqlConnection(connectionString);
            }
            return this._connection;
        }

        public override ISqlBuilder CreateBuilder()
        {
            throw new NotImplementedException();
        }

        public override IJoinQuery CreateJoinBuilder()
        {
            throw new NotImplementedException();
        }
    }
}
