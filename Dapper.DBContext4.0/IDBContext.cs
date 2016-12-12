using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Dapper.DBContext.Data;
namespace Dapper.DBContext
{
   public interface IDBContext
    {
        void Insert<TEntity>(TEntity model) where TEntity : class;
        void Insert<TEntity>(TEntity[] models) where TEntity : class;
        void Update<TEntity>(TEntity model) where TEntity : class;
        void Update<TEntity>(TEntity[] models) where TEntity : class;
        void Delete<TEntity>(TEntity model) where TEntity : class;
        void Delete<TEntity>(TEntity[] models) where TEntity : class;
        void Delete<TEntity>(object id) where TEntity : class;
        void Delete<TEntity>(Array ids) where TEntity : class;
        void Delete<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class;
        void SaveChange();
        IQuery Table { get; }
        IExecute Command { get; }
    }
}
