using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Dapper.DBContext.Data;
using Dapper.DBContext.Builder;
using Dapper.DBContext.Helper;
using System.Collections;
using System.Configuration;
namespace Dapper.DBContext
{
    /// <summary>
    /// Dapper 上下文
    /// </summary>
    public class DapperDBContext : IDBContext
    {
        IQuery _iquery;
        IUnitOfWork _uow;
        ISqlBuilder _builder;
        IConnectionFactory _connectionFactory;

        public DapperDBContext(string connectionStringName)
        {
            _connectionFactory = IConnectionFactory.Create(connectionStringName);
         
            this._iquery = new QueryService(connectionStringName);
            this._uow = new UnitOfWork(this._connectionFactory);
            this._builder = this._connectionFactory.CreateBuilder();
        }

        public DapperDBContext(string connectionStringName, IQuery iquery):this(connectionStringName)
        {
            this._iquery = iquery;          
        }
        public void Insert<T>(T model) where T : IEntity
        {
            // 子类的外键名，必须是 父类名+默认ID名；
            string parentIdName = string.Format("{0}{1}", model.GetType().Name, ReflectionHelper.GetKeyName(model.GetType()));
            string sql = this._builder.BuildInsert(model.GetType());

            this._uow.Add(sql, model, InsertMethodEnum.Parent, parentIdName);

            // 查找 关联的子类对象
            var childObjects = ReflectionHelper.GetForeignObject(model);
            //构造子对象sql 
            foreach (object childObjList in childObjects)
            {
                foreach (object childObj in childObjList as IEnumerable)
                {
                    var childSql = this._builder.BuildInsert(childObj.GetType());
                    this._uow.Add(childSql, childObjList, InsertMethodEnum.Child, parentIdName);
                    break;
                }
            }
        }

        public void Insert<T>(T[] models) where T : IEntity
        {
            if (models.Count() <= 0) throw new Exception("models is empty");
            var model = models[0];
            string parentIdName = string.Format("{0}{1}", model.GetType().Name, ReflectionHelper.GetKeyName(model.GetType()));
            string sql = this._builder.BuildInsert(model.GetType());

            this._uow.Add(sql, model, InsertMethodEnum.Parent, parentIdName);
        }

        public void Update<T>(T model) where T : IEntity
        {
            string sql = this._builder.BuildUpdate(model.GetType());

            this._uow.Add(sql, model);
        }

        public void Update<T>(T[] models) where T : IEntity
        {
            if (models.Count() <= 0) throw new Exception("models is empty");
            var model = models[0];
            string sql = this._builder.BuildUpdate(model.GetType());

            this._uow.Add(sql, model);
        }

        public void Delete<T>(T model) where T : IEntity
        {
            string sql = this._builder.BuildDelete(model.GetType());

            this._uow.Add(sql, model);
        }

        public void Delete<T>(T[] models) where T : IEntity
        {
            if (models.Count() <= 0) throw new Exception("models is empty");
            var model = models[0];
            string sql = this._builder.BuildDelete(model.GetType());

            this._uow.Add(sql, model);
        }

        public void SaveChange()
        {
            this._uow.Commit();
        }

        public IQuery Query
        {
            get { return this._iquery; }
        }
    }
}
