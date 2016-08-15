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
                var attrs = pi.GetCustomAttributes(false).FirstOrDefault(attr => attr.GetType().Name == typeof(NotMappedAttribute).Name || (attr is KeyAttribute && pi.PropertyType == typeof(int)));
                if (attrs != null) { continue; }
                //get rid of rowVersion
                if (pi.Name == _defaultRowVersion && pi.PropertyType == typeof(byte[])) { continue; }
                // get rid of identity key.  if type of propertie is int and Name is Id,then we think it is auto increment column.
                if (pi.Name.ToLower() == _defaultKey.ToLower() && pi.PropertyType == typeof(int)) { continue; }
                // get rid of the aggragation collection object 
                if ((pi.GetGetMethod().IsVirtual && pi.PropertyType.IsClass) || pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericArguments()[0].IsClass) { continue; }
                // find value object
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
                var attrs = pi.GetCustomAttributes(false).FirstOrDefault(attr => attr.GetType().Name == typeof(NotMappedAttribute).Name);
                if (attrs != null) { continue; }
                // get rid of the foreign key object 
                if ((pi.GetGetMethod().IsVirtual && pi.PropertyType.IsClass) || pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericArguments()[0].IsClass) { continue; }
                // value object
                if (pi.PropertyType.IsClass && pi.PropertyType != typeof(string) && pi.PropertyType != typeof(byte[]))
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
                if ((pi.GetGetMethod().IsVirtual && pi.PropertyType.IsClass)||pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericArguments()[0].IsClass )
                {
                    object childLists = pi.GetValue(model, null);
                    childObjects.Add(childLists);
                }
            }
            return childObjects;
        }
    }
}
