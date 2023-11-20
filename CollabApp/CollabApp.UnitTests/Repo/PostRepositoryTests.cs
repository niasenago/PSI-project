using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollabApp.mvc.Context;
using CollabApp.mvc.Models;
using CollabApp.mvc.Repo;
using Microsoft.EntityFrameworkCore;

namespace CollabApp.UnitTests.Repo
{
    public class PostRepositoryTests
    {
        [Fact]
        public async Task UpdateEntity_UpdatesEntity()
        {
            // Arrange
            var databaseName = Guid.NewGuid().ToString(); //kad kaskarta butu sukurta nauja db
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            using (var dbContext = new ApplicationDbContext(options))
            {
                // Add test data to the in-memory database
                dbContext.Posts.Add(new Post { Id = 1, BoardId = 0, Title = "Post 1", AuthorId = 1, Description = "Description 1" });
                dbContext.SaveChanges();

                var repository = new PostRepository(dbContext);

                // Act
                var result = await repository.UpdateEntity(new Post { Id = 1, BoardId = 0, Title = "Updated Title", AuthorId = 1, Description = "Updated Description" });

                // Assert
                Assert.True(result);

                // Check if the entity is updated
                var updatedEntity = await dbContext.Posts.FindAsync(1);
                Assert.Equal("Updated Title", updatedEntity.Title);
                Assert.Equal("Updated Description", updatedEntity.Description);
            }
        }

        [Fact]
        public async Task UpdateEntity_ReturnsFalseForNonexistentEntity()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var dbContext = new ApplicationDbContext(options))
            {
                var repository = new PostRepository(dbContext);

                // Act
                var result = await repository.UpdateEntity(new Post { Id = 1, Title = "Updated Title", Description = "Updated Description" });

                // Assert
                Assert.False(result);
            }
        }        
    }
}