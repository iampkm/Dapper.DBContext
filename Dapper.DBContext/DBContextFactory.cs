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
            IDbConnection writeConnection = new SqlConnection(masterConnectionString);
            IDbConnection readConnection = new SqlConnection(readConnectionString);
            IUnitOfWork iuow = new UnitOfWork("");
            ISqlBuilder ibuilder = new SqlBuilder(DataBaseEnum.SqlServer);
            IQuery query = new QueryService(readConnection, ibuilder);
            DapperDBContext db = new DapperDBContext(query,iuow,ibuilder);
            return db;
        }
    }
}
