// using CollabApp.mvc.Controllers;
// using CollabApp.mvc.Models;
// using CollabApp.mvc.Services;
// using Microsoft.AspNetCore.Mvc;

// namespace CollabApp.UnitTests.Controllers
// {
//     public class MessageControllerTests
//     {
//         [Fact]
//         public void Messages_ReturnsJsonResult()
//         {
//             // Arrange
//             var mockDB = new Mock<IDBAccess<Message>>();
//             mockDB.Setup(db => db.GetAllItems()).Returns(new List<Message>());

//             var controller = new MessageController(mockDB.Object);

//             // Act
//             var result = controller.Messages();

//             // Assert
//             result.Should().BeOfType<JsonResult>();
//         }

//         [Fact]
//         public void MessageView_ReturnsViewResult()
//         {
//             // Arrange
//             var mockDB = new Mock<IDBAccess<Message>>();
//             mockDB.Setup(db => db.GetItemById(It.IsAny<int>())).Returns(new Message());

//             var controller = new MessageController(mockDB.Object);

//             // Act
//             var result = controller.MessageView(1);

//             // Assert
//             result.Should().BeOfType<ViewResult>();
//         }

//         [Fact]
//         public void Index_GET_ReturnsViewResult()
//         {
//             // Arrange
//             var controller = new MessageController(Mock.Of<IDBAccess<Message>>());

//             // Act
//             var result = controller.Index();

//             // Assert
//             result.Should().BeOfType<ViewResult>();
//         }

//         [Fact]
//         public async Task Index_POST_RedirectsToAction_Messages()
//         {
//             // Arrange
//             var mockDB = new Mock<IDBAccess<Message>>();
//             var controller = new MessageController(mockDB.Object);
//             var message = new Message();

//             // Act
//             var result = await controller.Index(message) as RedirectToActionResult;

//             // Assert
//             result.Should().NotBeNull();
//             result.ActionName.Should().Be("Messages");
//         }

//         [Fact]
//         public void AddMessage_CallsAddItem()
//         {
//             // Arrange
//             var mockDB = new Mock<IDBAccess<Message>>();
//             var controller = new MessageController(mockDB.Object);
//             var message = new Message();

//             // Act
//             controller.AddMessage(message);

//             // Assert
//             mockDB.Verify(db => db.AddItem(message), Times.Once);
//         }

//         [Fact]
//         public void GetAllMessages_ReturnsListOfMessages()
//         {
//             // Arrange
//             var mockDB = new Mock<IDBAccess<Message>>();
//             mockDB.Setup(db => db.GetAllItems()).Returns(new List<Message> { new Message(), new Message() });

//             var controller = new MessageController(mockDB.Object);

//             // Act
//             var result = controller.GetAllMessages();

//             // Assert
//             result.Should().HaveCount(2);
//         }

//         [Fact]
//         public void GetMessageById_ReturnsMessage()
//         {
//             // Arrange
//             var mockDB = new Mock<IDBAccess<Message>>();
//             mockDB.Setup(db => db.GetItemById(It.IsAny<int>())).Returns(new Message { Id = 1 });

//             var controller = new MessageController(mockDB.Object);

//             // Act
//             var result = controller.GetMessageById(1);

//             // Assert
//             result.Id.Should().Be(1);
//         }
//     }
// }
