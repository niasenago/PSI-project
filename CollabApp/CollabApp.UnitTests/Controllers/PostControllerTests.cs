using CollabApp.mvc.Controllers;
using CollabApp.mvc.Models;
using CollabApp.mvc.Services;
using CollabApp.mvc.Validation;
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

        [Fact]
        public void AddPost_CallsAddItem()
        {
            // Arrange
            var mockDB = new Mock<IDBAccess<Post>>();
            var controller = new PostController(mockDB.Object);
            var post = new Post();

            // Act
            controller.AddPost(post);

            // Assert
            mockDB.Verify(db => db.AddItem(post), Times.Once);
        }

        [Fact]
        public void GetPostById_ReturnsPost()
        {
            // Arrange
            var mockDB = new Mock<IDBAccess<Post>>();
            mockDB.Setup(db => db.GetItemById(It.IsAny<int>())).Returns(new Post { Id = 1 });

            var controller = new PostController(mockDB.Object);

            // Act
            var result = controller.GetPostById(1);

            // Assert
            result.Id.Should().Be(1);
        }

        // Other methods should be tested with integration tests
    }
}
