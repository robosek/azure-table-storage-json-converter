using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;

namespace Azure.Table.Storage.JsonConverter
{
    public class JsonTableEntity : TableEntity
    {
        [IgnoreProperty]
        private Type EntityType { get; }

        public JsonTableEntity(Type entityType)
        {
            EntityType = entityType;
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);
            JsonPropertyConverter.Deserialize(this, properties, EntityType);
        }

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var entity = base.WriteEntity(operationContext);
            JsonPropertyConverter.Serialize(this, entity, EntityType);
            return entity;
        }
    }
}
