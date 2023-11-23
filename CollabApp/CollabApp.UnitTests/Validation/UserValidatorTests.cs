
using CollabApp.mvc.Context;
using CollabApp.mvc.Models;
using CollabApp.mvc.Validation;
using CollabApp.mvc.Exceptions;
using Microsoft.EntityFrameworkCore;
using CollabApp.mvc.Repo;

namespace CollabApp.Tests.Validation
{
    public class UserValidatorTests
    {
        [Fact]
        public void UserExists_ThrowsException()
        {
            int userId = 1;

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestUsers")
                .Options;

            using (var dbContext = new ApplicationDbContext(options))
            {
                var postRepository = new PostRepository(dbContext);
                var boardRepository = new BoardRepository(dbContext);
                var commentRepository = new CommentRepository(dbContext);
                var attachmentRepository = new AttachmentRepository(dbContext);
                var userRepository = new UserRepository(dbContext); // Add this line

                var unitOfWork = new UnitOfWork(postRepository, boardRepository, commentRepository, attachmentRepository, userRepository, dbContext);
                Assert.Throws<InvalidUserException>(() => UserValidator.UserExists(unitOfWork, userId));                 
            }

        }

        [Fact]
        public void UserExists_ThrowsNoException()
        {
            int userId = 1;

            var databaseName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            using(var dbContext = new ApplicationDbContext(options))
            {
                dbContext.Users.Add(new User { Id = userId, Username = "Test1" });
                dbContext.Users.Add(new User { Id = 2, Username = "Test2" });
                dbContext.Users.Add(new User { Id = 3, Username = "Test3" });
                dbContext.SaveChanges();

                var postRepository = new PostRepository(dbContext);
                var boardRepository = new BoardRepository(dbContext);
                var commentRepository = new CommentRepository(dbContext);
                var attachmentRepository = new AttachmentRepository(dbContext);
                var userRepository = new UserRepository(dbContext); // Add this line

                var unitOfWork = new UnitOfWork(postRepository, boardRepository, commentRepository, attachmentRepository, userRepository, dbContext);

                    
                var exception = Record.Exception(() => UserValidator.UserExists(unitOfWork, userId));
                Assert.Null(exception); 
            }
        }

        [Fact]
        public void UserExistsString_ThrowsException()
        {
            string username = null;
            Assert.Throws<InvalidUserException>(() => UserValidator.UserExists(username)); 
        }

        [Fact]
        public void UserExistsString_ThrowsNoException()
        {
            string username = "TestUsername";
            var exception = Record.Exception(() => UserValidator.UserExists(username));
            Assert.Null(exception); 
        }

    }
}