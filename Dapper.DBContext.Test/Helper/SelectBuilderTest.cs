using Dapper.DBContext.Builder;
using Dapper.DBContext.Data;
using Dapper.DBContext.Helper;
using Dapper.DBContext.Test.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext.Test.Helper
{
     [TestClass]
   public class SelectBuilderTest
    {
        SelectBuilder _builder;

        [TestInitialize]
        public void Init()
        {
            IDataBaseDialect msSqlDialect = new SqlServerFactory("O2O");
            IDialectBuilder mssqlDialectBuilder = new DialectBuilder(msSqlDialect);
            _builder = new SelectBuilder(mssqlDialectBuilder);
        }

        [TestMethod]
        public void BuildSelect_One_column_string()
        {
            //Arrange
            Expression<Func<Entity_Select_Column_Test, string>> predicate = n => n.Code;
            //Action
            var actual = _builder.BuildSelect(predicate).Trim();
            // Assert
            var expected = "[Code]";
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void BuildSelect_One_column_datetime()
        {
            //Arrange
            Expression<Func<Entity_Select_Column_Test, DateTime>> predicate = n => n.CreatedOn;
            //Action
            var actual = _builder.BuildSelect(predicate).Trim();
            // Assert
            var expected = "[CreatedOn]";
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void BuildSelect_One_column_enum()
        {
            //Arrange
            Expression<Func<Entity_Select_Column_Test, OrderStatus>> predicate = n => n.Status;
            //Action
            var actual = _builder.BuildSelect(predicate).Trim();
            // Assert
            var expected = "[Status]";
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void BuildSelect_many_column()
        {
            //Arrange
            Expression<Func<Entity_Select_Column_Test, object>> predicate = n => new { n.Id, n.Code, n.Quantity };
            //Action
            var actual = _builder.BuildSelect(predicate).Trim();
            // Assert
            var expected = "[Id] AS Id,[Code] AS Code,[Quantity] AS Quantity";
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void BuildSelect_many_column_init()
        {
            //Arrange
            Expression<Func<Entity_Select_Column_Test, object>> predicate = n => new { OrderId = n.Id, OrderCode = n.Code, Quantity = n.Quantity, n.Price };
            //Action
            var actual = _builder.BuildSelect(predicate).Trim();
            // Assert
            var expected = "[Id] AS OrderId,[Code] AS OrderCode,[Quantity] AS Quantity,[Price] AS Price";
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void BuildSelect_many_column_init_constant()
        {
            //Arrange
            Expression<Func<Entity_Select_Column_Test, object>> predicate = n => new { OrderId = 1, OrderCode = n.Code, Quantity = n.Quantity, n.Price };
            //Action
            var actual = _builder.BuildSelect(predicate).Trim();
            // Assert
            var expected = "1 AS OrderId,[Code] AS OrderCode,[Quantity] AS Quantity,[Price] AS Price";
            Assert.AreEqual(expected, actual);
                       
        }
        
    }
}
