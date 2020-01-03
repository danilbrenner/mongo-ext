using System;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDB.Extensions.OptimisticLocking
{
    public abstract class VersionedEntity<T>
    {
        [BsonId]
        public T Id { get; set; }
        public long Version { get; set; }
    }

    public abstract class MongoEntityWithGuidKey : VersionedEntity<Guid> { }
}