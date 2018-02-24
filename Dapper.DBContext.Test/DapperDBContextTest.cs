using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dapper.DBContext.Test.Domain;
namespace Dapper.DBContext.Test
{
    [TestClass]
    public class DapperDBContextTest
    {
        IDBContext _db;
        [TestInitialize]
        public void Init()
        {
            _db = new DapperDBContext("masterDB");
        }



        [TestMethod]
        public void Insert_ParentClassAndChildClass()
        {
            Order model = new Order()
            {
                Code = "abc123",
                Address =  new Address("cq","sha ping ba"),
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
            model.addressList.Add(new Address("city", "area") );

            _db.Insert<Order>(model);
            _db.SaveChange();

            Assert.AreEqual("1,", "22");
        }
    }
}
