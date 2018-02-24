using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Dapper.DBContext.Builder;
namespace Dapper.DBContext.Data
{
    public class OracleFactory : IConnectionFactory, IDataBaseDialect
    {
        string _connectionStringName;
        public OracleFactory(string connectionStringName)
        {
            this._connectionStringName = connectionStringName;
        } 
        public override IDbConnection CreateConnection()
        {           
            return new OracleConnection(ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString);
        }

        public string WrapFormat
        {
            get { return "\"{0}\""; }
        }

        public string PageFormat
        {
            get { throw new NotImplementedException(); }
        }

        public string IdentityFormat
        {
            get { return ""; }
        }

        public string VariableFormat
        {
            get { return ":"; }
        }





        public override ISqlBuilder CreateBuilder()
        {
            throw new NotImplementedException();
        }

        public override Builder.IJoinQuery CreateJoinBuilder()
        {
            throw new NotImplementedException();
        }
    }
}
