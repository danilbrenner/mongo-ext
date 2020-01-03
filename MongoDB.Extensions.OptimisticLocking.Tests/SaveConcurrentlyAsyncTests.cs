using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using MongoDB.Driver;
using Xunit;

namespace MongoDB.Extensions.OptimisticLocking.Tests
{
    public class SaveConcurrentlyAsyncTests  : MongoTestBase
    {
        private readonly Guid _entityId;
        private readonly string _entityName;
        private readonly IMongoCollection<MongoEntityImpl> _collection;

        public SaveConcurrentlyAsyncTests()
        {
            _entityId = Randomizer.Create<Guid>();
            _entityName = Randomizer.Create<string>();

            _collection = NewCollection<MongoEntityImpl>();

            _collection.SaveConcurrentlyAsync(
                new MongoEntityImpl{ Id = _entityId, Name = Randomizer.Create<string>() }).Wait();
        }

        [Fact]
        public async Task SaveEntity_WhenNewEntity()
        {
            // arrange
            var newEntityId = Randomizer.Create<Guid>();

            // act
            await _collection.SaveConcurrentlyAsync(new MongoEntityImpl{ Id = newEntityId, Name = _entityName });

            // assert
            var entity = await GetEntity(_collection, e => e.Id == newEntityId);
            entity.Should().NotBeNull();
            entity.Id.Should().Be(newEntityId);
            entity.Name.Should().Be(_entityName);
            entity.Version.Should().Be(1);
        }

        [Fact]
        public async Task SaveEntity_WhenUpdating_NoConflicts()
        {
            // arrange

            // act
            var item = await GetEntity(_collection, e => e.Id == _entityId);
            item.Name = _entityName;
            await _collection.SaveConcurrentlyAsync(item);

            // assert
            var entity = await GetEntity(_collection, e => e.Id == _entityId);
            entity.Should().NotBeNull();
            entity.Id.Should().Be(_entityId);
            entity.Name.Should().Be(_entityName);
            entity.Version.Should().Be(2);
        }

        [Fact]
        public async Task SaveEntity_WhenUpdating_Conflict()
        {
            // arrange

            // act
            var item1 = await GetEntity(_collection, e => e.Id == _entityId);
            item1.Name = _entityName;
            var item2 = await GetEntity(_collection, e => e.Id == _entityId);
            item2.Name = _entityName;

            await _collection.SaveConcurrentlyAsync(item1);
            Action act = () => _collection.SaveConcurrentlyAsync(item2).Wait();

            // assert
            var entity = await GetEntity(_collection, e => e.Id == _entityId);
            entity.Should().NotBeNull();
            entity.Id.Should().Be(_entityId);
            entity.Name.Should().Be(_entityName);
            entity.Version.Should().Be(2);

            act.Should().Throw<MongoWriteException>();
        }
    }
}