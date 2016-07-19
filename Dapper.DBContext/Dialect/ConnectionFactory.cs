using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
namespace Dapper.DBContext.Dialect
{
   public class ConnectionFactory :IConnectionFactory
    {
        IDbConnection _connection;
        DataBaseEnum _databaseType;
        string _connectionStringName;

        public ConnectionFactory(string connectionStringName)
        {
            this._connectionStringName = connectionStringName;
        }       

        public IDbConnection Create()
        {
            string connectionString = ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString;
            string provider = ConfigurationManager.ConnectionStrings[_connectionStringName].ProviderName;
            if (_connection == null)
            {
                switch (provider)
                {
                    case "System.Data.SqlClient":
                        this._connection = new SqlConnection(connectionString);
                        this._databaseType = DataBaseEnum.SqlServer;
                        break;
                    case "MySql.Data.MySqlClient":
                        _connection = new MySqlConnection(connectionString);
                        this._databaseType = DataBaseEnum.MySql;
                        break;
                    default:
                        _connection = new SqlConnection(connectionString);
                        this._databaseType = DataBaseEnum.SqlServer;
                        break;
                }
            }
            return _connection;          
        }

        public ISqlBuilder Builder
        {
            get { return new SqlBuilder(this._databaseType); }
        }
    }
}
