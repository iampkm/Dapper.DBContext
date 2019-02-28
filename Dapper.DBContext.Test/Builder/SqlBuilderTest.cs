using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dapper.DBContext.Builder;
using Dapper.DBContext.Test.Domain;
using System.Linq.Expressions;
using System.Dynamic;
using Dapper.DBContext.Data;

namespace Dapper.DBContext.Test.Builder
{
    [TestClass]
    public class SqlBuilderTest
    {
        ISqlBuilder _builder;
        // IDataBaseDialect _dialect;
        IConnectionFactory _connectionFactory;
        [TestInitialize]
        public void Init()
        {
            _connectionFactory = IConnectionFactory.Create("O2O");


            this._builder = this._connectionFactory.CreateBuilder();

            // this._builder = new SqlBuilder(_connectionFactory as IDataBaseDialect);
        }

        [TestMethod]
        public void BuilderInsert_test()
        {
            string target = this._builder.BuildInsert(typeof(Order));
            string expect = "INSERT INTO [Order] ([Code],[Status],[CreateAt],[CreateBy]) VALUES (@Code,@Status,@CreateAt,@CreateBy);SELECT SCOPE_IDENTITY()";
            Assert.AreEqual(expect, target);
        }
        [TestMethod]
        public void BuilderUpdate_test()
        {
            string target = this._builder.BuildUpdate(typeof(Order));
            string expect = "UPDATE [Order] SET [Code] = @Code,[Status] = @Status,[CreateAt] = @CreateAt,[CreateBy] = @CreateBy WHERE [Id] = @Id";
            Assert.AreEqual(expect, target);
        }
        [TestMethod]
        public void BuilderUpdate_sql_rowVersion_test()
        {
            string target = this._builder.BuildUpdate(typeof(SqlRowVersion_Test));
            string expect = "UPDATE [Order] SET [Code] = @Code WHERE [Id] = @Id AND [RowVersion] = @RowVersion";
            Assert.AreEqual(expect, target);
        }
        [TestMethod]
        public void BuildDelete_test()
        {
            string target = this._builder.BuildDelete(typeof(Order));
            string expect = "DELETE FROM [Order] WHERE [Id] = @Id";
            Assert.AreEqual(expect, target);
        }

        [TestMethod]
        public void BuildDelete_sql_rowVersion_test()
        {
            string target = this._builder.BuildDelete(typeof(SqlRowVersion_Test));
            string expect = "DELETE FROM [Order] WHERE [Id] = @Id AND [RowVersion] = @RowVersion";
            Assert.AreEqual(expect, target);
        }
        [TestMethod]
        public void BuilderSelect_No_Column()
        {
            string target = this._builder.BuildSelect<SqlRowVersion_Test>();

            string expect = "SELECT [Code],[Id],[RowVersion] FROM [Order]";
            Assert.AreEqual(expect, target);
        }

        [TestMethod]
        public void BuilderSelect_count_Columns()
        {
            string target = this._builder.BuildSelect<SqlRowVersion_Test>("COUNT(*)");
            string expect = "SELECT COUNT(*) FROM [Order]";
            Assert.AreEqual(expect, target);
        }

        [TestMethod]
        public void BuildWhere_One_Argument_test()
        {
            Expression<Func<SqlRowVersion_Test, bool>> where = p => p.Id == 12;
            dynamic args = new ExpandoObject();
            string target = this._builder.BuildSelectByLamda<SqlRowVersion_Test>(where, out args);
            string expect = "SELECT [Code],[Id],[RowVersion] FROM [Order] WHERE [Id] = @P0";
            Assert.AreEqual(expect, target);
            var dic = args as IDictionary<string, object>;
            Assert.AreEqual(12, (int)dic["@P0"]);
        }

        [TestMethod]
        public void BuildWhere_Two_Argument_test()
        {
            Expression<Func<SqlRowVersion_Test, bool>> where = p => p.Id == 12 && p.Code == "123";
            dynamic args = new ExpandoObject();
            string target = this._builder.BuildSelectByLamda<SqlRowVersion_Test>(where, out args);
            string expect = "SELECT [Code],[Id],[RowVersion] FROM [Order] WHERE [Id] = @P1 AND [Code] = @P0";
            Assert.AreEqual(expect, target);
            var dic = args as IDictionary<string, object>;
            Assert.AreEqual(12, (int)dic["@P1"]);
            Assert.AreEqual("123", (string)dic["@P0"]);
        }

        [TestMethod]
        public void BuildWhere_Multiple_Argument_and_Like_Method_test()
        {
            Expression<Func<SqlRowVersion_Test, bool>> where = p => p.Id == 12 || p.Code.Like("%123%");
            dynamic args = new ExpandoObject();
            string target = this._builder.BuildSelectByLamda<SqlRowVersion_Test>(where, out args);
            string expect = "SELECT [Code],[Id],[RowVersion] FROM [Order] WHERE [Id] = @P1 OR [Code] LIKE @P0";
            Assert.AreEqual(expect, target);
            var dic = args as IDictionary<string, object>;
            Assert.AreEqual(12, (int)dic["@P1"]);
            Assert.AreEqual("%123%", (string)dic["@P0"]);
        }

        [TestMethod]
        public void BuildWhere_Multiple_Argument_Range_test()
        {
            Expression<Func<SqlRowVersion_Test, bool>> where = p => p.Id > 12 && p.Id <= 24;
            dynamic args = new ExpandoObject();
            string target = this._builder.BuildSelectByLamda<SqlRowVersion_Test>(where, out args);
            string expect = "SELECT [Code],[Id],[RowVersion] FROM [Order] WHERE [Id] > @P1 AND [Id] <= @P0";
            Assert.AreEqual(expect, target);
            var dic = args as IDictionary<string, object>;
            Assert.AreEqual(12, (int)dic["@P1"]);
            Assert.AreEqual(24, (int)dic["@P0"]);

        }
        //[TestMethod]
        //public void Sum_One_Column()
        //{
        //    Expression<Func<SqlRowVersion_Test, bool>> where = p => p.Id > 12 && p.Id <= 24;
        //    Expression<Func<SqlRowVersion_Test, int>> select = p => p.Id;
        //    dynamic args = new ExpandoObject();
        //    string target = this._builder.BuildSelectByLamda<SqlRowVersion_Test, int>(where, out args, select, "sum");
        //    string expect = "select sum(Id) from [tb_time] where [Id] > @Id and [Id] <= @Id1";
        //    Assert.AreEqual(expect, target);
        //    var dic = args as IDictionary<string, object>;
        //    Assert.AreEqual(12, (int)dic["Id"]);
        //    Assert.AreEqual(24, (int)dic["Id1"]);
        //}

        //[TestMethod]
        //public void Sum_Two_Column()
        //{
        //    Expression<Func<OrderItem, bool>> where = p => p.Id > 12 && p.Id <= 24;
        //    Expression<Func<OrderItem, decimal>> select = p => p.Price * p.Quantity;
        //    dynamic args = new ExpandoObject();
        //    string target = this._builder.BuildSelectByLamda<OrderItem, decimal>(where, out args, select, "sum");
        //    string expect = "select sum(Price * Quantity) from [OrderItem] where [Id] > @Id and [Id] <= @Id1";
        //    Assert.AreEqual(expect, target);
        //    var dic = args as IDictionary<string, object>;
        //    Assert.AreEqual(12, (int)dic["Id"]);
        //    Assert.AreEqual(24, (int)dic["Id1"]);
        //}
        //[TestMethod]
        //public void Sum_Three_Column()
        //{
        //    Expression<Func<OrderItem, bool>> where = p => p.Id > 12 && p.Id <= 24;
        //    Expression<Func<OrderItem, decimal>> select = p => (p.Price + p.Quantity) / p.Quantity;
        //    dynamic args = new ExpandoObject();
        //    string target = this._builder.BuildSelectByLamda<OrderItem, decimal>(where, out args, select, "sum");
        //    string expect = "select sum((Price + Quantity) / Quantity) from [OrderItem] where [Id] > @Id and [Id] <= @Id1";
        //    Assert.AreEqual(expect, target);
        //    var dic = args as IDictionary<string, object>;
        //    Assert.AreEqual(12, (int)dic["Id"]);
        //    Assert.AreEqual(24, (int)dic["Id1"]);
        //}

        [TestMethod]
        public void BuildSelectByContext_Select_All()
        {
            //Arrange             
            QueryContext queryContext = new QueryContext();
            object args = new object();
            //Action
            var actual = _builder.BuildSelectByContext<Entity_Lamda_To_Sql>(queryContext, out args).Trim();
            // Assert
            var expected = "SELECT [Id],[Code],[Total_Amount],[TotalQuantity],[PaidTime],[Status] FROM [Order]";

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void BuildSelectByContext_Select_All_With_Where()
        {
            //Arrange
            Expression<Func<Entity_Lamda_To_Sql, bool>> predicate = i => i.Code == "No123";
            //Dictionary<string, object> args = new Dictionary<string, object>();
            QueryContext queryContext = new QueryContext();
            queryContext.Nodes.Add(new QueryNode(QueryNodeType.Where, predicate));
            object args = new object();
            //Action
            var actual = _builder.BuildSelectByContext<Entity_Lamda_To_Sql>(queryContext, out args).Trim();
            // Assert
            var expected = "SELECT [Id],[Code],[Total_Amount],[TotalQuantity],[PaidTime],[Status] FROM [Order] WHERE [Code] = @P0";

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void BuildSelectByContext_Select_All_With_Where_Where()
        {
            //Arrange
            int[] ids = { 1, 2, 3 };
            Expression<Func<Entity_Lamda_To_Sql, bool>> predicate = i => i.Code == "No123";
            Expression<Func<Entity_Lamda_To_Sql, bool>> predicate2 = i => i.Status == OrderStatus.Paid && i.Id.In(ids);
            //Dictionary<string, object> args = new Dictionary<string, object>();
            object args = new object();
            QueryContext queryContext = new QueryContext();
            queryContext.Nodes.Add(new QueryNode(QueryNodeType.Where, predicate));
            queryContext.Nodes.Add(new QueryNode(QueryNodeType.Where, predicate2));

            //Action
            var actual = _builder.BuildSelectByContext<Entity_Lamda_To_Sql>(queryContext, out args).Trim();
            // Assert
            var expected = "SELECT [Id],[Code],[Total_Amount],[TotalQuantity],[PaidTime],[Status] FROM [Order] WHERE [Code] = @P0 AND [Status] = @P2 AND [Id] IN @P1";

            Assert.AreEqual(expected, actual);
            var dd = (Dictionary<string, object>)args;
            Assert.AreEqual("No123", dd["@P0"]);
        }

        [TestMethod]
        public void BuildUpdate_Select_column_with_where()
          {
              //Arrange
              Expression<Func<Entity_Lamda_To_Sql, Entity_Lamda_To_Sql>> columns =n=>new Entity_Lamda_To_Sql(){ Code="123",Status= OrderStatus.Paid};
              Expression<Func<Entity_Lamda_To_Sql, bool>> where = n => n.Status == OrderStatus.WaitToPay && n.Id == 1;
              //Action
              object args = new object();
              var actual = _builder.BuildUpdate<Entity_Lamda_To_Sql>(columns, where, out args).Trim();

              var expected = "UPDATE [Order] SET [Code] = @Code,[Status] = @Status WHERE [Status] = @P1 AND [Id] = @P0";

              // Assert
              Assert.AreEqual(expected, actual);
              var dd = (Dictionary<string, object>)args;
              Assert.AreEqual(1, dd["@P0"]);
              Assert.AreEqual(1, dd["@P1"]);
              Assert.AreEqual(2, dd["@Status"]);
              Assert.AreEqual("123", dd["@Code"]);
          }

        [TestMethod]
        public void BuildUpdate_Entity_with_where()
        {
            //Arrange
            Entity_Lamda_To_Sql entity = new Entity_Lamda_To_Sql()
            {
                Id = 1,
                Code = "123",
                TotalAmount = 125.6m,
                TotalQuantity = 10,
                PaidTime = DateTime.Parse("2018-02-02"),
                Status = OrderStatus.Paid
            };
            Expression<Func<Entity_Lamda_To_Sql, bool>> where = n => n.Status == OrderStatus.WaitToPay && n.Id == 1;
            //Action
            object args = new object();
            var actual = _builder.BuildUpdate<Entity_Lamda_To_Sql>(entity, where, out args).Trim();

            var expected = "UPDATE [Order] SET [Code] = @Code,[Total_Amount] = @Total_Amount,[TotalQuantity] = @TotalQuantity,[PaidTime] = @PaidTime,[Status] = @Status WHERE [Status] = @P1 AND [Id] = @P0";

            // Assert
            Assert.AreEqual(expected, actual);
            var dd = (Dictionary<string, object>)args;
            Assert.AreEqual(1, dd["@P0"]);
            Assert.AreEqual(1, dd["@P1"]);
           
            Assert.AreEqual("123", dd["@Code"]);
            Assert.AreEqual(125.6m, dd["@Total_Amount"]);
            Assert.AreEqual(10, dd["@TotalQuantity"]);
            Assert.AreEqual(DateTime.Parse("2018-02-02"), dd["@PaidTime"]);
            Assert.AreEqual(2, dd["@Status"]);
        }
         [TestMethod]
        public void BuildUpdate_Foreach_Entity_with_where()
        {
            //Arrange
            List<Entity_Lamda_To_Sql> list = new List<Entity_Lamda_To_Sql>();
            for (var i = 0; i < 2; i++)
            {
                Entity_Lamda_To_Sql entity = new Entity_Lamda_To_Sql()
                {
                    Id = i,
                    Code = "100"+i.ToString(),
                    TotalAmount = 125.6m,
                    TotalQuantity = 10,
                    PaidTime = DateTime.Parse("2018-02-02"),
                    Status = OrderStatus.Paid
                };
                list.Add(entity);
            }

            list.ForEach(item =>
            {
                Expression<Func<Entity_Lamda_To_Sql, bool>> where = n => n.Status == OrderStatus.WaitToPay && n.Id == item.Id;
                //Action
                object args = new object();
                var actual = _builder.BuildUpdate<Entity_Lamda_To_Sql>(new Entity_Lamda_To_Sql() { 
                 Status = OrderStatus.Create, Code = item.Code
                }, where, out args).Trim();

                var expected = "UPDATE [Order] SET [Code] = @Code,[Total_Amount] = @Total_Amount,[TotalQuantity] = @TotalQuantity,[PaidTime] = @PaidTime,[Status] = @Status WHERE [Status] = @P1 AND [Id] = @P0";

                // Assert
                Assert.AreEqual(expected, actual);
            });
        }
    }
}
