using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollabApp.mvc.Context;
using CollabApp.mvc.Models;
using CollabApp.mvc.Services;
using Microsoft.EntityFrameworkCore;

namespace CollabApp.UnitTests.Services
{
    public class PostFilterServiceTests
    {
        [Theory]
        [InlineData(null, null, null, 1, 2)]
        [InlineData("Post 1", null, null, 1, 1)]
        [InlineData("Post 2", "2022-01-01T00:00:00Z", null, 1, 1)]
        [InlineData("Post 2",null, "2023-12-01T00:00:00Z", 1, 1)]
        [InlineData(null,"2022-01-01T00:00:00Z", "2023-12-01T00:00:00Z", 1, 2)]
        [InlineData("Post 4","2022-01-01T00:00:00Z", "2023-12-01T00:00:00Z", 1, 0)]
        public void FilterPosts_ReturnsFilteredPosts(string searchTerm, 
        string startDateStr, string endDateStr, int boardId, int expectedCount)
        {
            // Arrange
            var databaseName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            using (var dbContext = new ApplicationDbContext(options))
            {
                // Seed the in-memory database with test data
                dbContext.Posts.Add(new Post { Id = 1, BoardId = 1, Title = "Post 1", AuthorId = 1, Description = "Description 1", DatePosted = DateTime.UtcNow });
                dbContext.Posts.Add(new Post { Id = 2, BoardId = 1, Title = "Post 2", AuthorId = 2, Description = "Description 2", DatePosted = DateTime.UtcNow });
                dbContext.Posts.Add(new Post { Id = 3, BoardId = 2, Title = "Post 3", AuthorId = 1, Description = "Description 3", DatePosted = DateTime.UtcNow });

                dbContext.SaveChanges();
            }

            using (var dbContext = new ApplicationDbContext(options))
            {
                var postFilterService = new PostFilterService(dbContext);

                // Convert string representations of dates to DateTime objects
                DateTime? startDate = string.IsNullOrEmpty(startDateStr) ? (DateTime?)null : DateTime.Parse(startDateStr, null, System.Globalization.DateTimeStyles.RoundtripKind);
                DateTime? endDate = string.IsNullOrEmpty(endDateStr) ? (DateTime?)null : DateTime.Parse(endDateStr, null, System.Globalization.DateTimeStyles.RoundtripKind);

                // Act
                var filteredPosts = postFilterService.FilterPosts(searchTerm,null, startDate, endDate, boardId);

                // Assert
                Assert.NotNull(filteredPosts);
                Assert.Equal(expectedCount, filteredPosts.Count);
            }
        }        
    }
}