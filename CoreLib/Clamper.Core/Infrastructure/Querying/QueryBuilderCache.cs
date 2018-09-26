using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Clamper.Core.Infrastructure.Models;
using Clamper.Core.Mapper;

namespace Clamper.Core.Infrastructure.Querying
{
    public static class QueryBuilderCache
    {
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> KeyProperties 
            = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> IdentityProperties 
            = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> TypeProperties 
            = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> TypeTableName 
            = new ConcurrentDictionary<RuntimeTypeHandle, string>();
        
        internal static readonly ConcurrentDictionary<string, string> SelectParts 
            = new ConcurrentDictionary<string, string>();
        
        
        internal static IEnumerable<PropertyInfo> KeyPropertiesCache(Type type)
        {
            if (KeyProperties.TryGetValue(type.TypeHandle, out var pi))
            {
                return pi;
            }

            var allProperties = TypePropertiesCache(type).ToList();
            var keyProperties = allProperties.Where(p => p.GetCustomAttributes(true).Any(a => a is KeyAttribute)).ToList();

            KeyProperties[type.TypeHandle] = keyProperties;
            return keyProperties;
        }

        internal static IEnumerable<PropertyInfo> IdentityPropertiesCache(Type type)
        {
            if (IdentityProperties.TryGetValue(type.TypeHandle, out var pi))
            {
                return pi;
            }

            var allProperties = TypePropertiesCache(type).ToList();
            var identityProperties = allProperties.Where(p => p.GetCustomAttributes(true).Any(a => a is IdentityAttribute)).ToList();

            IdentityProperties[type.TypeHandle] = identityProperties;
            return identityProperties;
        }

        internal static IEnumerable<PropertyInfo> TypePropertiesCache(Type type)
        {
            if (TypeProperties.TryGetValue(type.TypeHandle, out var pis))
            {
                return pis;
            }

            var properties = type.GetProperties()
                .Where(p => !p.Name.StartsWith("__"))
                .Where(IsWriteable).ToList();
            TypeProperties[type.TypeHandle] = properties;
            return properties;
        }
        
        internal static (string name, string columnList, string parametersList) GetInsertParameters(BaseModel entityToInsert, QueryStrategy strategy)
        {
            var type = entityToInsert.GetType();

            var name = GetTableName(type);

            var sbColumnList = new StringBuilder(null);

            var allProperties = TypePropertiesCache(type).ToList();
            var identityProperties = IdentityPropertiesCache(type).ToList();
            var allPropertiesExceptIndentity = allProperties.Except(identityProperties).ToList();

            var index = 0;
            var lst = allPropertiesExceptIndentity;
            foreach (var property in lst)
            {
                sbColumnList.Append(strategy.Enclose(property.Name));
                if (index < lst.Count - 1)
                    sbColumnList.Append(", ");
                index++;
            }

            index = 0;
            var sbParameterList = new StringBuilder(null);

            foreach (var property in lst)
            {
                sbParameterList.Append($"@{property.Name}");
                if (index < lst.Count - 1)
                    sbParameterList.Append(", ");
                index++;
            }

            return (name, sbColumnList.ToString(), sbParameterList.ToString());
        }
        
        internal static string GetTableName(Type type)
        {
            if (TypeTableName.TryGetValue(type.TypeHandle, out var name)) return name;
            name = type.Name + "s";
            if (type.GetTypeInfo().IsInterface && name.StartsWith("I"))
                name = name.Substring(1);

            var tableattr = type.GetTypeInfo().GetCustomAttributes(false).SingleOrDefault(attr => attr.GetType().Name == "TableAttribute") as
                dynamic;
            if (tableattr != null)
                name = tableattr.Name;
            TypeTableName[type.TypeHandle] = name;
            return name;
        }
        
        
        private static bool IsWriteable(PropertyInfo pi)
        {
            var attributes = pi.GetCustomAttributes(typeof(WriteAttribute), false).ToList();
            if (attributes.Count != 1)
                return true;
            var write = (WriteAttribute)attributes.First();
            return write.Write;
        }
    }
}
