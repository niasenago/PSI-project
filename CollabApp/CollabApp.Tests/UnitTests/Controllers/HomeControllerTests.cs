using CollabApp.mvc.Controllers;
using CollabApp.mvc.Models;
using CollabApp.mvc.Repo;
using CollabApp.mvc.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CollabApp.mvc.Services;


namespace CollabApp.Tests.UnitTests.Controllers
{
    public class HomeControllerTests
    {
        [Fact]
        public async Task Index_ReturnsViewResult_WithAListOfBoards()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<HomeController>>();
            var mockHttpServiceClient = new Mock<IHttpServiceClient>();

            // Set up the mock response when calling GetAsync
            mockHttpServiceClient.Setup(client => client.GetAsync(It.IsAny<string>()))
                .ReturnsAsync("[]"); // Empty array as content

            var controller = new HomeController(mockLogger.Object, mockHttpServiceClient.Object);

            // Act
            var result = await controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<Board>>(result.Model);

            var model = (List<Board>)result.Model;
            model.Should().BeEmpty();
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

            // Provide a mock for IHttpServiceClient
            var mockHttpServiceClient = new Mock<IHttpServiceClient>();
            mockHttpServiceClient.Setup(client => client.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(""); // Empty string as content

            var controller = new HomeController(null, mockHttpServiceClient.Object); // Use IHttpServiceClient

            // Act
            var result = await controller.CreateBoard(new Board { Id = 1, BoardName = "ValidBoardName", BoardDescription = "qwerty" });

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();

            var redirectToActionResult = (RedirectToActionResult)result;
            redirectToActionResult.ActionName.Should().Be("Index");

            //mockBoardRepo.Verify(repo => repo.AddEntity(It.IsAny<Board>()), Times.Once);
        }
        [Fact]
        public async Task Index_HandlesApiError_ReturnsViewWithErrorMessage()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<HomeController>>();
            var mockHttpServiceClient = new Mock<IHttpServiceClient>();
            mockHttpServiceClient.Setup(client => client.GetAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Simulated API error"));

            var controller = new HomeController(mockLogger.Object, mockHttpServiceClient.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model); // Ensure no boards are returned
            Assert.Equal("Error retrieving boards. Please try again.", controller.ViewBag.ErrorMessage);
        }
        [Fact]
        public async Task CreateBoard_InvalidBoard_ReturnsViewWithErrorMessage()
        {
            // Arrange
            var mockHttpServiceClient = new Mock<IHttpServiceClient>();
            var controller = new HomeController(null, mockHttpServiceClient.Object);

            // Set up TempData on ControllerContext
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { }
            };
            controller.TempData = new TempDataDictionary(controller.HttpContext, Mock.Of<ITempDataProvider>());

            var invalidBoard = new Board
            {
                Id = 1,
                BoardName = "fuck",
                BoardDescription = "fuck"
            };

            // Act
            var result = await controller.CreateBoard(invalidBoard);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();

            var redirectToActionResult = (RedirectToActionResult)result;
            redirectToActionResult.ActionName.Should().Be("Index");
            Assert.Equal("Profanities are not allowed.", controller.TempData["BoardErrorMessage"]);
        }

        [Fact]
        public async Task CreateBoard_ApiError_ReturnsViewWithErrorMessage()
        {
            // Arrange
            var mockHttpServiceClient = new Mock<IHttpServiceClient>();
            mockHttpServiceClient.Setup(client => client.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Simulated API error"));

            var controller = new HomeController(null, mockHttpServiceClient.Object);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { }
            };
            controller.TempData = new TempDataDictionary(controller.HttpContext, Mock.Of<ITempDataProvider>());

            var validBoard = new Board
            {
                Id = 1,
                BoardName = "ValidBoardName",
                BoardDescription = "qwerty"
            };

            // Act
            var result = await controller.CreateBoard(validBoard);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();

            var redirectToActionResult = (RedirectToActionResult)result;
            redirectToActionResult.ActionName.Should().Be("Index");
            Assert.Equal("Simulated API error", controller.TempData["BoardErrorMessage"]);
        }
    }
}
