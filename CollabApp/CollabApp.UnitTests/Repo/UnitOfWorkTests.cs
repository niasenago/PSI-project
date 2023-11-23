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
    public class UnitOfWorkTests
    {
        [Fact]
        public async Task CompleteAsync_SavesChanges()
        {
            // Arrange
            var databaseName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            using (var dbContext = new ApplicationDbContext(options))
            {
                var postRepository = new PostRepository(dbContext);
                var boardRepository = new BoardRepository(dbContext);
                var commentRepository = new CommentRepository(dbContext);
                var attachmentRepository = new AttachmentRepository(dbContext);
                var userRepository = new UserRepository(dbContext); // Add this line

                var unitOfWork = new UnitOfWork(postRepository, boardRepository, commentRepository, attachmentRepository, userRepository, dbContext);

                await unitOfWork.PostRepository.AddEntity(new Post { Id = 1, BoardId = 0, Title = "Post 1", AuthorId = 1, Description = "Description 1" });
                await unitOfWork.PostRepository.AddEntity(new Post { Id = 2, BoardId = 0, Title = "Post 2", AuthorId = 2, Description = "Description 2" });

                // Act
                await unitOfWork.CompleteAsync();

                // Assert
                var expectedPostsCountAfterChanges = 2;
                var postsCount = await dbContext.Posts.CountAsync();
                Assert.Equal(expectedPostsCountAfterChanges, postsCount);
                // Add assertions if needed
            }
        }

        [Fact]
        public void UnitOfWork_RepositoriesAreNotNull()
        {
            // Arrange
            var databaseName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            using (var dbContext = new ApplicationDbContext(options))
            {
                var postRepository = new PostRepository(dbContext);
                var boardRepository = new BoardRepository(dbContext);
                var commentRepository = new CommentRepository(dbContext);
                var attachmentRepository = new AttachmentRepository(dbContext);
                var userRepository = new UserRepository(dbContext); // Add this line

                // Act
                var unitOfWork = new UnitOfWork(postRepository, boardRepository, commentRepository, attachmentRepository, userRepository, dbContext);

                // Assert
                Assert.NotNull(unitOfWork.PostRepository);
                Assert.NotNull(unitOfWork.BoardRepository);
                Assert.NotNull(unitOfWork.CommentRepository);
                Assert.NotNull(unitOfWork.AttachmentRepository);
                Assert.NotNull(unitOfWork.UserRepository); // Add this line
            }
        }
    }
}