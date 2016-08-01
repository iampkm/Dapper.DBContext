using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.DBContext.Builder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dapper.DBContext.Test.Domain;
namespace Dapper.DBContext.Test
{
    [TestClass]
    public class CustomerQueryServiceTest : QueryService
    {
        public CustomerQueryServiceTest(string connectionStringName)
            : base(connectionStringName)
        { 
        
        }

        IQuery _iquery;
        [TestInitialize]
        public void Init()
        {
            _iquery = new CustomerQueryServiceTest("");
        }
        [TestMethod]
        public void FindById_Test()
        {
           var model=  _iquery.Find<Order>(1);
           
           Assert.AreEqual(1, model.Id);
           Assert.AreEqual("123", model.Code);
        }
    }


}
