using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using OrderFlow.Controllers;
using OrderFlow.Data.Models;
using OrderFlow.Hubs;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Message;
using System.Security.Claims;

namespace OrderFlow.Tests.Controllers
{
    [TestFixture]
    public class MessageControllerTests
    {
        private Mock<IMessageService> _messageServiceMock;
        private Mock<IHubContext<MessageHub>> _hubContextMock;
        private Mock<IHubClients> _hubClientsMock;
        private Mock<IClientProxy> _clientProxyMock;
        private Mock<ILogger<MessageController>> _loggerMock;
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private MessageController _controller;

        [SetUp]
        public void Setup()
        {
            _messageServiceMock = new Mock<IMessageService>();
            _hubContextMock = new Mock<IHubContext<MessageHub>>();
            _hubClientsMock = new Mock<IHubClients>();
            _clientProxyMock = new Mock<IClientProxy>();
            _loggerMock = new Mock<ILogger<MessageController>>();
            _userManagerMock = CreateUserManagerMock();

            _hubContextMock.Setup(h => h.Clients).Returns(_hubClientsMock.Object);
            _hubClientsMock.Setup(c => c.Group(It.IsAny<string>())).Returns(_clientProxyMock.Object);
            _clientProxyMock
                .Setup(c => c.SendCoreAsync(It.IsAny<string>(), It.IsAny<object?[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _controller = new MessageController(
                _messageServiceMock.Object,
                _hubContextMock.Object,
                _loggerMock.Object,
                _userManagerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }

        [Test]
        public async Task Create_ShouldReturnOkAndBroadcastMessage_WhenModelIsValid()
        {
            var senderId = Guid.NewGuid();
            var notificationId = Guid.NewGuid();
            SetAuthenticatedUser(senderId);

            var model = new CreateNotificationMessageViewModel
            {
                Content = "Hello",
                NotificationID = notificationId
            };
            var createdMessage = new Message
            {
                MessageID = Guid.NewGuid(),
                SenderID = senderId,
                NotificationID = notificationId,
                Content = "Hello",
                SentAt = DateTime.UtcNow
            };

            _messageServiceMock
                .Setup(s => s.CreateMessageAsync(It.IsAny<CreateNotificationMessageViewModel>(), true))
                .ReturnsAsync(createdMessage);
            _userManagerMock
                .Setup(u => u.FindByIdAsync(senderId.ToString()))
                .ReturnsAsync(new ApplicationUser { Id = senderId, UserName = "SenderUser" });

            var result = await _controller.Create(model);

            Assert.That(result, Is.InstanceOf<OkResult>());
            Assert.That(model.SenderID, Is.EqualTo(senderId));
            _hubClientsMock.Verify(c => c.Group(notificationId.ToString()), Times.Once);
            _clientProxyMock.Verify(c => c.SendCoreAsync(
                "MessageAdded",
                It.Is<object?[]>(args => args.Length == 1 && args[0] is DetailsNotificationMessageViewModel),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task Create_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            _controller.ModelState.AddModelError("Content", "Required");

            var result = await _controller.Create(new CreateNotificationMessageViewModel());

            Assert.That(result, Is.InstanceOf<BadRequestResult>());
            _messageServiceMock.Verify(s => s.CreateMessageAsync(It.IsAny<CreateNotificationMessageViewModel>(), It.IsAny<bool>()), Times.Never);
        }

        [Test]
        public async Task Create_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await _controller.Create(new CreateNotificationMessageViewModel { Content = "Hello" });

            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public async Task Edit_ShouldReturnOkAndBroadcastEditedMessage_WhenMessageBelongsToUser()
        {
            var senderId = Guid.NewGuid();
            var messageId = Guid.NewGuid();
            var notificationId = Guid.NewGuid();
            SetAuthenticatedUser(senderId);

            var model = new CreateNotificationMessageViewModel
            {
                Content = "Edited",
                NotificationID = notificationId
            };
            var updatedMessage = new Message
            {
                MessageID = messageId,
                SenderID = senderId,
                NotificationID = notificationId,
                Content = "Edited",
                SentAt = DateTime.UtcNow
            };

            _messageServiceMock
                .Setup(s => s.UpdateMessageAsync(model, messageId, senderId, true))
                .ReturnsAsync(updatedMessage);
            _userManagerMock
                .Setup(u => u.FindByIdAsync(senderId.ToString()))
                .ReturnsAsync(new ApplicationUser { Id = senderId, UserName = "SenderUser" });

            var result = await _controller.Edit(model, messageId.ToString());

            Assert.That(result, Is.InstanceOf<OkResult>());
            Assert.That(model.SenderID, Is.EqualTo(senderId));
            _hubClientsMock.Verify(c => c.Group(notificationId.ToString()), Times.Once);
            _clientProxyMock.Verify(c => c.SendCoreAsync(
                "MessageEdited",
                It.Is<object?[]>(args => args.Length == 1 && args[0] is DetailsNotificationMessageViewModel),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task Edit_ShouldReturnBadRequest_WhenMessageIdIsInvalid()
        {
            SetAuthenticatedUser(Guid.NewGuid());

            var result = await _controller.Edit(new CreateNotificationMessageViewModel { Content = "Edited" }, "not-a-guid");

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Delete_ShouldReturnOkAndBroadcastDeletedMessage_WhenMessageBelongsToUser()
        {
            var senderId = Guid.NewGuid();
            var messageId = Guid.NewGuid();
            var notificationId = Guid.NewGuid();
            SetAuthenticatedUser(senderId);

            _messageServiceMock
                .Setup(s => s.DeleteMessageAsync(messageId, senderId, true))
                .ReturnsAsync(true);

            var result = await _controller.Delete(messageId, notificationId);

            Assert.That(result, Is.InstanceOf<OkResult>());
            _hubClientsMock.Verify(c => c.Group(notificationId.ToString()), Times.Once);
            _clientProxyMock.Verify(c => c.SendCoreAsync(
                "MessageDeleted",
                It.Is<object?[]>(args => args.Length == 1 && object.Equals(args[0], messageId)),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task Delete_ShouldReturnForbid_WhenServiceReturnsFalse()
        {
            var senderId = Guid.NewGuid();
            var messageId = Guid.NewGuid();
            SetAuthenticatedUser(senderId);

            _messageServiceMock
                .Setup(s => s.DeleteMessageAsync(messageId, senderId, true))
                .ReturnsAsync(false);

            var result = await _controller.Delete(messageId, Guid.NewGuid());

            Assert.That(result, Is.InstanceOf<ForbidResult>());
        }

        private void SetAuthenticatedUser(Guid userId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) },
                "TestAuth"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                store.Object,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!);
        }
    }
}
