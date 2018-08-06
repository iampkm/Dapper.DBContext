using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.DBContext;
using Dapper.DBContext.Schema;
namespace Dapper.DBContext.Test.Domain
{
    /// <summary>
    ///  普通类，类中聚合了 1：N 子对象 和 1：1 对象
    /// </summary>
   public class Order :Entity<int>
    {

       public Order() {
           this.Items = new List<OrderItem>();
          // this.addressList = new List<Address>();
       }
       public string Code { get; set; }       

       public OrderState Status { get; set; }

       public DateTime CreateAt { get; set; }

       public int CreateBy { get; set; }

       /// <summary>
       ///  外键 1 ：1 
       /// </summary>
      // public Address Address { get; set; }

       public List<OrderItem> Items { get; set; }

      // public virtual List<Address> addressList { get; set; }
      
    }

   public class OrderItem // : Entity<int>
   {
       public int Id { get; set; }
      public int OrderId { get; set; }

      public int ProductId { get; set; }

      public string ProductName { get; set; }

      public decimal Price { get; set; }

      public int Quantity { get; set; }


   }



   public class Address
   {
       public Address(string city, string area)
       {
           this.City = city;
           this.Area = area;
       }
       [Key]
       public int OrderId { get; set; }
       public string City { get; set; }

       public string Area { get; set; }
   }

   public enum OrderState
   { 
       WaitPay = 1 ,Paid =2
   }
   
    /// <summary>
    ///  sql  行版本测试类, 模拟表，order
    /// </summary>
     [Table("Order")]
   public class SqlRowVersion_Test :ConcurrentEntity<int>
   {
       public SqlRowVersion_Test() { }
       public SqlRowVersion_Test(int id, string code):base(id)
       {          
           this.Code = code;        
       }
       public string Code { get; private set; }

   }

    /// <summary>
    /// mysql 时间戳版本号测试
    /// </summary>
   public class temp_test :Entity<int>
   {
       public temp_test() { }
       public temp_test(int id, string qName, int quantity, DateTime rowVersion)
           : base(id)
       {
           this.QName = qName;
           this.Quantity = quantity;
           this.RowVersion = rowVersion;

       }
       public string QName { get; private set; }

       public int Quantity { get; set; }

       public DateTime RowVersion { get; set; }

   }
   
    [Table("o2o_customer")]
   public class CustomerEntity : IEntity
   {
       public CustomerEntity() { }

       public CustomerEntity(string id,DateTime createOn)
       {
           this.CustomerID = id;
           this.CreateOn = createOn;
       }
       [Key]
       public string CustomerID { get; private set; }

       public DateTime CreateOn { get; private set; }
   }

    [Table("o2o_customer")]
    public class CustomerAutoIDEntity : IEntity
    {
        public CustomerAutoIDEntity() { }

        public CustomerAutoIDEntity(int id, DateTime createOn)
        {
            this.CustomerID = id;
            this.CreateOn = createOn;
        }
        [Column("cusid")]
        [Key]
        public int CustomerID { get; private set; }

        public DateTime CreateOn { get; private set; }

        [NotMapped]
        public string Code { get; private set; }
    }

    public class Category : Entity<string>
    {
        public Category()
        {
            this.Level = 1;
        }
        public string Name { get; set; }

        public string FullName { get; set; }

        public int Level { get; set; }


    }
}
