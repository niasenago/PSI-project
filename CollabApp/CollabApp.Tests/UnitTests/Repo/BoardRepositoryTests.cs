using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollabApp.mvc.Context;
using CollabApp.mvc.Models;
using CollabApp.mvc.Repo;
using Microsoft.EntityFrameworkCore;

namespace CollabApp.Tests.UnitTests.Repo
{
    public class BoardRepositoryTests
    {
        [Fact]
        public async Task AddEntity_SuccessfullyAddsBoard()
        {
            // Arrange
            var databaseName = Guid.NewGuid().ToString(); //kad kaskarta butu sukurta nauja db
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            using (var dbContext = new ApplicationDbContext(options))
            {
                var repository = new BoardRepository(dbContext);

                // Act
                var result = await repository.AddEntity(new Board { Id = 1, BoardName = "Board 1" });

                // Assert
                Assert.True(result);
                //Assert.Equal(1, dbContext.Set<Board>().Count()); // Assuming DbSet is Board in this case
            }
        }

        [Fact]
        public async Task UpdateEntity_SuccessfullyUpdatesBoard()
        {
            // Arrange
            var databaseName = Guid.NewGuid().ToString(); //kad kaskarta butu sukurta nauja db
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            using (var dbContext = new ApplicationDbContext(options))
            {
                dbContext.Set<Board>().Add(new Board { Id = 1, BoardName = "Board 1" });
                dbContext.SaveChanges();

                var repository = new BoardRepository(dbContext);

                // Act
                var result = await repository.UpdateEntity(new Board { Id = 1, BoardName = "Updated Board 1" });

                // Assert
                Assert.True(result);
                var updatedBoard = await dbContext.Set<Board>().FindAsync(1);
                Assert.Equal("Updated Board 1", updatedBoard.BoardName);
            }
        }
    }
}