using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Dapper.DBContext.Dialect;
using Dapper.DBContext.Transaction;
namespace Dapper.DBContext
{
   public class DBContextFactory
    {
        public static IDBContext Create()
        {
            string masterConnectionString = "";
            string readConnectionString = "";
            IConnectionFactory commandConnection = new ConnectionFactory(masterConnectionString);
            IConnectionFactory queryConnection = new ConnectionFactory(masterConnectionString);          
            IQuery query = new QueryService(readConnectionString);
            IDBContext db = new DapperDBContext(masterConnectionString,query);
            return db;
        }
    }
}
