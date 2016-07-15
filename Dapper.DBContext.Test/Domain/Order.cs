using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.DBContext;
namespace Dapper.DBContext.Test.Domain
{
   public class Order :BaseEntity<int>
    {
       public string Code { get; set; }

       public Address Address { get; set; }

       public OrderState Status { get; set; }

       public DateTime CreateAt { get; set; }

       public int CreateBy { get; set; }

       public IList<OrderItem> Items { get; set; }
      
    }

   public class OrderItem : BaseEntity<int>
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

   public class tb_time :BaseEntity<int>
   {
       public tb_time() { }
       public tb_time(int id, string code):base(id)
       {          
           this.Code = code;        
       }
       public string Code { get; private set; }

   }
}
