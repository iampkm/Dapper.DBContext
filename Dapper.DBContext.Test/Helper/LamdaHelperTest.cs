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
    }
}
