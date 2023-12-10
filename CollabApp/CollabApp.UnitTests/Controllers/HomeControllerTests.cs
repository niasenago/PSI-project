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
using Microsoft.Extensions.Logging;

namespace CollabApp.UnitTests.Controllers
{
    public class HomeControllerTests
    {
        [Fact]
        public async Task Index_ReturnsViewResult_WithAListOfBoards()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<HomeController>>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockApiClient = new Mock<HttpClient>();
            var mockRepo = new Mock<IUnitOfWork>();

            // Set up the mock API client
            mockHttpClientFactory.Setup(factory => factory.CreateClient("Api")).Returns(mockApiClient.Object);

            // Set up the mock response when calling GetAsync
            mockApiClient.Setup(client => client.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("[]"), // Empty array as content
                });

            var controller = new HomeController(mockLogger.Object, mockHttpClientFactory.Object);

            // Act
            var result = await controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<Board>>(result.Model);

            var model = (List<Board>)result.Model;
            Assert.Empty(model);
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
            
            // Provide a mock for IHttpClientFactory
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(new System.Net.Http.HttpClient());

            var controller = new HomeController(null, mockHttpClientFactory.Object); // Use IHttpClientFactory

            // Act
            var result = await controller.CreateBoard(new Board { Id = 1, BoardName = "ValidBoardName" });

            // Assert
            Assert.IsType<RedirectToActionResult>(result);

            var redirectToActionResult = (RedirectToActionResult)result;
            Assert.Equal("Index", redirectToActionResult.ActionName);

            mockBoardRepo.Verify(repo => repo.AddEntity(It.IsAny<Board>()), Times.Once);
        }
    }
}
