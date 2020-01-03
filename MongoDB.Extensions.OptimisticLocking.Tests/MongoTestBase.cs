using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture;
using Mongo2Go;
using MongoDB.Driver;

namespace MongoDB.Extensions.OptimisticLocking.Tests
{
    public abstract class MongoTestBase : IDisposable
    {
        private readonly MongoDbRunner _runner;
        private readonly IMongoDatabase _database;

        protected readonly Fixture Randomizer;

        protected MongoTestBase()
        {
            Randomizer = new Fixture();

            _runner = MongoDbRunner.Start();
            var client = new MongoClient(_runner.ConnectionString);
            _database = client.GetDatabase(Randomizer.Create<string>());
        }

        protected IMongoCollection<T> NewCollection<T>()
            => _database.GetCollection<T>(Randomizer.Create<string>());

        protected async Task<T> GetEntity<T>(IMongoCollection<T> collection, Expression<Func<T, bool>> filter)
        {
            var resp = await collection.FindAsync(filter);
            var entity = resp.FirstOrDefault();
            return entity;
        }

        public void Dispose()
        {
            _runner.Dispose();
        }
    }
}