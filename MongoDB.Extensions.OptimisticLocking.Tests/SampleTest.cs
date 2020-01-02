using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Mongo2Go;
using MongoDB.Driver;
using Xunit;

namespace MongoDB.Extensions.OptimisticLocking.Tests
{
    public class SampleTest
    {
        private readonly Fixture _randomizer;

        public SampleTest()
        {
            _randomizer = new Fixture();
        }

        [Fact]
        public async Task Sample_Scenario()
        {
            // arrange
            var entityId = _randomizer.Create<long>();
            var entityName = _randomizer.Create<string>();

            using var runner = MongoDbRunner.Start();
            var client = new MongoClient(runner.ConnectionString);
            var database = client.GetDatabase("test");
            var collection = database.GetCollection<MongoEntityImpl>("entities");
            collection.InsertOne(new MongoEntityImpl{ Id = entityId, Name = entityName });

            // act
            var resp = await collection.FindAsync(e => e.Id == entityId);
            var entity = resp.FirstOrDefault();

            // assert
            entity.Should().NotBeNull();
            entity.Id.Should().Be(entityId);
            entity.Name.Should().Be(entityName);
        }
    }
}