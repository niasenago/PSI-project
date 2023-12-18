using CollabApp.SharedObjects.Dto;
using CollabApp.mvc.Controllers;
using CollabApp.mvc.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Xunit;
using CollabApp.mvc.Models;


namespace CollabApp.Tests.UnitTests.Controllers
{
    public class LoginControllerTests
    {
        private readonly Mock<IHttpServiceClient> mockHttpServiceClient;
        private readonly Mock<HttpContext> mockHttpContext;
        private readonly Mock<ISession> mockSession;
        private readonly LoginController controller;

        public LoginControllerTests()
        {
            mockHttpServiceClient = new Mock<IHttpServiceClient>();
            mockHttpContext = new Mock<HttpContext>();
            mockSession = new Mock<ISession>();
            mockHttpContext.Setup(_ => _.Session).Returns(mockSession.Object);
            controller = new LoginController(mockHttpServiceClient.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object }
            };
        }

        [Fact]
        public void Login_GetRequest_ReturnsLoginView()
        {
            // Act
            var result = controller.Login();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName); // Default view
        }

        [Fact]
        public async Task Login_ValidCredentials_RedirectsToIndex()
        {
            // Arrange
            var username = "validUsername";
            var password = "validPassword";
            var user = new User { Id = 2, Username = username, PasswordHash = password, Salt = "123456" };

            mockHttpServiceClient.Setup(client => client.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(JsonConvert.SerializeObject(user));

            // Act
            var result = await controller.Login(username, password);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsViewWithError()
        {
            // Arrange
            var username = "invalidUsername";
            var password = "invalidPassword";

            mockHttpServiceClient.Setup(client => client.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(string.Empty);

            // Act
            var result = await controller.Login(username, password);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Invalid username or password.", viewResult.ViewData["ErrorMessage"]);
        }
    }
}
