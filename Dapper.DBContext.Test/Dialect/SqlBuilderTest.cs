using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dapper.DBContext.Dialect;
using Dapper.DBContext.Test.Domain;
using System.Linq.Expressions;
using System.Dynamic;
namespace Dapper.DBContext.Test.Dialect
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
            string expect = "insert into [Order] ([Code],[City],[Area],[Status],[CreateAt],[CreateBy]) values (@Code,@City,@Area,@Status,@CreateAt,@CreateBy) SELECT SCOPE_IDENTITY()";
            Assert.AreEqual(expect, target);
        }
        [TestMethod]
        public void BuilderUpdate_test()
        {
            string target = this._builder.BuildUpdate(typeof(Order));
            string expect = "update [Order] set [Code] = @Code,[City] = @City,[Area] = @Area,[Status] = @Status,[CreateAt] = @CreateAt,[CreateBy] = @CreateBy where [Id]=@Id and [RowVersion]=@RowVersion";
            Assert.AreEqual(expect, target);
        }

        [TestMethod]
        public void BuildDelete_test()
        {
            string target = this._builder.BuildDelete(typeof(Order));
            string expect = "delete from [Order] where [Id]=@Id and [RowVersion]=@RowVersion";
            Assert.AreEqual(expect, target);
        }
        [TestMethod]
        public void BuilderSelect_No_Column()
        {
            string target = this._builder.BuildSelect<tb_time>();

            string expect = "select [Code],[Id],[RowVersion] from [tb_time] ";
            Assert.AreEqual(expect, target);
        }

        [TestMethod]
        public void BuilderSelect_Have_Columns()
        {
            string target = this._builder.BuildSelect<tb_time>("count(*)");
            string expect = "select count(*) from [tb_time] ";
            Assert.AreEqual(expect, target);
        }

        [TestMethod]
        public void BuildWhere_One_Argument_test()
        {
            Expression<Func<tb_time, bool>> where = p => p.Id == 12;
            dynamic args = new ExpandoObject();
            string target = this._builder.BuildWhere<tb_time>(where, out args);
            string expect = "where [Id] = @Id";
            Assert.AreEqual(expect, target);
            var dic = args as IDictionary<string, object>;
            Assert.AreEqual(12, (int)dic["Id"]);
        }

        [TestMethod]
        public void BuildWhere_Two_Argument_test()
        {
            Expression<Func<tb_time, bool>> where = p => p.Id == 12 && p.Code == "123";
            dynamic args = new ExpandoObject();
            string target = this._builder.BuildWhere<tb_time>(where, out args);
            string expect = "where [Id] = @Id and [Code] = @Code";
            Assert.AreEqual(expect, target);
            var dic = args as IDictionary<string, object>;
            Assert.AreEqual(12, (int)dic["Id"]);
            Assert.AreEqual("123", (string)dic["Code"]);
        }

        [TestMethod]
        public void BuildWhere_Multiple_Argument_and_Like_Method_test()
        {
            Expression<Func<tb_time, bool>> where = p => p.Id == 12 || p.Code.Like("%123%");
            dynamic args = new ExpandoObject();
            string target = this._builder.BuildWhere<tb_time>(where, out args);
            string expect = "where [Id] = @Id or [Code] Like @Code";
            Assert.AreEqual(expect, target);
            var dic = args as IDictionary<string, object>;
            Assert.AreEqual(12, (int)dic["Id"]);
            Assert.AreEqual("%123%", (string)dic["Code"]);
        }

        [TestMethod]
        public void BuildWhere_Multiple_Argument_Range_test()
        {
            Expression<Func<tb_time, bool>> where = p => p.Id > 12 && p.Id <= 24;
            dynamic args = new ExpandoObject();
            string target = this._builder.BuildWhere<tb_time>(where, out args);
            string expect = "where [Id] > @Id and [Id] <= @Id1";
            Assert.AreEqual(expect, target);
            var dic = args as IDictionary<string, object>;
            Assert.AreEqual(12, (int)dic["Id"]);
            Assert.AreEqual(24, (int)dic["Id1"]);
        }
    }
}
