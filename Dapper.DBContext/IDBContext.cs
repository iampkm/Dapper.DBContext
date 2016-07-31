﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        void SaveChange();
        IQuery Table { get; }       
    }
}
