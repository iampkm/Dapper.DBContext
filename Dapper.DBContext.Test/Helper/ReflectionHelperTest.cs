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
    }
}
