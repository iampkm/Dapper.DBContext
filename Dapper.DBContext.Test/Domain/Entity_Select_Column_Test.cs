using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.DBContext.Schema;
namespace Dapper.DBContext.Test.Domain
{
    /// <summary>
    ///  table = Order
    /// </summary>
    [Table("Order") ]
   public class Entity_Select_Column_Test
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public DateTime CreatedOn { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public OrderStatus Status { get; set; }
    }
}
