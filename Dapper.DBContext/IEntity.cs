using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext
{
    public interface IEntity
    {

    }

    public interface IEntity<TKey> : IEntity
    {
        /// <summary>
        /// 实体Id
        /// </summary>
        TKey Id { get; set; }       
    }
    /// <summary>
    /// 实体基类
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class Entity<TKey> : IEntity<TKey>
    {
        public Entity() { }       
        public Entity(TKey id)
        {
            this.Id = id;         
        }
        /// <summary>
        /// 实体Id
        /// </summary>
        public TKey Id { get; set; }        

    }
}
