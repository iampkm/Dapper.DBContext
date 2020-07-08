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
        IExecute _command;

        public DapperDBContext(string connectionStringName)
        {
            _connectionFactory = IConnectionFactory.Create(connectionStringName);

            this._iquery = new QueryService(connectionStringName);
            this._uow = new UnitOfWork(this._connectionFactory);
            this._builder = this._connectionFactory.CreateBuilder();
            this._command = new ExecuteService(this._connectionFactory, this._uow);
        }

        public DapperDBContext(string connectionStringName, IQuery iquery)
            : this(connectionStringName)
        {
            this._iquery = iquery;
        }
        public void Insert<TEntity>(TEntity model) where TEntity : class
        {
            // 子类的外键名，必须是 父类名+默认ID名；
            string parentIdName = string.Format("{0}{1}", model.GetType().Name, ReflectionHelper.GetKeyName(model.GetType()));
            string sql = this._builder.BuildInsert(model.GetType());         
            if (ReflectionHelper.isIdentity(model.GetType()))
            {
                this._uow.Add(sql, model, InsertMethodEnum.Parent, parentIdName);
            }
            else {
                this._uow.Add(sql, model);
            }

            // 查找 关联的子类对象
            var childObjects = ReflectionHelper.GetForeignObject(model);
            //子对象不是 1：1 就是 1：N
            foreach (object childObjItem in childObjects)
            {
                if (childObjItem == null) { continue; }
                var childObjItemType = childObjItem.GetType();
                var childSql = "";
                if (childObjItemType.IsGenericType)
                {
                    // 1:N                  
                    var childObjList = (childObjItem as IEnumerable).GetEnumerator();
                    if (!childObjList.MoveNext()) { continue; } //  有外键定义，但无数据，不用生成子对象sql 语句
                    var childObjElementType = childObjItemType.GetGenericArguments().FirstOrDefault();
                    childSql = this._builder.BuildInsert(childObjElementType);
                }
                else
                {
                    // 1:1
                    childSql = this._builder.BuildInsert(childObjItemType);
                }
                if (ReflectionHelper.isIdentity(model.GetType()))
                {
                    this._uow.Add(childSql, childObjItem, InsertMethodEnum.Child, parentIdName);
                }
                else {
                    this._uow.Add(childSql, childObjItem);
                }                
            }
        }

        public void Insert<TEntity>(TEntity[] models) where TEntity : class
        {
            if (models.Count() <= 0) throw new Exception("models is empty");
            var model = models[0];
            string parentIdName = string.Format("{0}{1}", model.GetType().Name, ReflectionHelper.GetKeyName(model.GetType()));
            string sql = this._builder.BuildInsert(model.GetType());

            this._uow.Add(sql, models);
        }

        public void Update<TEntity>(TEntity model) where TEntity : class
        {
            string sql = this._builder.BuildUpdate(model.GetType());

            this._uow.Add(sql, model);
        }

        public void Update<TEntity>(TEntity[] models) where TEntity : class
        {
            if (models.Count() <= 0) throw new Exception("models is empty");
            var model = models[0];
            string sql = this._builder.BuildUpdate(model.GetType());

            this._uow.Add(sql, models);
        }

        public void Delete<TEntity>(TEntity model) where TEntity : class
        {
            string sql = this._builder.BuildDelete(model.GetType());

            this._uow.Add(sql, model);
        }

        public void Delete<TEntity>(TEntity[] models) where TEntity : class
        {
            if (models.Count() <= 0) throw new Exception("models is empty");
            var model = models[0];
            string sql = this._builder.BuildDelete(model.GetType());
            this._uow.Add(sql, model);
        }

        public void Delete<TEntity>(object id) where TEntity : class
        {
            if (id == null) throw new Exception("id is empty");
            if (id is Array) throw new Exception("id 不能为数组"); 
            string sql = this._builder.BuildDelete(typeof(TEntity));
            this._uow.Add(sql, new { Id = id });
        }
        public void Delete<TEntity>(Array ids) where TEntity : class
        {
            if (ids == null) throw new Exception("id is empty");           
            string sql = this._builder.BuildDelete(typeof(TEntity),false);
            this._uow.Add(sql, new { Id = ids });
        }

        public void Delete<TEntity>(System.Linq.Expressions.Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            if (expression == null) throw new Exception("参数不能为空");
             object args = new object();
             string sql = this._builder.buildDeleteByLamda<TEntity>(expression, out args);
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

        public IQuery Table
        {
            get { return this._iquery; }
        }

        public IExecute Command
        {
            get { return this._command; }
        }
    }
}
