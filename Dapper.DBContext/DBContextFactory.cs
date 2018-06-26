using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Dapper.DBContext.Builder;
using Dapper.DBContext.Data;
namespace Dapper.DBContext
{
   public class DBContextFactory
    {
        public static IDBContext Create()
        {
            string masterConnectionString = "";
          //  string readConnectionString = "";
            IConnectionFactory commandConnection = IConnectionFactory.Create(masterConnectionString);
          //  IConnectionFactory queryConnection = IConnectionFactory.Create(masterConnectionString);          
           // IQuery query = new QueryService(readConnectionString);
            IDBContext db = new DapperDBContext(masterConnectionString);
            return db;
        }
    }
}
