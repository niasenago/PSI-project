using CollabApp.mvc.Controllers;
using CollabApp.mvc.Models;
using CollabApp.mvc.Validation;
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
using CollabApp.mvc.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;


namespace CollabApp.Tests.UnitTests.Controllers
{
    public class RegisterControllerTests
    {
        [Fact]
        public async Task Register_ReturnsRedirectToAction_WhenModelStateIsValid()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.UserRepository.IsUsernameTakenAsync(It.IsAny<string>())).ReturnsAsync(false);

            var loggerMock = new Mock<ILogger<RegisterController>>();

            var controller = new RegisterController(loggerMock.Object, unitOfWorkMock.Object);

            var validModel = new RegisterViewModel
            {
                Username = "ValidUsername",
                Password = "SecurePassword123",
                ConfirmPassword = "SecurePassword123"
            };


            // Act
            var result = await controller.Register(validModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectToActionResult.ActionName);
            Assert.Equal("Login", redirectToActionResult.ControllerName);
        }
        [Fact]
        public async Task Register_ReturnsRedirectToAction_WhenPasswordsDoNotMatch()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<RegisterController>>();

            var controller = new RegisterController(loggerMock.Object, unitOfWorkMock.Object);

            var model = new RegisterViewModel
            {
                Username = "ValidUsername",
                Password = "ValidPassword1",
                ConfirmPassword = "InvalidPassword"
            };

            // Set up TempData on ControllerContext
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { }
            };
            controller.TempData = new TempDataDictionary(controller.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = await controller.Register(model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectToActionResult.ActionName);
            Assert.Equal("Login", redirectToActionResult.ControllerName);
            Assert.Equal("The password and confirmation password do not match.", controller.TempData["RegisterErrorMessage"]);
        }
        [Fact]
        public async Task Register_ReturnsRedirectToLogin_WhenPasswordIsInvalid()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<RegisterController>>();

            var controller = new RegisterController(loggerMock.Object, unitOfWorkMock.Object);

            var model = new RegisterViewModel
            {
                Username = "ValidUsername",
                Password = "invalid",
                ConfirmPassword = "invalid"
            };

            // Set up TempData on ControllerContext
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { }
            };
            controller.TempData = new TempDataDictionary(controller.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = await controller.Register(model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectToActionResult.ActionName);
            Assert.Equal("Login", redirectToActionResult.ControllerName);
            Assert.Equal("Password must contain both uppercase and lowercase letters, numbers, and be at least 8 characters long.", controller.TempData["RegisterErrorMessage"]);
        }

        [Fact]
        public async Task Register_ReturnsRedirectToLogin_WhenUsernameIsTaken()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(repo => repo.IsUsernameTakenAsync(It.IsAny<string>())).ReturnsAsync(true);
            unitOfWorkMock.SetupGet(uow => uow.UserRepository).Returns(userRepositoryMock.Object);

            var loggerMock = new Mock<ILogger<RegisterController>>();

            var controller = new RegisterController(loggerMock.Object, unitOfWorkMock.Object);

            var model = new RegisterViewModel
            {
                Username = "TakenUsername",
                Password = "ValidPassword1",
                ConfirmPassword = "ValidPassword1"
            };

            // Set up TempData on ControllerContext
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { }
            };
            controller.TempData = new TempDataDictionary(controller.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = await controller.Register(model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectToActionResult.ActionName);
            Assert.Equal("Login", redirectToActionResult.ControllerName);
            Assert.Equal("The username is already taken. Please choose a different one.", controller.TempData["RegisterErrorMessage"]);
        }

        [Fact]
        public async Task Register_ReturnsRedirectToLogin_WhenUsernameContainsProfanity()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(repo => repo.IsUsernameTakenAsync(It.IsAny<string>())).ReturnsAsync(false);
            unitOfWorkMock.SetupGet(uow => uow.UserRepository).Returns(userRepositoryMock.Object);

            var loggerMock = new Mock<ILogger<RegisterController>>();

            var controller = new RegisterController(loggerMock.Object, unitOfWorkMock.Object);

            var model = new RegisterViewModel
            {
                Username = "fuck",
                Password = "ValidPassword1",
                ConfirmPassword = "ValidPassword1"
            };

            // Set up TempData on ControllerContext
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { }
            };
            controller.TempData = new TempDataDictionary(controller.HttpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = await controller.Register(model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectToActionResult.ActionName);
            Assert.Equal("Login", redirectToActionResult.ControllerName);

            // Check if TempData["RegisterErrorMessage"] is not null before comparing values
            Assert.NotNull(controller.TempData["RegisterErrorMessage"]);
            Assert.Equal("Username contains profanities.", controller.TempData["RegisterErrorMessage"]);
            Assert.Equal("Username contains profanities.", controller.TempData["RegisterErrorMessage"]);
        }
        [Fact]
        public void Error_ReturnsErrorView()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<RegisterController>>();
            var controller = new RegisterController(loggerMock.Object, unitOfWorkMock.Object);

            // Act
            var result = controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error!", viewResult.ViewName);
        }
    }
}