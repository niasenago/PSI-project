using CollabApp.mvc.Context;
using CollabApp.mvc.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollabApp.UnitTests.Controllers
{
    public class LoginControllerTests
    {
        private readonly Mock<HttpContext> mockHttpContext;
        private readonly Mock<ISession> mockSession;
        private readonly LoginController controller;

        public LoginControllerTests()
        {
            mockHttpContext = new Mock<HttpContext>();
            mockSession = new Mock<ISession>();
            mockHttpContext.Setup(_ => _.Session).Returns(mockSession.Object);
            controller = new LoginController(null)
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
            viewResult.ViewName.Should().BeNull(); // Default view
        }

        // TODO: Test other methods in integration tests
    }
}
