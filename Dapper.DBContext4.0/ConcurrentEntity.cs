using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext
{
    /// <summary>
    /// 并发控制实体，表中需要创建一个字节型 RowVersion 字段
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class ConcurrentEntity<TKey> : IEntity<TKey>
    {
        public ConcurrentEntity() { }
        public ConcurrentEntity(TKey id)
        {
            this.Id = id;         
        }
        /// <summary>
        /// 实体Id
        /// </summary>
        public TKey Id { get; set; }

        /// <summary>
        /// 版本乐观并发字段，无须开发显示设置值
        /// </summary>
        public byte[] RowVersion { get; private set; }

        //private long DateTimeToStamp(System.DateTime time)
        //{
        //    System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        //    return (long)(time - startTime).TotalSeconds;
        //}
    }
}
