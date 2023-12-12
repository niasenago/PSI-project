using System;
using System.Threading.Tasks;
using CollabApp.API.Controllers;
using CollabApp.SharedObjects.Dto;
using CollabApp.mvc.Exceptions;
using CollabApp.mvc.Models;
using CollabApp.mvc.Repo;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_ValidCredentials_ReturnsOk()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var userRepoMock = new Mock<IUserRepository>();

        unitOfWorkMock.Setup(u => u.UserRepository).Returns(userRepoMock.Object);

        var authController = new AuthController(unitOfWorkMock.Object);

        var loginDto = new LoginDto
        {
            Username = "validUsername",
            Password = "validPassword"
        };

        var user = new User
        {
            Id = 1,
            Username = "validUsername",
            PasswordHash = "validPassword",
            Salt = "salt"
        };

        userRepoMock.Setup(repo => repo.GetUserByUsernameAsync("validUsername"))
            .ReturnsAsync(user);

        // Act
        var result = await authController.Login(loginDto);

        // Assert
        Assert.IsType<ActionResult<User>>(result);
        var actionResult = Assert.IsType<OkObjectResult>(Assert.IsType<ActionResult<User>>(result).Result);
        var userResult = Assert.IsType<User>(actionResult.Value);

        Assert.Equal(user.Id, userResult.Id);
        Assert.Equal(user.Username, userResult.Username);
        // Add additional property assertions as needed
    }
}
