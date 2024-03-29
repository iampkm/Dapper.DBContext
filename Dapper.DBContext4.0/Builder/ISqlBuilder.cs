﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace Dapper.DBContext.Builder
{
    public interface ISqlBuilder
    {
        string BuildInsert(Type modelType);
        string BuildUpdate(Type modelType);
        string BuildDelete(Type modelType, bool isOnlyOneId = true);

        string buildDeleteByLamda<TEntity>(Expression<Func<TEntity, bool>> expression,out object arguments);
        string buildSelectById<TEntity>(bool isOnlyOneId = true);
        string buildSelect<TEntity>();
        string buildSelect<TEntity>(string columns);
        string BuildSelectByLamda<TEntity>(Expression<Func<TEntity, bool>> expression, out object arguments, string columns = "");
        string BuildSelectByLamda<TEntity, TResult>(Expression<Func<TEntity, bool>> expression, out object arguments, Expression<Func<TEntity, TResult>> select,string function);

    }
}
