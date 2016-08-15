using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dapper.DBContext.Test.Domain;
using System.Linq.Expressions;
using Dapper.DBContext.Helper;
using System.Collections.Generic;
namespace Dapper.DBContext.Test.Helper
{
    [TestClass]
    public class LamdaHelperTest
    {
        [TestMethod]
        public void ParseExpresss()
        {
            Expression<Func<Order, int>> exp = o => o.Id;
            Expression<Func<Order, int>> exp1 = o => o.Id * o.Id;
            Expression<Func<Order, int>> exp2 = o => o.Id * o.Id + o.Id / o.Id;
            Expression<Func<Order, int>> exp3 = o => o.Id * o.Id + o.Id ;
            // sum<Order, int>(o => o.Id * o.Id);    
            var t = exp.Body as MemberExpression;
            
            // var be= Dapper.DBContext.Helper.LamdaHelper.GetBinaryExpression(exp);
           // exp.Body is MemberExpression
          //  var body = LamdaHelper.GetBinaryExpression2(exp3.Body);
            List<string> list = new List<string>();
            LamdaHelper.ParseExp(exp2.Body, list, "");

            Assert.AreEqual(1, 1);
        }
    }
}
