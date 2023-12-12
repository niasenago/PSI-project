using System;
using System.Threading.Tasks;
using CollabApp.API.Controllers;
using CollabApp.mvc.Exceptions;
using CollabApp.mvc.Models;
using CollabApp.SharedObjects.Dto;
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

        var user = new User( "validUsername", "validPassword");

        userRepoMock.Setup(repo => repo.GetUserByUsernameAsync("validUsername"))
            .ReturnsAsync(user);

        // Act
        var result = await authController.Login(loginDto);

        // Assert
        Assert.IsType<ActionResult<User>>(result);

        var actionResult = Assert.IsType<OkObjectResult>(result.Result);
        var userResult = Assert.IsType<User>(actionResult.Value);

        Assert.Equal(user.Id, userResult.Id);
        Assert.Equal(user.Username, userResult.Username);
        // Add additional property assertions as needed
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsBadRequest()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var userRepoMock = new Mock<IUserRepository>();

        unitOfWorkMock.Setup(u => u.UserRepository).Returns(userRepoMock.Object);

        var authController = new AuthController(unitOfWorkMock.Object);

        var loginDto = new LoginDto
        {
            Username = "invalidUsername",
            Password = "invalidPassword"
        };

        userRepoMock.Setup(repo => repo.GetUserByUsernameAsync("invalidUsername"))
            .ReturnsAsync((User)null);

        // Act
        var result = await authController.Login(loginDto);

        // Assert
        Assert.IsType<ActionResult<User>>(result);

        var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var errorMessage = Assert.IsType<string>(actionResult.Value);

        Assert.Equal("Invalid username or password.", errorMessage);
    }

    [Fact]
    public async Task Login_ValidationException_ReturnsBadRequest()
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

        userRepoMock.Setup(repo => repo.GetUserByUsernameAsync("validUsername"))
            .Throws(new ValidationException("Validation error message"));

        // Act
        var result = await authController.Login(loginDto);

        // Assert
        Assert.IsType<ActionResult<User>>(result);

        var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var errorMessage = Assert.IsType<string>(actionResult.Value);

        Assert.Equal("Validation error message", errorMessage);
    }
}
