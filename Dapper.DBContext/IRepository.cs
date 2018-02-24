using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace Dapper.DBContext
{
    public interface IRepository
    {
        void Insert<TEntity>(TEntity entity) where TEntity : IEntity;
        void Insert<TEntity>(TEntity[] entitys) where TEntity : IEntity;
        void Update<TEntity>(TEntity entity) where TEntity : IEntity;
        void Update<TEntity>(TEntity[] entitys) where TEntity : IEntity;
        ///// <summary>
        ///  条件更新全部字段
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        void Update<TEntity>(TEntity entity,Expression<Func<TEntity, bool>> where) where TEntity : IEntity;
        /// <summary>
        ///  条件更新指定字段 update( a => new TEntity() { Name = "lu", Age = a.Age + 1},a => a.Id == 1); 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="columns">必须传递Lamda 实体初始化方式</param>
        /// <param name="where">条件表达式</param>
        void Update<TEntity>(Expression<Func<TEntity, TEntity>> columns, Expression<Func<TEntity, bool>> where) where TEntity : IEntity;
        void Delete<TEntity>(object id) where TEntity : IEntity;
        void Delete<TEntity>(TEntity entity) where TEntity : IEntity;
        void Delete<TEntity>(TEntity[] entitys) where TEntity : IEntity;
        void Delete<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : IEntity;
        void SaveChange();
        void SaveChangeAsync();
        /// <summary>
        /// 查询端接口
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        IQueryRepository<TEntity> Table<TEntity>() where TEntity : IEntity;
        /// <summary>
        ///  执行原生sql，返回一行一列值
        /// </summary>
        T ExecuteScalar<T>(string sql, object param = null);
        /// <summary>
        ///  执行原生sql,返回 影响行数
        /// </summary>
        int ExecuteSql(string sql, object param = null);

        /// <summary>
        /// 加入工作单元，SaveChange 后才执行
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        void AddExecute(string sql, object param = null);

        /// <summary>
        /// 原生Dapper 连接,使用方式参考Dapper 官网
        /// </summary>
        IDbConnection Dapper { get; }
    }
}
