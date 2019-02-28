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



        //[TestMethod]
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
            _db.Insert(model);
            _db.SaveChange();

            Assert.AreEqual("1,", "22");
        }
        // [TestMethod]
        //public void ExistsTest()
        //{
        //    Order model = new Order()
        //    {
        //        Code = "abc123",
        //        Address = new Address("cq", "sha ping ba"),
        //        CreateAt = DateTime.Now,
        //        CreateBy = 1,
        //        Status = OrderState.Paid,
        //         Id  = 1
        //    };
        //    Order item = new Order();
        //    item.Id = 2;
        //    var result=  _db.Table<Order>().Exists(n => n.Id == item.Id);
        //    Assert.AreEqual(false, result);
        //}

        // [TestMethod]
        // public void Where_SelectTest()
        // {
        //     Order model = new Order()
        //     {
        //         Code = "abc123",
        //         Address = new Address("cq", "sha ping ba"),
        //         CreateAt = DateTime.Now,
        //         CreateBy = 1,
        //         Status = OrderState.Paid
        //     };
        //     var code = "123";
        //     var result = _db.Table<Order>().Where(n => n.Code == code).Select(n => n.Id).ToList();
        //     Assert.AreEqual(false, result);
        // }

        [TestCleanup]
         public void Rollback()
         { 
            
         }
    }
}
