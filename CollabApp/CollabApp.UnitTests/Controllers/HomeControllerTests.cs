using CollabApp.mvc.Controllers;
using CollabApp.mvc.Models;
using CollabApp.mvc.Repo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabApp.UnitTests.Controllers
{
    public class HomeControllerTests
    {
        [Fact]
        public async Task Index_ReturnsViewResult_WithAListOfBoards()
        {
            // Arrange
            var mockRepo = new Mock<IUnitOfWork>();
            mockRepo.Setup(repo => repo.BoardRepository.GetAllAsync()).ReturnsAsync(new List<Board>());
            var controller = new HomeController(null, mockRepo.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Board>>(viewResult.ViewData.Model);
            model.Should().BeEquivalentTo(new List<Board>());
        }

        [Fact]
        public void Chat_ReturnsViewResult()
        {
            // Arrange
            var controller = new HomeController( null, null);

            // Act
            var result = controller.Chat();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void Error_ReturnsViewResult_WithAnErrorViewModel()
        {
            // Arrange
            var controller = new HomeController(null, null);

            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            httpContext.TraceIdentifier = "TestTraceId";

            // Act
            var result = controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);
            Assert.Equal("TestTraceId", model.RequestId);
        }

        [Fact]
        public async Task CreateBoard_ValidBoard_RedirectsToIndex()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockBoardRepo = new Mock<IBoardRepository>();
            mockUnitOfWork.Setup(uow => uow.BoardRepository).Returns(mockBoardRepo.Object);
            var controller = new HomeController(null, mockUnitOfWork.Object);

            // Act
            var result = await controller.CreateBoard(new Board { Id = 1, BoardName = "ValidBoardName" });

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            mockBoardRepo.Verify(repo => repo.AddEntity(It.IsAny<Board>()), Times.Once);
        }
    }
}
