using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Dapper.DBContext.Data;
using Dapper.DBContext.Builder;
using Dapper.DBContext.Helper;
using System.Collections;
namespace Dapper.DBContext
{
    /// <summary>
    ///  Dapper 仓储
    /// </summary>
    public class Repository : IRepository
    {
        IQuery _iquery;
        IUnitOfWork _uow;
        ISqlBuilder _builder;
        IConnectionFactory _connectionFactory;
        IExecute _command;
        IExecuteQuery _executeQuery;
        string _connString;
       


        public Repository(string connectionStringName)
        {
            _connectionFactory = IConnectionFactory.Create(connectionStringName);
            _connString = connectionStringName;
            this._iquery = new QueryService(connectionStringName);
            this._uow = new UnitOfWork(this._connectionFactory);
            _builder = _connectionFactory.CreateBuilder();
            this._command = new ExecuteService(this._connectionFactory, this._uow);
            this._executeQuery = new ExecuteQuery(this._connectionFactory);
        }
        public void Insert<TEntity>(TEntity entity) where TEntity : IEntity
        {
            // 子类的外键名，必须是 父类名+默认ID名；
            string parentIdName = string.Format("{0}{1}", entity.GetType().Name, ReflectionHelper.GetKeyName(entity.GetType()));
            string sql = this._builder.BuildInsert(entity.GetType());
            if (ReflectionHelper.ExistsAutoIncrementKey(entity.GetType()))
            {
                this._uow.Add(sql, entity, InsertMethodEnum.Parent, parentIdName);
            }
            else
            {
                this._uow.Add(sql, entity);
            }

            // 查找 关联的子类对象
            var childObjects = ReflectionHelper.GetForeignObject(entity);
            //子对象不是 1：1 就是 1：N
            foreach (object childObjItem in childObjects)
            {
                if (childObjItem == null) { continue; }
                var childObjItemType = childObjItem.GetType();
                var childObj = childObjItem as IEnumerable;
                if (childObj != null)
                {
                    var childObjList = childObj.GetEnumerator();
                    if (childObjList.MoveNext())
                    {
                        childObjItemType = childObjList.Current.GetType();
                    }
                }
                var childSql = this._builder.BuildInsert(childObjItemType);
                if (ReflectionHelper.ExistsAutoIncrementKey(entity.GetType()))
                {
                    this._uow.Add(childSql, childObjItem, InsertMethodEnum.Child, parentIdName);
                }
                else
                {
                    this._uow.Add(childSql, childObjItem);
                }
            }
        }

        public void Insert<TEntity>(TEntity[] entitys) where TEntity : IEntity
        {
            if (entitys.Count() <= 0) return;
            var entity = entitys[0];
            string parentIdName = string.Format("{0}{1}", entity.GetType().Name, ReflectionHelper.GetKeyName(entity.GetType()));
            string sql = this._builder.BuildInsert(entity.GetType());

            this._uow.Add(sql, entitys);
        }

        public void Update<TEntity>(TEntity entity) where TEntity : IEntity
        {
            string sql = this._builder.BuildUpdate(entity.GetType());

            this._uow.Add(sql, entity);
        }

        public void Update<TEntity>(TEntity[] entitys) where TEntity : IEntity
        {
            if (entitys.Count() <= 0) return;
            var entity = entitys[0];
            string sql = this._builder.BuildUpdate(entity.GetType());

            this._uow.Add(sql, entitys);
        }

        public void Update<TEntity>(TEntity entity, Expression<Func<TEntity, bool>> where) where TEntity : IEntity
        {
            if (entity == null) throw new Exception("entity参数不能为空");
            if (where == null) throw new Exception("where参数不能为空");
            object args = new object();
            var sql = _builder.BuildUpdate<TEntity>(entity, where, out args);
            this._uow.Add(sql, args); 
        }

        public void Update<TEntity>(Expression<Func<TEntity, TEntity>> columns, Expression<Func<TEntity, bool>> where) where TEntity : IEntity
        {
            if (columns == null) throw new Exception("columns参数不能为空");
            if (where == null) throw new Exception("where参数不能为空");
            object args = new object();
            var sql = _builder.BuildUpdate<TEntity>(columns, where,out args);
            this._uow.Add(sql, args); 
        }

        public void Delete<TEntity>(object id) where TEntity : IEntity
        {
            if (id == null) throw new Exception("id is empty");
            if (id is Array) throw new Exception("id 不能为数组");
            string sql = this._builder.BuildDelete(typeof(TEntity));
            this._uow.Add(sql, new { Id = id });
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : IEntity
        {
            string sql = this._builder.BuildDelete(entity.GetType());

            this._uow.Add(sql, entity);
        }

        public void Delete<TEntity>(TEntity[] entitys) where TEntity : IEntity
        {
            if (entitys.Count() <= 0) return;
            var entity = entitys[0];
            string sql = this._builder.BuildDelete(entity.GetType());
            this._uow.Add(sql, entitys);
        }

        public void Delete<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : IEntity
        {
            if (where == null) throw new Exception("参数不能为空");
            object args = new object();
            string sql = this._builder.BuildDeleteByLamda<TEntity>(where, out args);
            this._uow.Add(sql, args);
        }

        public void SaveChange()
        {
            this._uow.Commit();
        }

        public void SaveChangeAsync()
        {
            this._uow.CommitAsync();
        }

        public IQueryRepository<TEntity> Table<TEntity>() where TEntity : IEntity
        {           
            return new QueryRepository<TEntity>(this._connString);
        }

        public T ExecuteScalar<T>(string sql, object param = null)
        {
            return _command.ExecuteScalar<T>(sql, param);
        }

        public int ExecuteSql(string sql, object param = null)
        {

            return _command.Execute(sql, param);
        }

        public void AddExecute(string sql, object param = null)
        {
            this._uow.Add(sql, param);
        }

        public System.Data.IDbConnection Dapper
        {
            get { return _connectionFactory.CreateConnection(); }
        }
    }
}
