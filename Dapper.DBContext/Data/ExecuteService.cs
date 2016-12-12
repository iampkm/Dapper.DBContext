using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Dapper.DBContext.Data
{
   public class ExecuteService:IExecute
    {
        IConnectionFactory _connectionFactory;
        IDbConnection _connection;
        IUnitOfWork _uow;
        public ExecuteService(IConnectionFactory connectionFactory, IUnitOfWork uow)
        {
            this._connectionFactory = connectionFactory;
            this._uow = uow;
        }

        public int Execute(string sql, object param = null)
        {
            this._connection = this._connectionFactory.CreateConnection();
            this._connection.Open();
            print(sql);
            var result = this._connection.Execute(sql, param);
            this._connection.Close();
            return result;
        }

        private void print(string msg)
        {
            if (Debugger.IsAttached)
                Trace.WriteLine(msg);
        }


        public void AddExecute(string sql, object param = null)
        {
            this._uow.Add(sql, param);
        }
    }
}
