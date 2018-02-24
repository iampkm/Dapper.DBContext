using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.DBContext.Schema;
namespace Dapper.DBContext.Test.Domain
{
   public class Entity_Parent_Test:IEntity
    {
       public Entity_Parent_Test() {
           this.Childs = new List<Entity_Child_Test>();
       }
       public List<Entity_Child_Test> Childs { get; set; }

       public virtual Entity_Child_Test One { get; set; }

       [NotMapped]
       public string NotMappedColumn { get; set; }

       public int Id { get; set; }
    }

   public class Entity_AutoInCreament_longType : IEntity
   {
       public long Id { get; set; }
   }
   public class Entity_Not_AutoInCreament_stringType : IEntity
   {
       public string Id { get; set; }
   }
   public class Entity_AutoInCreament_KeyAttribute_intType : IEntity
   {
       [Key]
       public int SysNo { get; set; }
   }
   public class Entity_Not_AutoInCreament_KeyAttribute_intType : IEntity
   {
       [Key(false)]
       public int SysNo { get; set; }
   }
   public class Entity_Not_AutoInCreament_KeyAttribute_stringType : IEntity
   {
       [Key(false)]
       public string SysNo { get; set; }
   }

  
}
