using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.DBContext;
using Dapper.DBContext.Schema;
namespace Dapper.DBContext.Test.Domain
{
   public class Order :Entity<int>
    {

       public Order() {
           this.Items = new List<OrderItem>();
       }
       public string Code { get; set; }

       public Address Address { get; set; }

       public OrderState Status { get; set; }

       public DateTime CreateAt { get; set; }

       public int CreateBy { get; set; }

       public virtual IList<OrderItem> Items { get; set; }
      
    }

   public class OrderItem : Entity<int>
   {
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
       public string City { get; set; }

       public string Area { get; set; }
   }

   public enum OrderState
   { 
       WaitPay = 1 ,Paid =2
   }

   public class tb_time :ConcurrentEntity<int>
   {
       public tb_time() { }
       public tb_time(int id, string code):base(id)
       {          
           this.Code = code;        
       }
       public string Code { get; private set; }

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
