using Dapper.DBContext.Schema;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.DBContext.Helper
{
    public class ReflectionHelper
    {
        private static readonly ConcurrentDictionary<Type, List<PropertyInfo>> _paramCache = new ConcurrentDictionary<Type, List<PropertyInfo>>();
        private static string _defaultKey = "Id";
        private static string _defaultRowVersion = "RowVersion";
        public static List<PropertyInfo> GetPropertyInfos(object obj)
        {
            if (obj == null)
            {
                return new List<PropertyInfo>();
            }
            return GetPropertyInfos(obj.GetType());
        }

        public static List<PropertyInfo> GetPropertyInfos(Type type)
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();
            if (_paramCache.TryGetValue(type, out properties)) return properties.ToList();
            properties = type.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public).ToList();
            _paramCache[type] = properties;
            return properties;
        }

        public static string GetTableName(Type modelType)
        {
            var table = modelType.GetCustomAttributes(true).SingleOrDefault(attr => attr.GetType().Name == typeof(TableAttribute).Name) as dynamic;
            if (table != null)
            {
                return table.Name;
            }
            return modelType.Name;
        }

        public static string GetKeyName(Type modelType)
        {
            var properties = GetPropertyInfos(modelType);
            var key = properties.Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name)).FirstOrDefault();
            if (key != null)
            {
                return key.Name;
            }
            return _defaultKey;
        }

        public static List<string> GetBuildSqlProperties(Type typeModel)
        {
            var propertieInfos = GetPropertyInfos(typeModel);
            var properties = new List<string>();
            foreach (PropertyInfo pi in propertieInfos)
            {
                var attrs = pi.GetCustomAttributes(false);
                if (attrs != null || attrs.Length != 0)
                {
                    bool isSkip = false;
                    foreach (var attr in attrs)
                    {
                        if (attr is NotMappedAttribute)
                        {
                            isSkip = true;
                            break;
                        }
                        else if (attr is KeyAttribute && pi.PropertyType == typeof(int))  //标记了key 属性，且为int 型 就默认为自增长字段
                        {
                            isSkip = true;
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    if (isSkip) { continue; }
                };
                if (pi.Name == _defaultRowVersion && pi.PropertyType == typeof(byte[])) { continue; } // 行版本排除
                // get rid of identity key
                if (pi.Name == _defaultKey && pi.PropertyType == typeof(int)) // 属性名为约定的 值，且为int 型 就默认为自增长字段 
                {
                    continue; //自增跳过
                }
                if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericArguments()[0].IsClass)
                {
                    continue;  // 泛型集合跳过
                }
                // value object
                if (pi.PropertyType.IsClass && pi.PropertyType != typeof(string))
                {
                    properties.AddRange(GetBuildSqlProperties(pi.PropertyType));
                    continue;
                }
                properties.Add(pi.Name);
            }
            return properties;
        }

        public static List<string> GetSelectSqlProperties(Type typeModel)
        {
            var propertieInfos = GetPropertyInfos(typeModel);
            var properties = new List<string>();
            foreach (PropertyInfo pi in propertieInfos)
            {
                var attrs = pi.GetCustomAttributes(false);
                if (attrs != null || attrs.Length != 0)
                {
                    bool isSkip = false;
                    foreach (var attr in attrs)
                    {
                        if (attr is NotMappedAttribute)
                        {
                            isSkip = true;
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    if (isSkip) { continue; }
                };
                if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericArguments()[0].IsClass)
                {
                    continue;  // 泛型集合跳过
                }
               
                // value object
                if (pi.PropertyType.IsClass && pi.PropertyType != typeof(string))
                {
                    properties.AddRange(GetSelectSqlProperties(pi.PropertyType));
                    continue;
                }
                properties.Add(pi.Name);
            }
            return properties;
        }

        public static List<object> GetForeignObject<T>(T model)
        {
            List<object> childObjects = new List<object>();
            var propertieInfos = GetPropertyInfos(model);
            foreach (var pi in propertieInfos)
            {
                if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericArguments()[0].IsClass)
                {
                    object childLists = pi.GetValue(model, null);
                    childObjects.Add(childLists);
                }
            }
            return childObjects;
        }
    }
}
