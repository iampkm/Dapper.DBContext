using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.DBContext.Schema;
namespace Dapper.DBContext.Test.Domain
{
    /// <summary>
    ///  测试实体，兰姆达表达式转sql  : table =Order
    /// </summary>
   
    [Table("Order")]
   public class Entity_Lamda_To_Sql:IEntity
    {
        public Entity_Lamda_To_Sql() {
            list = new List<Entity_Lamda_To_Sql_Foreign>();
        }
        public int Id { get; set; }

        public string Code { get; set; }
        [Column("Total_Amount")]
        public decimal TotalAmount { get; set; }

        public int TotalQuantity { get; set; }

        public DateTime PaidTime { get; set; }

        public OrderStatus Status { get; set; }

        public List<Entity_Lamda_To_Sql_Foreign> list { get; set; }
    }
    [Table("OrderItem")]
    public class Entity_Lamda_To_Sql_Foreign:IEntity
    {
        public int Id { get; set; }
    }

    public enum OrderStatus
    {
        Create, WaitToPay, Paid
    }
}
