using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dapper.DBContext.Builder;
using Dapper.DBContext.Schema;
using Dapper.DBContext.Helper;
using Dapper.DBContext.Test.Domain;
namespace Dapper.DBContext.Test.Helper
{
    [TestClass]
    public class ReflectionHelperTest
    {


        [TestInitialize]
        public void Init()
        {

        }

        [TestMethod]
        public void GetBuildSqlProperties_test()
        {
            var target = ReflectionHelper.GetBuildSqlProperties(typeof(tb_time));
            Assert.IsNotNull(target);
            Assert.AreEqual(1, target.Count);
            Assert.IsTrue(target.Contains("Code"));
        }
        [TestMethod]
        public void GetBuildSqlProperties_Customer_Domin_test()
        {
            var target = ReflectionHelper.GetBuildSqlProperties(typeof(CustomerEntity));
            Assert.IsNotNull(target);
            Assert.AreEqual(2, target.Count);
            Assert.IsTrue(target.Contains("CustomerID"));
            Assert.IsTrue(target.Contains("CreateOn"));
        }

        [TestMethod]
        public void GetBuildSqlProperties_string_id_key_test()
        {
            var target = ReflectionHelper.GetBuildSqlProperties(typeof(Category));
            Assert.IsNotNull(target);
            Assert.AreEqual(2, target.Count);
            Assert.IsTrue(target.Contains("Id"));
            Assert.IsTrue(target.Contains("Name"));
        }

        [TestMethod]
        public void GetBuildSqlProperties_Customer_AutoID_Domin_test()
        {
            var target = ReflectionHelper.GetBuildSqlProperties(typeof(CustomerAutoIDEntity));
            Assert.IsNotNull(target);
            Assert.AreEqual(1, target.Count);
            Assert.IsTrue(target.Contains("CreateOn"));
        }

        [TestMethod]
        public void GetSelectSqlProperties_Domin_inherit_BaseEntity_test()
        {
            var target = ReflectionHelper.GetSelectSqlProperties(typeof(tb_time));
            Assert.IsNotNull(target);
            Assert.AreEqual(3, target.Count);
            Assert.IsTrue(target.Contains("Code"));
            Assert.IsTrue(target.Contains("Id"));
            Assert.IsTrue(target.Contains("RowVersion"));
        }

        [TestMethod]
        public void GetSelectSqlProperties_Customer_Domin_test()
        {
            var target = ReflectionHelper.GetSelectSqlProperties(typeof(CustomerEntity));
            Assert.IsNotNull(target);
            Assert.AreEqual(2, target.Count);
            Assert.IsTrue(target.Contains("CustomerID"));
            Assert.IsTrue(target.Contains("CreateOn"));
        }
        [TestMethod]
        public void GetSelectSqlProperties_Customer_AutoID_Domin_test()
        {
            var target = ReflectionHelper.GetSelectSqlProperties(typeof(CustomerAutoIDEntity));
            Assert.IsNotNull(target);
            Assert.AreEqual(2, target.Count);
            Assert.IsTrue(target.Contains("CustomerID"));
            Assert.IsTrue(target.Contains("CreateOn"));
        }
        [TestMethod]
        public void GetSelectSqlProperties_string_key_test()
        {
            var target = ReflectionHelper.GetSelectSqlProperties(typeof(Category));
            Assert.IsNotNull(target);
            Assert.AreEqual(4, target.Count);
            Assert.IsTrue(target.Contains("Id"));
            Assert.IsTrue(target.Contains("Name"));
        }

        [TestMethod]
        public void GetKeyName_default_Domain_test()
        {
            var target = ReflectionHelper.GetKeyName(typeof(Order));
            Assert.AreEqual("Id", target);
        }
        [TestMethod]
        public void GetKeyName_Customer_Domain_test()
        {
            var target = ReflectionHelper.GetKeyName(typeof(CustomerAutoIDEntity));
            Assert.AreEqual("CustomerID", target);
        }

        [TestMethod]
        public void GetObjectPropertyValue_TEST_calss_OBJ()
        {
            Category cat = new Category()
            {
                Id = "01",
                Name = "shp",
                FullName = "222",
                Level = 1
            };
            var target = ReflectionHelper.GetObjectPropertyValue(cat);
            Assert.AreEqual("Dapper.DBContext.Test.Domain.CategoryName : shp ;FullName : 222 ;Level : 1 ;Id : 01 ;", target);
        }

        [TestMethod]
        public void GetObjectPropertyValue_TEST_value_OBJ()
        {
            var cat = 111;
            var target = ReflectionHelper.GetObjectPropertyValue(cat);
            Assert.AreEqual("111", target);
        }

        [TestMethod]
        public void isIdentity_test()
        {
            Category cat = new Category()
            {
                Id = "01",
                Name = "shp",
                FullName = "222",
                Level = 1
            };
            var target = ReflectionHelper.isIdentity(cat.GetType());
            Assert.AreEqual(false, target);
        }
         [TestMethod]
        public void SetIdValue()
        {
            Order model = new Order()
            {
                Id = 1,
                Code = "1111"
            };

            object obj = model;

            var t= obj.GetType();
            var pi = t.GetProperty("Id", typeof(int));
            pi.SetValue(obj, 2);

            Assert.AreEqual(2, model.Id);
        }
    }
}
