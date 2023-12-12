using CollabApp.API.Controllers;
using CollabApp.mvc.Models;
using CollabApp.mvc.Repo;
using CollabApp.SharedObjects.Dto;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CollabApp.API.tests.Controllers
{
    public class BoardsControllerTests
    {
        [Fact]
        public async Task GetBoards_ReturnsOkResultWithData()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var boardRepoMock = new Mock<IBoardRepository>();

            unitOfWorkMock.Setup(u => u.BoardRepository).Returns(boardRepoMock.Object);

            var boardsController = new BoardsController(null, unitOfWorkMock.Object);

            var boards = new List<CreateBoardDto>()
            {
                new CreateBoardDto { BoardName = "Board 1", BoardDescription = "Board 1 Description" },
                new CreateBoardDto { BoardName = "Board 2", BoardDescription = "Board 2 Description" },
                new CreateBoardDto { BoardName = "Board 3", BoardDescription = "Board 3 Description" }
            };

            var boardEntities = new List<Board>()
            {
                new Board { BoardName = "Board 1", BoardDescription = "Board 1 Description" },
                new Board { BoardName = "Board 2", BoardDescription = "Board 2 Description" },
                new Board { BoardName = "Board 3", BoardDescription = "Board 3 Description" }
            };

            boardRepoMock.Setup(repo => repo.GetAllAsync(null))
                .ReturnsAsync(boardEntities);

            // Act
            var result = await boardsController.GetBoards();

            // Assert
            Assert.IsType<ActionResult<IEnumerable<Board>>>(result);

            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var boardsResult = Assert.IsType<List<Board>>(actionResult.Value);

            Assert.Equal(boards.Count, boardsResult.Count);
        }

        [Fact]
        public async Task CreateBoard_ValidBoard_ReturnsCreatedResultWithData()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var boardRepoMock = new Mock<IBoardRepository>();

            unitOfWorkMock.Setup(u => u.BoardRepository).Returns(boardRepoMock.Object);

            var boardsController = new BoardsController(null, unitOfWorkMock.Object);

            var createBoardDto = new CreateBoardDto
            {
                BoardName = "Board 1",
                BoardDescription = "Board 1 Description"
            };

            var board = new Board { BoardName = "Board 1", BoardDescription = "Board 1 Description" };

            boardRepoMock.Setup(repo => repo.GetAsync(board.Id, null))
                .ReturnsAsync(board);

            boardRepoMock.Setup(repo => repo.AddEntity(It.IsAny<Board>())).ReturnsAsync(true);

            // Act
            var result = await boardsController.CreateBoard(createBoardDto);

            // Assert
            Assert.IsType<ActionResult<Board>>(result);

            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var boardResult = Assert.IsType<Board>(actionResult.Value);

            Assert.Equal(board.Id, boardResult.Id);
            Assert.Equal(board.BoardName, boardResult.BoardName);
            Assert.Equal(board.BoardDescription, boardResult.BoardDescription);
        }

        [Fact]
        public async Task CreateBoard_InvalidBoard_ReturnsBadRequest()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var boardRepoMock = new Mock<IBoardRepository>();

            unitOfWorkMock.Setup(u => u.BoardRepository).Returns(boardRepoMock.Object);

            var boardsController = new BoardsController(null, unitOfWorkMock.Object);

            var createBoardDto = new CreateBoardDto
            {
                BoardName = "Board 1",
                BoardDescription = "Board 1 Description"
            };

            var board = new Board { BoardName = "Board 1", BoardDescription = "Board 1 Description" };

            boardRepoMock.Setup(repo => repo.AddEntity(board))
                .ReturnsAsync(false);

            // Act
            var result = await boardsController.CreateBoard(createBoardDto);

            // Assert
            Assert.IsType<ActionResult<Board>>(result);

            var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errorMessage = Assert.IsType<string>(actionResult.Value);
        }

        [Fact]
        public async Task GetBoard_ValidId_ReturnsOkResultWithData()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var boardRepoMock = new Mock<IBoardRepository>();

            unitOfWorkMock.Setup(u => u.BoardRepository).Returns(boardRepoMock.Object);

            var boardsController = new BoardsController(null, unitOfWorkMock.Object);

            var board = new Board { BoardName = "Board 1", BoardDescription = "Board 1 Description" };

            boardRepoMock.Setup(repo => repo.GetAsync(board.Id, null))
                .ReturnsAsync(board);

            // Act
            var result = await boardsController.GetBoard(board.Id);

            // Assert
            Assert.IsType<ActionResult<Board>>(result);

            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var boardResult = Assert.IsType<Board>(actionResult.Value);

            Assert.Equal(board.Id, boardResult.Id);
            Assert.Equal(board.BoardName, boardResult.BoardName);
            Assert.Equal(board.BoardDescription, boardResult.BoardDescription);
        }

        [Fact]
        public async Task GetBoard_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var boardRepoMock = new Mock<IBoardRepository>();

            unitOfWorkMock.Setup(u => u.BoardRepository).Returns(boardRepoMock.Object);

            var boardsController = new BoardsController(null, unitOfWorkMock.Object);

            var board = new Board { BoardName = "Board 1", BoardDescription = "Board 1 Description" };

            boardRepoMock.Setup(repo => repo.GetAsync(board.Id, null))
                .ReturnsAsync((Board)null);

            // Act
            var result = await boardsController.GetBoard(board.Id);

            // Assert
            Assert.IsType<ActionResult<Board>>(result);

            var actionResult = Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
