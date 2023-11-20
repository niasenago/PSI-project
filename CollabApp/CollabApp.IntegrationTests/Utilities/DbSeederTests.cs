using CollabApp.mvc.Context;
using CollabApp.mvc.Utilities;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;

namespace CollabApp.IntegrationTests.Utilities
{
    public class DbSeederTests
    {
        private ApplicationDbContext CreateDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public void SeedSampleData_ShouldSeedDataCorrectly()
        {
            // Arrange
            using var context = CreateDbContext(nameof(SeedSampleData_ShouldSeedDataCorrectly));
            var seeder = new DatabaseSeeder(context);

            // Act
            seeder.SeedSampleData();

            // Assert
            context.Users.Count().Should().Be(5);
            context.Posts.Count().Should().Be(2);
            context.Comments.Count().Should().Be(3);
        }

        [Fact]
        public void SeedSampleData_ShouldBeIdempotent()
        {
            // Arrange
            using var context = CreateDbContext(nameof(SeedSampleData_ShouldBeIdempotent));
            var seeder = new DatabaseSeeder(context);

            // Act
            seeder.SeedSampleData(); // First seeding
            Action secondCall = () => seeder.SeedSampleData(); // Second seeding

            // Assert
            secondCall.Should().Throw<InvalidOperationException>();
        }
    }
}
