using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            var databaseName = Guid.NewGuid().ToString(); //kad kaskarta butu sukurta nauja db
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            using (var dbContext = new ApplicationDbContext(options))
            {
                // Add test data to the in-memory database
                dbContext.Posts.Add(new Post { Id = 1, BoardId = 0, Title = "Post 1", AuthorId = 1, Description = "Description 1" });
                dbContext.Posts.Add(new Post { Id = 2, BoardId = 0, Title = "Post 2", AuthorId = 2, Description = "Description 2" });
                dbContext.SaveChanges();

                // Act
                var repository = new GenericRepository<Post>(dbContext);
                var result = await repository.GetAllAsync();

                // Assert
                Assert.NotNull(result);
                Assert.IsType<List<Post>>(result);
                Assert.Equal(2, result.Count); 
            }
        }
        [Fact]
        public async Task GetAsync_ReturnsEntityById()
        {
            // Arrange
            var databaseName = Guid.NewGuid().ToString(); 
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            using (var dbContext = new ApplicationDbContext(options))
            {
                // Add test data to the in-memory database
                dbContext.Posts.Add(new Post { Id = 1, BoardId = 0, Title = "Post 1",AuthorId=1, Description = "Description 1" });
                dbContext.Posts.Add(new Post { Id = 2, BoardId = 0, Title = "Post 2",AuthorId=2, Description = "Description 2" });
                dbContext.SaveChanges();

                var repository = new GenericRepository<Post>(dbContext);

                // Act
                var result = await repository.GetAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.IsType<Post>(result);
                Assert.Equal(1, result.Id);
                Assert.Equal("Post 1", result.Title);
            }
        }        
        [Fact]
        public async Task DeleteEntity_HandlesExceptionAndThrows()
        {
            var databaseName = Guid.NewGuid().ToString(); 
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            using (var dbContext = new ApplicationDbContext(options))
            {
                // Add test data to the in-memory database
                var post = new Post { Id = 1, BoardId = 0, Title = "Post 1", AuthorId = 1, Description = "Description 1" };
                dbContext.Posts.Add(post);
                dbContext.SaveChanges();

                var repository = new GenericRepository<Post>(dbContext);

                // Act
                var entityToDelete = post;

                // Delete the entity to create an exception (e.g., if the database is read-only)
                dbContext.Entry(entityToDelete).State = EntityState.Detached;

                bool result = false;
                try
                {
                    result = await repository.DeleteEntity(entityToDelete);
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it as needed
                    // For example, you might log it using a logging framework:
                    // logger.LogError(ex, "An error occurred during the test.");
                    Assert.True(false, "An unexpected exception occurred during the test. Exception: " + ex.Message);
                }

                // Assert
                Assert.True(result, "The DeleteEntity method should return true indicating that the entity was successfully deleted.");
                
                // Verify that the post is no longer in the database
                var remainingPosts = await dbContext.Posts.ToListAsync();
                Assert.Empty(remainingPosts);
            }
        }



        [Fact]
        public async Task DeleteEntity_RemovesEntityFromDatabase()
        {
            // Arrange
            var databaseName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            using (var dbContext = new ApplicationDbContext(options))
            {
                // Add a test entity to the in-memory database
                var post = new Post { Id = 1, BoardId = 0, Title = "Post 1", AuthorId = 1, Description = "Description 1" };
                dbContext.Posts.Add(post);
                dbContext.SaveChanges();

                // Act
                var repository = new GenericRepository<Post>(dbContext);
                var result = await repository.DeleteEntity(post);

                // Assert
                Assert.True(result); // Assert that the entity was successfully deleted

                // Verify that the entity is no longer in the database
                var deletedPost = await dbContext.Posts.FindAsync(post.Id);
                Assert.Null(deletedPost);
            }
        }
        [Fact]
        public async Task DeleteEntitiesByExpression_RemovesAllEntitiesWithBoardIdZeroFromDatabase()
        {
            // Arrange
            var databaseName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            using (var dbContext = new ApplicationDbContext(options))
            {
                var post1 = new Post { Id = 1, BoardId = 0, Title = "Post 1", AuthorId = 1, Description = "Description 1" };
                var post2 = new Post { Id = 2, BoardId = 0, Title = "Post 2", AuthorId = 2, Description = "Description 2" };
                var post3 = new Post { Id = 3, BoardId = 1, Title = "Post 3", AuthorId = 3, Description = "Description 3" };

                dbContext.Posts.AddRange(post1, post2, post3);
                dbContext.SaveChanges();

                // Act
                var repository = new GenericRepository<Post>(dbContext);
                Expression<Func<Post, bool>> expression = p => p.BoardId == 0; // condition for entities to delete
                var result = await repository.DeleteEntitiesByExpression(expression);

                // Assert
                Assert.True(result); // Assert that entities were successfully deleted

                var deletedPosts = await dbContext.Posts.Where(p => p.BoardId == 0).ToListAsync();
                Assert.Empty(deletedPosts);//all posts with boardId = 0 should be deleted, so list deletedPosts must be empty

                // Verify that the post with BoardId 1 is still in the database
                var remainingPost = await dbContext.Posts.FindAsync(post3.Id);
                Assert.NotNull(remainingPost);
            }
        }
        [Fact]
        public async Task DeleteEntitiesByExpression_HandlesExceptionAndReturnsFalse()
        {
            var databaseName = Guid.NewGuid().ToString();
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            using (var dbContext = new ApplicationDbContext(options))
            {
                dbContext.Posts.Add(new Post { Id = 1, BoardId = 0, Title = "Post 1", AuthorId = 1, Description = "Description 1" });
                dbContext.Posts.Add(new Post { Id = 2, BoardId = 0, Title = "Post 2", AuthorId = 2, Description = "Description 2" });
                dbContext.SaveChanges();

                var repository = new GenericRepository<Post>(dbContext);

                // Act
                Expression<Func<Post, bool>> expression = p => p.BoardId == 0;

                // Delete db to create an exception 
                await dbContext.Database.EnsureDeletedAsync();//deletes db

                try
                {
                    var result = await repository.DeleteEntitiesByExpression(expression);
                    // Assert
                    Assert.False(result); // The method should return false due to the exception
                }
                catch (Exception ex)
                {
                    Assert.True(false, "An unexpected exception occurred during the test. Exception: " + ex.Message);
                }
            }
        } 
        [Fact]
        public async Task AddEntity_ThrowsNotImplementedException()
        {
            var databaseName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            using (var dbContext = new ApplicationDbContext(options))
            {
                var repository = new GenericRepository<Post>(dbContext);

                // Act and Assert
                await Assert.ThrowsAsync<NotImplementedException>(async () =>
                {
                    await repository.AddEntity(new Post { Id = 1, BoardId = 0, Title = "Post 1", AuthorId = 1, Description = "Description 1" });
                });
            }
        } 
        [Fact]
        public async Task UpdateEntity_ThrowsNotImplementedException()
        {
            var databaseName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            using (var dbContext = new ApplicationDbContext(options))
            {
                var repository = new GenericRepository<Post>(dbContext);

                // Act and Assert
                await Assert.ThrowsAsync<NotImplementedException>(async () =>
                {
                    await repository.UpdateEntity(new Post { Id = 1, BoardId = 0, Title = "Post 1", AuthorId = 1, Description = "Description 1" });
                });
            }
        }         

    }

    
}