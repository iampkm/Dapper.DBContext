﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.DBContext.Data;
namespace Dapper.DBContext.Builder
{
    public interface IDialectBuilder
    {       
        string GetKey(Type modelType, bool isWrapDialect = true);

        string GetForeignKey(Type modelType, bool isWrapDialect = true);
        string GetTable(Type modelType, bool isWrapDialect = true);

        string GetColumn(string columnName, bool isWrapDialect = true);

        IDataBaseDialect DBDialect { get; }
    }
}
