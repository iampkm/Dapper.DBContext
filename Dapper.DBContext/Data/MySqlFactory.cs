using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;
using Dapper.DBContext.Builder;
namespace Dapper.DBContext.Data
{
    public class MySqlFactory : IConnectionFactory, IDataBaseDialect
    {        
        string _connectionStringName;
        public MySqlFactory(string connectionStringName)
        {
            this._connectionStringName = connectionStringName;
        } 

        public string WrapFormat
        {
            get { return "`{0}`"; }
        }

        public string PageFormat
        {
            get { return "Select {SelectColumns} from {TableName} {TableAlias} {JoinClause} where 1=1 {WhereClause} Order By  {TableAlias}.{OrderBy} LIMIT {Offset},{PageSize}"; }
        }

        public string IdentityFromat
        {
            get { return "SELECT LAST_INSERT_ID()"; }
        }

        public override IDbConnection CreateConnection()
        {
            return new MySqlConnection(ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString);
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
