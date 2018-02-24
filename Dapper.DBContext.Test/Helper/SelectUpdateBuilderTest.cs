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
   public class SelectUpdateBuilderTest
    {
        SelectUpdateBuilder _builder;

        [TestInitialize]
        public void Init()
        {
            IDataBaseDialect msSqlDialect = new SqlServerFactory("O2O");
            IDialectBuilder mssqlDialectBuilder = new DialectBuilder(msSqlDialect);
            _builder = new SelectUpdateBuilder(mssqlDialectBuilder);
        }

        [TestMethod]
        public void BuildSelect_many_column_update()
        {
            //Arrange
            var qty = 10;
            Expression<Func<Entity_Select_Column_Test, Entity_Select_Column_Test>> predicate = n => new Entity_Select_Column_Test() { Code = "123", CreatedOn = DateTime.Parse("2018-02-06"), Quantity = qty + 10, Status = OrderStatus.Paid };
            Dictionary<string, object> args = new Dictionary<string,object>();
            //Action
            var actual = _builder.BuildSelect(predicate,out args).Trim();
            // Assert
            var expected = "[Code] = @Code,[CreatedOn] = @CreatedOn,[Quantity] = @Quantity,[Status] = @Status";
            Assert.AreEqual(expected, actual);
            // args
            Assert.AreEqual("123", args["@Code"]);
            Assert.AreEqual(DateTime.Parse("2018-02-06"), args["@CreatedOn"]);
            Assert.AreEqual(20, args["@Quantity"]);
            Assert.AreEqual(2, args["@Status"]);
        }
        [TestMethod]
        public void BuildSelect_One_column_enum()
        {
            //Arrange
            Expression<Func<Entity_Select_Column_Test, Entity_Select_Column_Test>> predicate = n => new Entity_Select_Column_Test() {  Status = OrderStatus.Paid };
            Dictionary<string, object> args = new Dictionary<string, object>();
            //Action
            var actual = _builder.BuildSelect(predicate, out args).Trim();
            // Assert
            var expected = "[Status] = @Status";
            Assert.AreEqual(expected, actual);
            // args
            Assert.AreEqual(2, args["@Status"]);
        }
        [TestMethod]
        public void BuildSelect_One_column_var_enum()
        {
            //Arrange
            var currentStatus = OrderStatus.Paid;
            Expression<Func<Entity_Select_Column_Test, Entity_Select_Column_Test>> predicate = n => new Entity_Select_Column_Test() { Status = currentStatus };
            Dictionary<string, object> args = new Dictionary<string, object>();
            //Action
            var actual = _builder.BuildSelect(predicate, out args).Trim();
            // Assert
            var expected = "[Status] = @Status";
            Assert.AreEqual(expected, actual);
            // args
            Assert.AreEqual(2, args["@Status"]);
        }
    }
}
