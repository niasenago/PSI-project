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
            PasswordHash = "hashedPassword",
            Salt = "salt"
        };

        userRepoMock.Setup(repo => repo.GetUserByUsernameAsync("validUsername"))
            .ReturnsAsync(user);

        // Act
        var result = await authController.Login(loginDto);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.IsType<User>(okResult.Value);
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
        Assert.IsType<BadRequestObjectResult>(result);
        var badRequestResult = result as BadRequestObjectResult;
        Assert.NotNull(badRequestResult);
        Assert.Equal("Invalid username or password.", badRequestResult.Value);
    }

    [Fact]
    public async Task Login_ValidationException_ReturnsBadRequest()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var authController = new AuthController(unitOfWorkMock.Object);

        var loginDto = new LoginDto
        {
            Username = "validUsername",
            Password = "validPassword"
        };

        unitOfWorkMock.Setup(u => u.UserRepository)
            .Throws(new ValidationException("Validation error message"));

        // Act
        var result = await authController.Login(loginDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        var badRequestResult = result as BadRequestObjectResult;
        Assert.NotNull(badRequestResult);
        Assert.Equal("Validation error message", badRequestResult.Value);
    }
}
