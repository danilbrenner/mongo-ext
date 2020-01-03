namespace MongoDB.Extensions.OptimisticLocking.Tests
{
    public class MongoEntityImpl : MongoEntityWithGuidKey
    {
        public string Name { get; set; }
    }
}