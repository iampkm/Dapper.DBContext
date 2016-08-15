using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext
{
    public abstract class AggregateRoot<TKey> : IEntity<TKey>
    {
        public AggregateRoot() { }
        public AggregateRoot(TKey id)
        {
            this.Id = id;         
        }
        /// <summary>
        /// 实体Id
        /// </summary>
        public TKey Id { get; set; }
    }
}
