using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dapper.DBContext.Dialect;
using Dapper.DBContext.Test.Domain;
namespace Dapper.DBContext.Test.Dialect
{
      [TestClass]
   public class SqlBuilderTest
    {
          ISqlBuilder _builder;

          [TestInitialize]
          public void Init()
          {
              this._builder = new SqlBuilder(); 
          }

          [TestMethod]
          public void BuilderInsert_test()
          {
              //Order order = new Order()
              //{
              //     Code = "12345678", CreateAt = DateTime.Now, CreateBy =1 , Status = OrderState.WaitPay, Address = new Address("九龙坡","重庆市")
              //};

             
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
           public void BuilderUpdate_Delete()
           {
               string target = this._builder.BuildDelete(typeof(Order));
               string expect = "delete from [Order] where [Id]=@Id and [RowVersion]=@RowVersion";
               Assert.AreEqual(expect, target);
           }
            [TestMethod]
           public void BuilderSelect_No_Column()
           {
               string target = this._builder.BuildSelect<tb_time>();
               string expect = "select [Code],[City],[Area],[Status],[CreateAt],[CreateBy] from [Order] ";
               Assert.AreEqual(expect, target);
           }
    }
}
