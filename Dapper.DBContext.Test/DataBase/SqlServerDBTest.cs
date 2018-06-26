using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dapper.DBContext.Test.Domain;
namespace Dapper.DBContext.Test.DataBase
{
    [TestClass]
    public class SqlServerDBTest // :TransactionTest
    {
        IDBContext _db;
        [TestInitialize]
        public void Init()
        {
            _db = new DapperDBContext("dbtest");
        }


        [TestMethod]
        public void Insert_ParentClassAndChildClass()
        {
            Order model = new Order()
            {
                Code = "abc123",             
                CreateAt = DateTime.Now,
                CreateBy = 1,
                Status = OrderState.Paid
            };
            OrderItem item = new OrderItem()
            {
                OrderId = 0,
                Id = 1,
                Price = 12.33m,
                ProductId = 11,
                ProductName = "haha",
                Quantity = 10
            };
            model.Items.Add(item);          

            _db.Insert<Order>(model);
            _db.SaveChange();

            Assert.AreEqual("1,", "1");
        }
    }
}
