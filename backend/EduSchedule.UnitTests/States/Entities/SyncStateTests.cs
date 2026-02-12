using Bogus;
using EduSchedule.Domain.States.Entities;
using FluentAssertions;

namespace EduSchedule.UnitTests.States.Entities
{
    public class SyncStateTests
    {
        private readonly Faker _faker = new Faker();

        [Fact]
        public void Constructor_ShouldInitializeProperties_AndSetTimestamp()
        {
            // Arrange
            var entityName = _faker.Random.Word();
            var nextLink = _faker.Internet.Url();

            // Act
            var syncState = new SyncState(entityName, nextLink);

            // Assert
            syncState.EntityName.Should().Be(entityName);
            syncState.NextLink.Should().Be(nextLink);
            syncState.LastSync.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task UpdateNextLink_ShouldUpdateLink_AndRefreshTimestamp()
        {
            // Arrange
            var syncState = new SyncState("Users", "http://api.com/page1");
            var dataAntiga = syncState.LastSync;
            var novoLink = _faker.Internet.Url();

            await Task.Delay(10); 

            // Act
            syncState.UpdateNextLink(novoLink);

            // Assert
            syncState.NextLink.Should().Be(novoLink);
            syncState.LastSync.Should().BeAfter(dataAntiga);
            syncState.LastSync.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }
    }
}