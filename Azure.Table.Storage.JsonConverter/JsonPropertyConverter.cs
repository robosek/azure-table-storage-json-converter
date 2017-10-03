using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Azure.Table.Storage.JsonConverter
{
    internal static class JsonPropertyConverter
    {
        internal static bool IsSimpleType(Type type)
        {
            return
                type.GetTypeInfo().IsValueType ||
                type.GetTypeInfo().IsPrimitive ||
                new[]
                {
                typeof(string),
                typeof(String),
                typeof(decimal),
                typeof(Decimal),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(TimeSpan),
                typeof(Guid)
                }.Contains(type) ||
                Convert.GetTypeCode(type) != TypeCode.Object;
        }

        internal static void Serialize<TEntity>(TEntity baseEntity, IDictionary<string, EntityProperty> results, Type entityType)
        {
            var baseEntityType = GetEntityType(entityType);
            var entity = Convert.ChangeType(baseEntity, baseEntityType);

            foreach (var property in entityType.GetRuntimeProperties().Where(p => !IsSimpleType(p.PropertyType)))
            {
                var entityProperty = entityType.GetRuntimeProperty(property.Name)?.GetValue(entity);

                results.Add(property.Name, new EntityProperty(JsonConvert.SerializeObject(entityProperty)));
            }
        }

        internal static void Deserialize<TEntity>(TEntity baseEntity, IDictionary<string, EntityProperty> properties, Type entityType)
        {
            var baseEntityType = GetEntityType(entityType);
            var entity = Convert.ChangeType(baseEntity, baseEntityType);

            foreach (var property in entityType.GetRuntimeProperties().Where(p => !IsSimpleType(p.PropertyType)))
            {
                var propertyName = property.Name;

                if (!properties.ContainsKey(propertyName)) continue;

                var objectValue = JsonConvert.DeserializeObject(properties[propertyName].StringValue,
                    property.PropertyType);
                entityType.GetRuntimeProperty(property.Name)?.SetValue(entity, objectValue);
            }
        }

        private static Type GetEntityType(Type entityType)
        {
            return entityType;
        }
    }
}
