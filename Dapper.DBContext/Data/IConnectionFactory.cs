using System;
using System.Configuration;
using System.Data;
using Dapper.DBContext.Builder;
namespace Dapper.DBContext.Data
{
    public abstract class IConnectionFactory
    {
        public static IConnectionFactory Create(string connectionStringName)
        {
            string provider = ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName;
            if (string.IsNullOrEmpty(provider)) { throw new Exception("ProviderName is empty"); }
            IConnectionFactory factory = null;
            switch (provider)
            {
                case "System.Data.SqlClient":
                    factory =new SqlServerFactory(connectionStringName);
                    break;
                case "MySql.Data.MySqlClient":
                    factory = new MySqlFactory(connectionStringName);
                    break;
                default:
                    factory = new SqlServerFactory(connectionStringName);
                    break;
            }
            return factory;
        }
        public abstract IDbConnection CreateConnection();
        public abstract ISqlBuilder CreateBuilder();
        public abstract IJoinQuery CreateJoinBuilder();
    }
}
