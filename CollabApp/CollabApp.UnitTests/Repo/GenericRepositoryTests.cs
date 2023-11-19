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
    public class GenericRepositoryTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsListOfEntities()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var dbContext = new ApplicationDbContext(options))
            {
                // Add test data to the in-memory database
                dbContext.Posts.Add(new Post { Id = 1, BoardId = 0, Title = "Post 1",Author="Author1", Description = "Description 1" });
                dbContext.Posts.Add(new Post { Id = 2, BoardId = 0, Title = "Post 2",Author="Author2", Description = "Description 2" });
                dbContext.SaveChanges();

                var repository = new GenericRepository<Post>(dbContext);

                // Act
                var result = await repository.GetAllAsync();

                // Assert
                Assert.NotNull(result);
                Assert.IsType<List<Post>>(result);
                Assert.Equal(2, result.Count); // Adjust the count based on the added test data
            }
        }     
    }
}