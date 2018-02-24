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
        static readonly ConcurrentDictionary<Type, List<PropertyInfo>> _entityCache = new ConcurrentDictionary<Type, List<PropertyInfo>>();
        static string _defaultKey;
        static string _defaultRowVersion;
        static ReflectionHelper()
        {
            _defaultKey = Settings.PrimaryKey;
            _defaultRowVersion = Settings.Timestamp;
        }

        /// <summary>
        /// 获取实体属性列，包含所有外键关系对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<PropertyInfo> GetPropertyInfos(Type type)
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();
            if (_entityCache.TryGetValue(type, out properties)) return properties.ToList();
            properties = type.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public).ToList();
            _entityCache[type] = properties;
            return properties;
        }

        public static string GetTableName(Type entityType)
        {
            var attr = entityType.GetCustomAttribute<TableAttribute>();        
            return attr == null ? entityType.Name : attr.Name;            
        }

        public static string GetKeyName(Type entityType)
        {
            var column = GetKeyColumn(entityType);
            return column == null ? "" : column.Name;
        }

        public static string GetColumnName(PropertyInfo property)
        {
            var attr = property.GetCustomAttribute<ColumnAttribute>();
            return attr == null ? property.Name : attr.Name;
        }
        /// <summary>
        /// 返回属性名对应的列名，属性不属于表，将抛出异常
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static string GetColumnName(string propertyName, Type entityType)
        { 
            var properties = GetTableColumns(entityType);
            var column = properties.Where(pi => pi.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (column == null) { throw new Exception(string.Format("属性{0}不能映射到表", propertyName)); }
            return GetColumnName(column);
        }
              

        /// <summary>
        /// 判断实体中是否有自增主键
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static bool ExistsAutoIncrementKey(Type entityType)
        {
            var properties = GetPropertyInfos(entityType);
            return properties.Exists(pi => IsKeyAndAutoIncrement(pi));            
        }
        /// <summary>
        ///  判断是否主键，且为自增长
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool IsKeyAndAutoIncrement(PropertyInfo property)
        {
             var keyAttr= property.GetCustomAttribute<KeyAttribute>();
             if (keyAttr == null)
             {
                 return (property.Name.Equals(_defaultKey, StringComparison.OrdinalIgnoreCase)&& IsInteger(property.PropertyType));
             }
             else
             {
                 return (keyAttr.AutoIncrement && IsInteger(property.PropertyType));
             }
        }
        /// <summary>
        ///  获取自增主键列
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static PropertyInfo GetAutoIncrementColumn(Type entityType)
        {
            var properties = GetPropertyInfos(entityType);
            var column= properties.FirstOrDefault(pi => IsKeyAndAutoIncrement(pi));
            return column;
        }
        /// <summary>
        /// 根据名字查找列
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static PropertyInfo GetColumn(Type entityType, string columnName)
        {
            var properties = GetPropertyInfos(entityType);
            var column = properties.FirstOrDefault(pi => pi.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase)&&pi.PropertyType.IsSimpleType());
            return column;
        }

        /// <summary>
        /// Is Primary key 
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool IsKeyColumn(PropertyInfo property)
        {
            var keyAttr = property.GetCustomAttribute<KeyAttribute>();
            // has keyAttribute       keyName = default setting 
            return (keyAttr != null || property.Name.Equals(_defaultKey, StringComparison.OrdinalIgnoreCase));            
        }

        public static PropertyInfo GetKeyColumn(Type entityType)
        {
            var properties = GetPropertyInfos(entityType);
            var column = properties.FirstOrDefault(pi => IsKeyColumn(pi));
            return column;
        }

        /// <summary>
        /// 是否为不需要映射到数据库列
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool IsNotMappedColumn(PropertyInfo property)
        {
            var attr= property.GetCustomAttribute<NotMappedAttribute>();
            return attr != null;          
        }

        public static bool IsForeignColumn(PropertyInfo property)
        {          
            // 1:N
            if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericArguments()[0].IsClass 
                && property.PropertyType.GetGenericArguments()[0] != typeof(string)) {
                    return true;
            }
            // 1:1
            if (property.GetGetMethod().IsVirtual && property.PropertyType.IsClass && property.PropertyType != typeof(string))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        ///  是否为并发行版本列
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool IsRowVersionColumn(PropertyInfo property)
        {
            return property.Name.Equals(_defaultRowVersion, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ExistsRowVersion(Type entityType)
        { 
            return GetPropertyInfos(entityType).Exists(pi => IsRowVersionColumn(pi));
        }

        /// <summary>
        /// 类型字段是否整型
        /// </summary>
        /// <param name="fieldType"></param>
        /// <returns></returns>
        private static bool IsInteger(Type fieldType)
        {
            return (fieldType == typeof(int) || fieldType == typeof(long));            
        }

        /// <summary>
        ///  获取构建Insert，Update，sql语句 的实体列名
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static List<string> GetBuildSqlProperties(Type entityType)
        {
            var propertieInfos = GetPropertyInfos(entityType);
            var properties = new List<string>();
            foreach (PropertyInfo pi in propertieInfos)
            {               
                if (IsNotMappedColumn(pi)) { continue; }
                //get rid of rowVersion
                if (IsRowVersionColumn(pi)) { continue; }
                // get rid of identity key. 
                if (IsKeyAndAutoIncrement(pi)) { continue; }

                if (pi.PropertyType.IsSimpleType()) {
                   properties.Add(GetColumnName(pi)); // 启用自定义列名
                   // properties.Add(pi.Name);  //实体名和列名一致
                }
            }
            return properties;
        }

        public static List<string> GetSelectSqlProperties(Type entityType)
        {
            //var propertieInfos = GetPropertyInfos(entityType);
            //var properties = new List<string>();
            //foreach (PropertyInfo pi in propertieInfos)
            //{
            //    if (IsNotMappedColumn(pi)) { continue; }
            //    if (pi.PropertyType.IsSimpleType())
            //    {
            //        properties.Add(GetColumnName(pi)); // 启用自定义列名
            //       // properties.Add(pi.Name);  //实体名和列名一致
            //    }
            //}
            //return properties;
            var propertieInfos = GetTableColumns(entityType);
            return propertieInfos.Select(pi => GetColumnName(pi)).ToList();

        }
        /// <summary>
        ///  获取实体与数据表对应的列属性
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static List<PropertyInfo> GetTableColumns(Type entityType)
        {
            var propertieInfos = GetPropertyInfos(entityType);
            var properties = new List<PropertyInfo>();
            foreach (PropertyInfo pi in propertieInfos)
            {
                if (IsNotMappedColumn(pi)) { continue; }
                if (pi.PropertyType.IsSimpleType())
                {
                    properties.Add(pi);                   
                }
            }
            return properties;
        }
        /// <summary>
        /// 返回update set 字段
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static List<PropertyInfo> GetUpdateColumns(Type entityType)
        {
            var propertieInfos = GetPropertyInfos(entityType);
            var properties = new List<PropertyInfo>();
            foreach (PropertyInfo pi in propertieInfos)
            {
                if (IsNotMappedColumn(pi)) { continue; }
                if (IsRowVersionColumn(pi)) { continue; }
                // get rid of primary key.
                if (IsKeyColumn(pi)) continue;
                if (pi.PropertyType.IsSimpleType())
                {
                    properties.Add(pi);
                }
            }
            return properties;
        }

        public static List<object> GetForeignObject<T>(T entity)
        {
            List<object> childObjects = new List<object>();
            var propertieInfos = GetPropertyInfos(entity.GetType());
            foreach (var pi in propertieInfos)
            {
                if (IsForeignColumn(pi))
                {
                    object childLists = pi.GetValue(entity, null);
                    childObjects.Add(childLists);
                }
            }
            return childObjects;
        }

        /// <summary>
        ///  序列化对象属性和值
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string Serialize(object entity)
        {

            var propInfos = GetPropertyInfos(entity.GetType());
            string keyValue = "";
            if (propInfos.Count == 0)
            {
                keyValue = entity.ToString();
            }
            foreach (var pi in propInfos)
            {
                object value = new object();
                if (pi == null) value = string.Empty;
                value = pi.GetValue(entity, null);
                keyValue += string.Format("{0} : {1},", pi.Name, value);
            }
            return keyValue;
        }

        /// <summary>
        /// 给实体自增主键赋值
        /// </summary>
        /// <param name="entity"></param>
        public static void SetPrimaryKey(object entity, object value)
        {
            var Id = GetAutoIncrementColumn(entity.GetType());
            if (Id != null)
            {
                Id.SetValue(entity, value);
            }
        }

        public static void SetForeignKey(object entity,string foreignKeyName, object value)
        {
            var column = GetColumn(entity.GetType(), foreignKeyName);
            if (column != null) {
                column.SetValue(entity, value);
            }
        }


    }
}
