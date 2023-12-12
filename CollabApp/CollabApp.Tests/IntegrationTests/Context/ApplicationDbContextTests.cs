
using CollabApp.mvc.Context;
using CollabApp.mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace CollabApp.Tests.IntegrationTests.Context
{
    public class ApplicationDbContextTests
    {
        [Fact]
        public void CanSaveAndGetPost()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var dbContext = new ApplicationDbContext(options))
            {
                var post = new Post { Id = 1, BoardId = 1, Title = "Post 1", AuthorId = 1, Description = "Description 1" };
                dbContext.Posts.Add(post);
                dbContext.SaveChanges();
            }
            var postId = 1;

            using (var dbContext = new ApplicationDbContext(options))
            {
                var retrievedPost = dbContext.Posts.FirstOrDefault(post => post.Id == postId);
                Assert.NotNull(retrievedPost);
            }
        }
    }
}
