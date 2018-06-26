using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
namespace Dapper.DBContext.Test.DataBase
{
    [TestClass]
   public class TransactionTest
    {
        private TransactionScope scope;

       [TestInitialize]
        public void BeginTransaction()
        {
            scope = new TransactionScope();
        }

       [TestCleanup]
        public void RollBack()
        {
            Console.WriteLine("transaction rollback ");
            scope.Dispose();
        }
    }
}
