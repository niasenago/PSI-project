using CollabApp.mvc.Context;
using CollabApp.mvc.Models;
using CollabApp.mvc.Repo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabApp.Tests.IntegrationTests.Repo
{
    public class UserRepositoryTests
    {
        [Fact]
        public async Task GetUserByUsernameAsync_ReturnsUser()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var dbContext = new ApplicationDbContext(options))
            {
                dbContext.Users.Add(new User { Id = 1, Username = "TestUser", PasswordHash = "1234", Salt = "abc" });
                dbContext.Users.Add(new User { Id = 2, Username = "AnotherUser", PasswordHash = "12345", Salt = "abcd" });
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new ApplicationDbContext(options))
            {
                var userRepository = new UserRepository(dbContext);
                var result = await userRepository.GetUserByUsernameAsync("TestUser");

                Assert.NotNull(result);
                Assert.Equal("TestUser", result.Username);
            }
        }
        [Fact]
        public async Task IsUsernameTakenAsync_ReturnsBool()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var dbContext = new ApplicationDbContext(options))
            {
                dbContext.Users.Add(new User { Id = 3, Username = "TestUser", PasswordHash = "1234", Salt = "abc" });
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new ApplicationDbContext(options))
            {
                var userRepository = new UserRepository(dbContext);
                var result = await userRepository.IsUsernameTakenAsync("TestUser");

                Assert.True(result);
            }
        }
    }
}
