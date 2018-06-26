using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Dapper.DBContext.Data;
namespace Dapper.DBContext
{
    public interface IDBContext
    {
        void Insert<TEntity>(TEntity entity) where TEntity : class;
        void Insert<TEntity>(TEntity[] entitys) where TEntity : class;
        void Update<TEntity>(TEntity entity) where TEntity : class;
        void Update<TEntity>(TEntity[] entitys) where TEntity : class;
        ///// <summary>
        ///  条件更新全部字段
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        void Update<TEntity>(TEntity entity,Expression<Func<TEntity, bool>> where) where TEntity : class;
        /// <summary>
        ///  条件更新指定字段 update( a => new TEntity() { Name = "lu", Age = a.Age + 1},a => a.Id == 1); 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="columns">必须传递Lamda 实体初始化方式</param>
        /// <param name="where">条件表达式</param>
        void Update<TEntity>(Expression<Func<TEntity, TEntity>> columns, Expression<Func<TEntity, bool>> where) where TEntity : class;
        //void Delete<TEntity>(object id) where TEntity : class;
        //void Delete<TEntity>(Array ids) where TEntity : class;
        void Delete<TEntity>(TEntity entity) where TEntity : class;       
        void Delete<TEntity>(TEntity[] entitys) where TEntity : class;
        void Delete<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class;
        void SaveChange();
        void SaveChangeAsync();
        /// <summary>
        /// 查询端接口
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        IQuery<TEntity> Table<TEntity>() where TEntity : class;        

        IDataBase DataBase { get; }
    }
}
