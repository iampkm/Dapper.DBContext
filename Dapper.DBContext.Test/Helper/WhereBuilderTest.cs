﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dapper.DBContext.Helper;
using Dapper.DBContext;
using Dapper.DBContext.Test.Domain;
using Dapper.DBContext.Data;
using Dapper.DBContext.Builder;
namespace Dapper.DBContext.Test.Helper
{
    [TestClass]
    public class WhereBuilderTest
    {

        ConditionBuilder _builder;

        [TestInitialize]
        public void Init()
        {
            IDataBaseDialect msSqlDialect = new SqlServerFactory("O2O");
            IDialectBuilder mssqlDialectBuilder = new DialectBuilder(msSqlDialect);
            _builder = new ConditionBuilder(mssqlDialectBuilder);
        }

        [TestMethod]
        public void Build_One_Constant()
        {
            //Arrange
            Expression<Func<Entity_Lamda_To_Sql, bool>> predicate = i => i.Code == "No123";
            Dictionary<string, object> args = new Dictionary<string, object>();
            //Action
            var actual = _builder.BuildWhere(predicate, out args).Trim();
            // Assert
            var expected = "[Code] = @P0";

            Assert.AreEqual(expected, actual);
            Assert.AreEqual("No123", args["@P0"]);
        }

        [TestMethod]
        public void Build_One_PropieteSame()
        {
            //Arrange
            // Entity_Lamda_To_Sql model = new Entity_Lamda_To_Sql();
            Order model = new Order();
            model.Code = "No123";
            Expression<Func<Entity_Lamda_To_Sql, bool>> predicate = i => i.Code == model.Code;
            Dictionary<string, object> args = new Dictionary<string, object>();
            //Action
            var actual = _builder.BuildWhere(predicate, out args).Trim();
            // Assert
            var expected = "[Code] = @P0";

            Assert.AreEqual(expected, actual);
            Assert.AreEqual("No123", args["@P0"]);
        }

        [TestMethod]
        public void Build_Same_Propiete_And_Type_Add()
        {
            //Arrange
            // Entity_Lamda_To_Sql model = new Entity_Lamda_To_Sql();
            Entity_Lamda_To_Sql model = new Entity_Lamda_To_Sql();
            model.Code = "No123";
            Expression<Func<Entity_Lamda_To_Sql, bool>> predicate = i =>  i.Code == i.Code + model.Code;
            Dictionary<string, object> args = new Dictionary<string, object>();
            //Action
            var actual = _builder.BuildWhere(predicate, out args).Trim();
            // Assert
            var expected = "[Code] = [Code] + @P0";

            Assert.AreEqual(expected, actual);
            Assert.AreEqual("No123", args["@P0"]);
        }

        [TestMethod]
        public void Build_Same_Propiete_And_Type_Add_right()
        {
            //Arrange
            // Entity_Lamda_To_Sql model = new Entity_Lamda_To_Sql();
            Entity_Lamda_To_Sql model = new Entity_Lamda_To_Sql();
            model.Code = "No123";
            Expression<Func<Entity_Lamda_To_Sql, bool>> predicate = i =>i.Code + model.Code == i.Code;
            Dictionary<string, object> args = new Dictionary<string, object>();
            //Action
            var actual = _builder.BuildWhere(predicate, out args).Trim();
            // Assert
            var expected = "[Code] + @P0 = [Code]";

            Assert.AreEqual(expected, actual);
            Assert.AreEqual("No123", args["@P0"]);
        }

        [TestMethod]
        public void Build_One_Propiete_Type_Same()
        {
            //Arrange
            // Entity_Lamda_To_Sql model = new Entity_Lamda_To_Sql();
            Entity_Lamda_To_Sql model = new Entity_Lamda_To_Sql();
            model.Code = "No123";
            Expression<Func<Entity_Lamda_To_Sql, bool>> predicate = i => i.Code == model.Code && i.Code == i.Code + model.Code;
            Dictionary<string, object> args = new Dictionary<string, object>();
            //Action
            var actual = _builder.BuildWhere(predicate, out args).Trim();
            // Assert
            var expected = "[Code] = @P1 AND [Code] = [Code] + @P0";

            Assert.AreEqual(expected, actual);
            Assert.AreEqual("No123", args["@P0"]);
            Assert.AreEqual("No123", args["@P1"]);
        }

        [TestMethod]
        public void Build_One_Object_Method()
        {
            //Arrange
            // Entity_Lamda_To_Sql model = new Entity_Lamda_To_Sql();
            Entity_Lamda_To_Sql model = new Entity_Lamda_To_Sql();
            Expression<Func<Entity_Lamda_To_Sql, bool>> predicate = i => i.PaidTime == model.GetTime();
            Dictionary<string, object> args = new Dictionary<string, object>();
            //Action
            var actual = _builder.BuildWhere(predicate, out args).Trim();
            // Assert
            var expected = "[PaidTime] = @P0";

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(DateTime.Parse("2019-01-01 10:10:10"), args["@P0"]);
        }

        [TestMethod]
        public void Build_One_Object_Datetime_Now()
        {
            //Arrange
            // Entity_Lamda_To_Sql model = new Entity_Lamda_To_Sql();
            Entity_Lamda_To_Sql model = new Entity_Lamda_To_Sql();
            Expression<Func<Entity_Lamda_To_Sql, bool>> predicate = i => i.PaidTime == DateTime.Now;
            Dictionary<string, object> args = new Dictionary<string, object>();
            //Action
            var actual = _builder.BuildWhere(predicate, out args).Trim();
            // Assert
            var expected = "[PaidTime] = @P0";

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ((DateTime)args["@P0"]).ToString("yyyy-MM-dd HH:mm:ss"));
        }

        [TestMethod]
        public void Build_One_Object_Method2()
        {
            //Arrange
            // Entity_Lamda_To_Sql model = new Entity_Lamda_To_Sql();
            Entity_Lamda_To_Sql model = new Entity_Lamda_To_Sql();
            model.Code = "No123";
            Expression<Func<Entity_Lamda_To_Sql, bool>> predicate = i => i.Code == model.GetNewId().ToString();
            Dictionary<string, object> args = new Dictionary<string, object>();
            //Action
            var actual = _builder.BuildWhere(predicate, out args).Trim();
            // Assert
            var expected = "[Code] = @P0";

            Assert.AreEqual(expected, actual);
            Assert.AreEqual("123", args["@P0"]);
        }

        [TestMethod]
        public void Build_One_Object_Method_withParameter()
        {
            //Arrange
            // Entity_Lamda_To_Sql model = new Entity_Lamda_To_Sql();
            Entity_Lamda_To_Sql model = new Entity_Lamda_To_Sql();
            model.Code = "No123";
            Expression<Func<Entity_Lamda_To_Sql, bool>> predicate = i => i.Code == model.GetNewId(113).ToString();
            Dictionary<string, object> args = new Dictionary<string, object>();
            //Action
            var actual = _builder.BuildWhere(predicate, out args).Trim();
            // Assert
            var expected = "[Code] = @P0";

            Assert.AreEqual(expected, actual);
            Assert.AreEqual("123", args["@P0"]);
        }

        [TestMethod]
        public void Build_One_variable()
        {
            //Arrange
            var codeValue = "No123";
            Expression<Func<Entity_Lamda_To_Sql, bool>> predicate = i => i.Code == codeValue;
            Dictionary<string, object> args = new Dictionary<string, object>();
            //Action
            var actual = _builder.BuildWhere(predicate, out args).Trim();
            // Assert
            var expected = "[Code] = @P0";

            Assert.AreEqual(expected, actual);
            Assert.AreEqual("No123", args["@P0"]);
        }
        [TestMethod]
        public void Build_One_variable_menu()
        {
            //Arrange
            Expression<Func<Entity_Lamda_To_Sql, bool>> predicate = i => i.Status == OrderStatus.Paid;
            Dictionary<string, object> args = new Dictionary<string, object>();
            //Action
            var actual = _builder.BuildWhere(predicate, out args).Trim();
            // Assert
            var expected = "[Status] = @P0";

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(2, args["@P0"]);
        }

        [TestMethod]
        public void Build_One_variable_and_one_Constant()
        {
            //Arrange
            var codeValue = "No123";
            Expression<Func<Entity_Lamda_To_Sql, bool>> predicate = i => i.Code == codeValue && i.TotalQuantity == 100;
            Dictionary<string, object> args = new Dictionary<string, object>();
            //Action
            var actual = _builder.BuildWhere(predicate, out args).Trim();
            // Assert
            var expected = "[Code] = @P1 AND [TotalQuantity] = @P0";

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(codeValue, args["@P1"]);
            Assert.AreEqual(100, args["@P0"]);
        }
        [TestMethod]
        public void Build_Three_Variable_In()
        {
            //Arrange
            var codeValue = "No123";
            int[] numbers = { 1, 2, 3, 4 };
            Expression<Func<Entity_Lamda_To_Sql, bool>> predicate = i => i.Code == codeValue && i.TotalQuantity == 100 || i.Id.In(numbers);
            Dictionary<string, object> args = new Dictionary<string, object>();
            //Action
            var actual = _builder.BuildWhere(predicate, out args).Trim();
            // Assert
            var expected = "[Code] = @P2 AND [TotalQuantity] = @P1 OR [Id] IN @P0";

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(codeValue, args["@P2"]);
            Assert.AreEqual(100, args["@P1"]);
            Assert.AreEqual(numbers, args["@P0"]);
        }
        [TestMethod]
        public void Build_Three_Variable_Like_In()
        {
            //Arrange
            var codeValue = "No123";
            var likeValue = "No%";
            int[] numbers = { 1, 2, 3, 4 };
            Expression<Func<Entity_Lamda_To_Sql, bool>> predicate = i => i.Code == codeValue && i.TotalQuantity == 100 || i.Id.In(numbers) && i.Code.Like(likeValue);
            Dictionary<string, object> args = new Dictionary<string, object>();
            //Action
            var actual = _builder.BuildWhere(predicate, out args).Trim();
            // Assert
            var expected = "[Code] = @P3 AND [TotalQuantity] = @P2 OR [Id] IN @P1 AND [Code] LIKE @P0";

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(codeValue, args["@P3"]);
            Assert.AreEqual(100, args["@P2"]);
            Assert.AreEqual(numbers, args["@P1"]);
            Assert.AreEqual(likeValue, args["@P0"]);
        }
        [TestMethod]
        public void Build_Mehtod_Between()
        {
            //Arrange
            DateTime now = DateTime.Parse("2018-01-25 10:00:00");
            Expression<Func<Entity_Lamda_To_Sql, bool>> predicate = i => i.PaidTime.Between(now.Date, now.Date.AddDays(1).AddSeconds(-1));
            Dictionary<string, object> args = new Dictionary<string, object>();
            //Action
            var actual = _builder.BuildWhere(predicate, out args).Trim();
            // Assert
            var expected = "[PaidTime] BETWEEN @P0 AND @P1";

            Assert.AreEqual(expected, actual);

            Assert.AreEqual(now.Date, args["@P0"]);
            Assert.AreEqual(now.Date.AddDays(1).AddSeconds(-1), args["@P1"]);
        }
        [TestMethod]
        public void Build_complex_expression()
        {
            //Arrange
            var codeValue = "No123";
            DateTime now = DateTime.Parse("2018-01-25 10:00:00");
            Expression<Func<Entity_Lamda_To_Sql, bool>> predicate = i => i.Code == codeValue && i.TotalQuantity > "123".Length || i.PaidTime.Between(now.Date, now.Date.AddDays(1).AddSeconds(-1)) && i.TotalQuantity == i.TotalQuantity + 1 || i.PaidTime > now;
            Dictionary<string, object> args = new Dictionary<string, object>();
            //Action
            var actual = _builder.BuildWhere(predicate, out args).Trim();
            // Assert
            var expected = "[Code] = @P5 AND [TotalQuantity] > @P4 OR [PaidTime] BETWEEN @P2 AND @P3 AND [TotalQuantity] = [TotalQuantity] + @P1 OR [PaidTime] > @P0";

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(codeValue, args["@P5"]);
            Assert.AreEqual(3, args["@P4"]);
            Assert.AreEqual(now.Date, args["@P2"]);
            Assert.AreEqual(now.Date.AddDays(1).AddSeconds(-1), args["@P3"]);
            Assert.AreEqual(1, args["@P1"]);
            Assert.AreEqual(now, args["@P0"]);
        }

        [TestMethod]
        public void Build_One_is_Null()
        {
            //Arrange

            Expression<Func<Entity_Lamda_To_Sql, bool>> predicate = i => i.Code == null;
            Dictionary<string, object> args = new Dictionary<string, object>();
            //Action
            var actual = _builder.BuildWhere(predicate, out args).Trim();
            // Assert
            var expected = "[Code] IS NULL";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Build_One_is_Not_Null()
        {
            //Arrange

            Expression<Func<Entity_Lamda_To_Sql, bool>> predicate = i => i.Code != null;
            Dictionary<string, object> args = new Dictionary<string, object>();
            //Action
            var actual = _builder.BuildWhere(predicate, out args).Trim();
            // Assert
            var expected = "[Code] IS NOT NULL";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Build_One_Mutile_is_Null()
        {
            //Arrange
            DateTime now = DateTime.Parse("2018-01-25 10:00:00");
            Expression<Func<Entity_Lamda_To_Sql, bool>> predicate = i => i.Code == null && i.PaidTime.Between(now.Date, now.Date.AddDays(1).AddSeconds(-1));
            Dictionary<string, object> args = new Dictionary<string, object>();
            //Action
            var actual = _builder.BuildWhere(predicate, out args).Trim();
            // Assert
            var expected = "[Code] IS NULL AND [PaidTime] BETWEEN @P0 AND @P1";

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(now.Date, args["@P0"]);
            Assert.AreEqual(now.Date.AddDays(1).AddSeconds(-1), args["@P1"]);
        }


    }
}
