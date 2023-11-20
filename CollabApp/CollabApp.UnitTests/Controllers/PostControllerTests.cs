using CollabApp.mvc.Controllers;
using CollabApp.mvc.Models;
using CollabApp.mvc.Repo;
using CollabApp.mvc.Services;
using CollabApp.mvc.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;


namespace CollabApp.UnitTests.Controllers
{
    public class PostControllerTests
    {
        [Fact]
        public async Task PostsAsync_ReturnsViewWithPosts()
        {
            // Arrange
            var postFilterServiceMock = new Mock<PostFilterService>();
            //var cloudStorageServiceMock = new Mock<ICloudStorageService>();
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var notificationServiceMock = new Mock<NotificationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var controller = new PostController(
                postFilterServiceMock.Object,
                httpContextAccessorMock.Object,
                null,
                notificationServiceMock.Object,
                unitOfWorkMock.Object
            );

            var boardId = 1; // Change this to the desired boardId for testing

            var posts = new List<Post>
            {
                // Create sample posts for testing
                new Post { Id = 1, BoardId = 1, Title = "Post 1",AuthorId=1, Description = "Description 1" },
                new Post { Id = 2, BoardId = 1, Title = "Post 2",AuthorId=2, Description = "Description 2" }
                // Add more posts as needed
            };

            unitOfWorkMock.Setup(u => u.PostRepository.GetAllAsync()).ReturnsAsync(posts);

            // Act
            var result = await controller.PostsAsync(boardId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Post>>(viewResult.ViewData.Model);
            Assert.Equal(posts.Count, model.Count());
        }
        [Fact]
        public async Task PostsAsync_ReturnsViewWithPostsWhenBoardIdIsNull()
        {
            // Arrange
            var postFilterServiceMock = new Mock<PostFilterService>();
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var notificationServiceMock = new Mock<NotificationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var controller = new PostController(
                postFilterServiceMock.Object,
                httpContextAccessorMock.Object,
                null,
                notificationServiceMock.Object,
                unitOfWorkMock.Object
            );

            int? boardId = null; 

            var posts = new List<Post>
            {

                new Post { Id = 1, BoardId = 1, Title = "Post 1",AuthorId=1, Description = "Description 1" },
                new Post { Id = 2, BoardId = 1, Title = "Post 2",AuthorId=2, Description = "Description 2" }
            };

            unitOfWorkMock.Setup(u => u.PostRepository.GetAllAsync()).ReturnsAsync(posts);

            // Act
            var result = await controller.PostsAsync(boardId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Post>>(viewResult.ViewData.Model);
            Assert.Equal(posts.Count, model.Count());
        }
        [Fact]
        public void DisplayForm_WithNonNullBoardId_ReturnsViewWithPost()
        {
            // Arrange
            var postFilterServiceMock = new Mock<PostFilterService>();
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var notificationServiceMock = new Mock<NotificationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var controller = new PostController(
                postFilterServiceMock.Object,
                httpContextAccessorMock.Object,
                null,
                notificationServiceMock.Object,
                unitOfWorkMock.Object
            );

            int boardId = 1; // Set a non-null boardId for testing

            // Act
            var result = controller.DisplayForm(boardId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Post>(viewResult.ViewData.Model);

            Assert.Equal(boardId, model.BoardId);
        }
        [Theory]
        [InlineData(SortingOption.DescComments)]
        [InlineData(SortingOption.AscComments)]
        [InlineData(SortingOption.DescDate)]
        [InlineData(SortingOption.AscDate)]
        public async Task SortPosts_ReturnsSortedPostsView(SortingOption sortingOption)
        {
            // Arrange
            var postFilterServiceMock = new Mock<PostFilterService>();
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var notificationServiceMock = new Mock<NotificationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var controller = new PostController(
                postFilterServiceMock.Object,
                httpContextAccessorMock.Object,
                null,
                notificationServiceMock.Object,
                unitOfWorkMock.Object
            );

            int boardId = 1; // Set a non-null boardId for testing

            var posts = new List<Post>
            {
                new Post { Id = 1, BoardId = 1, Title = "Post 1", AuthorId = 1, Description = "Description 1" },
                new Post { Id = 2, BoardId = 1, Title = "Post 2", AuthorId = 2, Description = "Description 2" }
                // Add more posts as needed
            };

            unitOfWorkMock.Setup(u => u.PostRepository.GetAllAsync()).ReturnsAsync(posts);

            // Act
            var result = await controller.SortPosts(boardId, sortingOption);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Post>>(viewResult.ViewData.Model);

            // Add assertions based on the sorting logic
            switch (sortingOption)
            {
                case SortingOption.DescComments:
                    // Add assertions for descending comments sorting
                    break;
                case SortingOption.AscComments:
                    // Add assertions for ascending comments sorting
                    break;
                case SortingOption.DescDate:
                    // Add assertions for descending date sorting
                    break;
                case SortingOption.AscDate:
                    // Add assertions for ascending date sorting
                    break;
            }
        }

    }
}
