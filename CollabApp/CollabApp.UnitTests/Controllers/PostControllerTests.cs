using CollabApp.mvc.Controllers;
using CollabApp.mvc.Models;
using CollabApp.mvc.Repo;
using CollabApp.mvc.Services;
using CollabApp.mvc.Validation;
using CollabApp.mvc.Exceptions;
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
        public async Task PostViewAsync_ReturnsNotFound_WhenPostNotFound()
        {
            // Arrange
            var postId = 999; // Replace with a non-existing post ID

            var postFilterServiceMock = new Mock<PostFilterService>();
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var notificationServiceMock = new Mock<NotificationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.PostRepository.GetAsync(postId))
                          .ReturnsAsync((Post)null); // Simulate post not found

            var controller = new PostController(
                postFilterServiceMock.Object,
                httpContextAccessorMock.Object,
                null,
                notificationServiceMock.Object,
                unitOfWorkMock.Object
            );

            // Act
            var result = await controller.PostViewAsync(postId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task PostViewAsync_ReturnsViewResult_WithCorrectData()
        {
            // Arrange
            var postId = 1; // Replace with an existing post ID
            var post = new Post { Id = postId, Title = "Test Post" };
            var comments = new List<Comment> { new Comment(1, "Test Comment", postId) };

            var postFilterServiceMock = new Mock<PostFilterService>();
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var notificationServiceMock = new Mock<NotificationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.PostRepository.GetAsync(postId))
                        .ReturnsAsync(post);
            unitOfWorkMock.Setup(u => u.CommentRepository.GetAllAsync())
                        .ReturnsAsync(comments);
            unitOfWorkMock.Setup(u => u.AttachmentRepository.GetAllAsync())
                        .ReturnsAsync(new List<Attachment>()); // Mocking an empty list of attachments

            var cloudStorageServiceMock = new Mock<ICloudStorageService>();

            var controller = new PostController(
                postFilterServiceMock.Object,
                httpContextAccessorMock.Object,
                cloudStorageServiceMock.Object,
                notificationServiceMock.Object,
                unitOfWorkMock.Object
            );

            // Act
            var result = await controller.PostViewAsync(postId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Post>(viewResult.ViewData.Model);
            var commentsInViewData = Assert.IsAssignableFrom<IEnumerable<Comment>>(viewResult.ViewData["Comments"]);
            var attachmentsInViewData = Assert.IsAssignableFrom<List<Attachment>>(viewResult.ViewData["Attachments"]);

            Assert.Equal(postId, model.Id);
            Assert.Equal(post.Title, model.Title);
            Assert.Equal(comments.Count(), commentsInViewData.Count());
            Assert.Empty(attachmentsInViewData); // Ensure that Attachments list is empty in this scenario
        }
        [Fact]
        public async Task AddFilesToDatabase_UploadsFilesAndAddsToRepository()
        {
            // Arrange
            var post = new Post
            {
                Id = 1, // Replace with an existing post ID
                MediaFiles = new List<IFormFile>
                {
                    new FormFile(null, 0, 0, "file1.txt", "file1.txt"),
                    new FormFile(null, 0, 0, "file2.txt", "file2.txt"),
                    // Add more form files as needed
                }
            };

            var postFilterServiceMock = new Mock<PostFilterService>();
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var notificationServiceMock = new Mock<NotificationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.AttachmentRepository.AddEntity(It.IsAny<Attachment>()))
                          .Verifiable();

            var cloudStorageServiceMock = new Mock<ICloudStorageService>();
            cloudStorageServiceMock.Setup(c => c.UploadFileAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
                                  .ReturnsAsync("mocked_url");

            var controller = new PostController(
                postFilterServiceMock.Object,
                httpContextAccessorMock.Object,
                cloudStorageServiceMock.Object,
                notificationServiceMock.Object,
                unitOfWorkMock.Object
            );

            // Act
            await controller.AddFilesToDatabase(post);

            // Assert
            unitOfWorkMock.Verify(u => u.AttachmentRepository.AddEntity(It.IsAny<Attachment>()), Times.Exactly(post.MediaFiles.Count));
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
        [Fact]
        public async Task AddComment_ReturnsNotFound_WhenPostNotFound()
        {
            // Arrange
            var postId = 1; // Replace with a non-existing post ID
            var authorId = 1;
            var commentDescription = "Test comment";

            var postFilterServiceMock = new Mock<PostFilterService>();
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var notificationServiceMock = new Mock<NotificationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.PostRepository.GetAsync(postId))
                          .ReturnsAsync((Post)null);

            var controller = new PostController(
                postFilterServiceMock.Object,
                httpContextAccessorMock.Object,
                null,
                notificationServiceMock.Object,
                unitOfWorkMock.Object
            );

            // Act
            var result = await controller.AddComment(postId, authorId, commentDescription);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task AddComment_ValidationException_RedirectsToPostViewWithError()
        {
            // Arrange
            var postId = 1; // Replace with an existing post ID
            var authorId = 1; // Replace with an existing author ID
            var commentDescription = "fuck";

            var post = new Post { Id = postId };

            var postFilterServiceMock = new Mock<PostFilterService>();
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var notificationServiceMock = new Mock<NotificationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.PostRepository.GetAsync(postId))
                        .ReturnsAsync(post);

            var controller = new PostController(
                postFilterServiceMock.Object,
                httpContextAccessorMock.Object,
                null,
                notificationServiceMock.Object,
                unitOfWorkMock.Object
            );

            // Throw a ValidationException during the execution of AddComment
            unitOfWorkMock.Setup(u => u.CommentRepository.AddEntity(It.IsAny<Comment>()))
                        .Throws(new ProfanityException());

            // Act
            var result = await controller.AddComment(postId, authorId, commentDescription);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.Equal("PostView", redirectToActionResult.ActionName);
            Assert.Equal(postId, redirectToActionResult.RouteValues["id"]);

            // Verify ViewBag.ErrorMessage has the expected value
            Assert.NotNull(controller.ViewBag.ErrorMessage);
            Assert.Equal("Profanities are not allowed.", controller.ViewBag.ErrorMessage);
        }
        [Fact]
        public async Task AddComment_AddsCommentAndRedirectsToPostView()
        {
            // Arrange
            var postId = 1; // Replace with an existing post ID
            var authorId = 1;
            var commentDescription = "Test comment";
            
            var post = new Post { Id = postId, Title = "Test Post" };
            var commentToAdd = new Comment(authorId, commentDescription, postId);

            var postFilterServiceMock = new Mock<PostFilterService>();
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var notificationServiceMock = new Mock<NotificationService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.PostRepository.GetAsync(postId))
                        .ReturnsAsync(post);
            unitOfWorkMock.Setup(u => u.CommentRepository.AddEntity(commentToAdd))
                  .ReturnsAsync(true); // Return true or the result based on your repository implementation

            var controller = new PostController(
                postFilterServiceMock.Object,
                httpContextAccessorMock.Object,
                null,
                notificationServiceMock.Object,
                unitOfWorkMock.Object
            );

            // Act
            var result = await controller.AddComment(postId, authorId, commentDescription);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("PostView", ((RedirectToActionResult)result).ActionName);
            Assert.Equal(postId, ((RedirectToActionResult)result).RouteValues["id"]);
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
