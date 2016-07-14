using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext
{
   public interface IDBContext
    {
        void Insert<T>(T model) where T : IEntity;
        void Insert<T>(T[] models) where T : IEntity;
        void Update<T>(T model) where T : IEntity;
        void Update<T>(T[] models) where T : IEntity;
        void Delete<T>(T model) where T : IEntity;
        void Delete<T>(T[] models) where T : IEntity;
        void SaveChange();
        IQuery Query { get; }       
    }
}
