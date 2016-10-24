using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dapper.DBContext.Test.Domain;
using System.Linq.Expressions;
using Dapper.DBContext.Helper;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace Dapper.DBContext.Test.Helper
{
    [TestClass]
    public class LamdaHelperTest
    {
        [TestMethod]
        public void ParseExpresss()
        {
            Expression<Func<OrderItem, decimal>> exp = o => (o.Price + o.Quantity) / o.Quantity;
           // Expression<Func<OrderItem, decimal>> exp = o => o.Price + o.Quantity;
           // Expression<Func<OrderItem, decimal>> exp = o => o.Price ;

            string expstring = exp.Body.ToString();
            // ((o.Price + Convert(o.Quantity)) / Convert(o.Quantity))
            string pattern = @"Convert\((.*?)\)";
            Regex rgx = new Regex(pattern);
           string fun = rgx.Match(expstring).Value;
           fun = fun.Replace("Convert(", "").Replace(")","");
           // var result = GetValue(expstring,)
          
            rgx.Replace(expstring, "");

          //  Expression<Func<OrderItem, Order>> exp2 = o => new Order() {  Address = 1, Code = 2 };
            foreach (ParameterExpression pe in exp.Parameters)
            {
                expstring = expstring.Replace(pe.Name+".", "");             
            }
            expstring = expstring.Replace("((", "(");
            expstring = expstring.Replace("))", ")");
            expstring = expstring.Replace("Convert(", "");
            
           // parenthesizedexpression
               // par
            string expect = "(Price + Quantity) / Quantity";
            //               (Price + Quantity) / Quantity)
            Assert.AreEqual(expect, expstring);

        }

        public static string GetValue(string str, string s, string e)
        {
            Regex rg = new Regex("(?<=(" + s + "))[.\\s\\S]*?(?=(" + e + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            return rg.Match(str).Value;
        }
        [TestMethod]
        public void TestWhere()
        {
            string value ="234"+"%";
            Expression<Func<Category, bool>> exp = n => n.Id.Like(value) && n.Level == 2;
            var result= LamdaHelper.GetWhere<Category>(exp);
            Assert.AreEqual("11", "11");
        }

        [TestMethod]
        public void TestWhere_constraint()
        {           
            Category cat = new Category()
            {
                Id = "01",
                Name = "shp",
                FullName = "222",
                Level = 1
            };
            Expression<Func<Category, bool>> exp = n => n.Id.Like(cat.Id+"%") && n.Level == cat.Level;
            var result = LamdaHelper.GetWhere<Category>(exp);
            Assert.AreEqual("11", "11");
        }
            
    }
}
