using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace MongoDB.Extensions.OptimisticLocking
{
    public static class CollectionExtensions
    {
        public static async Task SaveConcurrentlyAsync<T>(
            this IMongoCollection<T> collection, T document, CancellationToken cancellationToken = default)
            where T : MongoEntityWithGuidKey
        {
            var currentVersion = document.Version;
            document.Version += 1;
            await collection.ReplaceOneAsync(
                    e => e.Id == document.Id && e.Version == currentVersion,
                    document,
                    new ReplaceOptions{ IsUpsert = true, BypassDocumentValidation = false },
                    cancellationToken)
                .ConfigureAwait(false);
        }
    }
}