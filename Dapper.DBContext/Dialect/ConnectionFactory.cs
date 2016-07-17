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
   public class ConnectionFactory
    {
       static IDbConnection _connection;
      
        public static IDbConnection CreateConnection(string connectionStringName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            string provider = ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName;
            if (_connection == null)
            {
                switch (provider)
                {
                    case "System.Data.SqlClient":
                        _connection = new SqlConnection(connectionString);
                        break;
                    case "MySql.Data.MySqlClient ":
                        _connection = new MySqlConnection(connectionString);
                        break;
                    default:
                        _connection = new SqlConnection(connectionString);
                        break;
                }
            }
            return _connection;          
        }
    }
}
