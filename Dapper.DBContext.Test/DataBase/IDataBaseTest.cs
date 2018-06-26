using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dapper.DBContext.Data;

namespace Dapper.DBContext.Test.DataBase
{
    [TestClass]
    public class IDataBaseTest
    {
        DapperDBContext _db;
        [TestInitialize]
        public void Init()
        {
            _db = new DapperDBContext("dbtest");
        }

        [TestMethod]
        public void ExecuteScalar_Test_Open()
        {
            
            var row= _db.DataBase.ExecuteScalar<int>("select count(*) from [Order]");
            row = _db.DataBase.ExecuteScalar<int>("select count(*) from [Order]");
            Assert.AreEqual(1, row);
        }
    }
}
