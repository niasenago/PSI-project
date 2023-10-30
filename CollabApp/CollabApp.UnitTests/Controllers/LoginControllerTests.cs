using CollabApp.mvc.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace CollabApp.Tests.Controllers
{
    public class LoginControllerTests
    {
        [Fact]
        public void Login_ReturnsViewResult()
        {
            // Arrange
            var controller = new LoginController();

            // Act
            var result = controller.Login();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void GetUsername_Returns_Username_From_Session()
        {
            // Arrange
            var controller = new LoginController();
            var sessionMock = new Mock<ISession>();

            string expectedUsername = "testuser";
            byte[] usernameBytes = Encoding.UTF8.GetBytes(expectedUsername);

            sessionMock.Setup(s => s.TryGetValue("Username", out usernameBytes)).Returns(true);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(c => c.Session).Returns(sessionMock.Object);

            controller.ControllerContext = new ControllerContext { HttpContext = httpContextMock.Object };

            // Act
            var result = controller.GetUsername();

            // Assert
            result.Should().Be(expectedUsername);
        }

        // Other methods should be tested with integration tests
    }
}
