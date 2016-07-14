using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext
{
    internal interface IEntity
    {
        
    }

    public interface IEntity<TKey>:IEntity
    {
        /// <summary>
        /// 实体Id
        /// </summary>
        public TKey Id { get; set; }
        /// <summary>
        /// 时间戳，乐观并发使用
        /// </summary>
        public byte[] TimeStamp { get; set; }
    }
}
