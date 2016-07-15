using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext
{
    /// <summary>
    /// Dapper 上下文
    /// </summary>
   public class DapperDBContext:IDBContext
    {
       public DapperDBContext(string connectionString) { 
       
       }

       public void Insert<T>(T model) where T : IEntity
       {
           throw new NotImplementedException();
       }

       public void Insert<T>(T[] models) where T : IEntity
       {
           throw new NotImplementedException();
       }

       public void Update<T>(T model) where T : IEntity
       {
           throw new NotImplementedException();
       }

       public void Update<T>(T[] models) where T : IEntity
       {
           throw new NotImplementedException();
       }

       public void Delete<T>(T model) where T : IEntity
       {
           throw new NotImplementedException();
       }

       public void Delete<T>(T[] models) where T : IEntity
       {
           throw new NotImplementedException();
       }

       public void SaveChange()
       {
           throw new NotImplementedException();
       }

       public IQuery Query
       {
           get { throw new NotImplementedException(); }
       }
    }
}
