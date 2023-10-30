using CollabApp.mvc.Controllers;
using CollabApp.mvc.Models;
using CollabApp.mvc.Services;
using Microsoft.AspNetCore.Mvc;

namespace CollabApp.UnitTests.Controllers
{
    public class PostControllerTests
    {
        [Fact]
        public void Posts_ReturnsViewResult()
        {
            // Arrange
            var mockDB = new Mock<IDBAccess<Post>>();
            mockDB.Setup(db => db.GetAllItems()).Returns(new List<Post>());

            var controller = new PostController(mockDB.Object, new PostFilterService(mockDB.Object));

            // Act
            var result = controller.Posts();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void PostView_ReturnsViewResult()
        {
            // Arrange
            var mockDB = new Mock<IDBAccess<Post>>();
            mockDB.Setup(db => db.GetItemById(It.IsAny<int>())).Returns(new Post());

            var controller = new PostController(mockDB.Object, new PostFilterService(mockDB.Object));

            // Act
            var result = controller.PostView(1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }
        /*
        [Fact]
        public async Task Index_POST_WithValidPost_RedirectsToAction_Posts()
        {
            // Arrange
            var mockDB = new Mock<IDBAccess<Post>>();
            var controller = new PostController(mockDB.Object, new PostFilterService(mockDB.Object));
            var post = new Post { Title = "Valid Title", Description = "Valid Description" };

            // Act
            var result = await controller.Index(post) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Posts");
        }
        */
        /*
        [Fact]
        public async Task Index_POST_WithInvalidPost_ReturnsViewResultWithErrorMessage()
        {
            // Arrange
            var mockDB = new Mock<IDBAccess<Post>>();
            var controller = new PostController(mockDB.Object, new PostFilterService(mockDB.Object));
            var post = new Post { Title = "", Description = "" };

            // Act
            var result = await controller.Index(post) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().Be("Index");
            result.ViewData["ErrorMessage"].Should().NotBeNull();
        }
        */

    }
}
