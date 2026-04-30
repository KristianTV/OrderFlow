using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Services.Core;
using OrderFlow.ViewModels.Message;

namespace OrderFlow.Tests.Services
{
    [TestFixture]
    public class MessageServiceTests
    {
        private OrderFlowDbContext _context;
        private MessageService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<OrderFlowDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new OrderFlowDbContext(options);
            _service = new MessageService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task CreateMessageAsync_ShouldCreateMessageSuccessfully()
        {
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var notificationId = Guid.NewGuid();
            var model = new CreateNotificationMessageViewModel
            {
                Content = "Test message",
                SenderID = senderId,
                ReceiverID = receiverId,
                NotificationID = notificationId
            };

            var result = await _service.CreateMessageAsync(model);

            var message = await _context.Messages.SingleOrDefaultAsync();
            Assert.That(result, Is.Not.Null);
            Assert.That(message, Is.Not.Null);
            Assert.That(message!.Content, Is.EqualTo("Test message"));
            Assert.That(message.SenderID, Is.EqualTo(senderId));
            Assert.That(message.ReceiverID, Is.EqualTo(receiverId));
            Assert.That(message.NotificationID, Is.EqualTo(notificationId));
            Assert.That(message.IsRead, Is.False);
            Assert.That(message.IsDeleted, Is.False);
            Assert.That(message.SentAt, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(5)));
        }

        [Test]
        public void CreateMessageAsync_ShouldThrowArgumentNullException_WhenModelIsNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateMessageAsync(null!));
        }

        [Test]
        public void CreateMessageAsync_ShouldThrowArgumentException_WhenSenderIdIsNull()
        {
            var model = new CreateNotificationMessageViewModel
            {
                Content = "Test message"
            };

            var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.CreateMessageAsync(model));
            Assert.That(ex!.Message, Does.Contain("SenderID cannot be null."));
        }

        [Test]
        public async Task GetMessageByIdAsync_ShouldReturnExistingMessage()
        {
            var message = await AddMessageAsync();

            var result = await _service.GetMessageByIdAsync(message.MessageID);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.MessageID, Is.EqualTo(message.MessageID));
        }

        [Test]
        public async Task GetMessagesByNotificationIdAsync_ShouldReturnMessagesOrderedBySentAt()
        {
            var notificationId = Guid.NewGuid();
            var senderId = Guid.NewGuid();

            var laterMessage = new Message
            {
                MessageID = Guid.NewGuid(),
                Content = "Later",
                SenderID = senderId,
                NotificationID = notificationId,
                SentAt = DateTime.UtcNow.AddMinutes(2)
            };
            var earlierMessage = new Message
            {
                MessageID = Guid.NewGuid(),
                Content = "Earlier",
                SenderID = senderId,
                NotificationID = notificationId,
                SentAt = DateTime.UtcNow.AddMinutes(1)
            };
            var otherMessage = new Message
            {
                MessageID = Guid.NewGuid(),
                Content = "Other",
                SenderID = senderId,
                NotificationID = Guid.NewGuid(),
                SentAt = DateTime.UtcNow
            };

            await _context.Messages.AddRangeAsync(laterMessage, earlierMessage, otherMessage);
            await _context.SaveChangesAsync();

            var result = (await _service.GetMessagesByNotificationIdAsync(notificationId)).ToList();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0]!.Content, Is.EqualTo("Earlier"));
            Assert.That(result[1]!.Content, Is.EqualTo("Later"));
        }

        [Test]
        public async Task UpdateMessageAsync_ShouldUpdateOwnMessageAndMarkAsUnread()
        {
            var senderId = Guid.NewGuid();
            var message = await AddMessageAsync(senderId: senderId, content: "Old content", isRead: true);
            var model = new CreateNotificationMessageViewModel { Content = "New content" };

            var result = await _service.UpdateMessageAsync(model, message.MessageID, senderId);

            Assert.That(result.Content, Is.EqualTo("New content"));
            Assert.That(result.IsRead, Is.False);
            var updatedMessage = await _context.Messages.SingleAsync(m => m.MessageID == message.MessageID);
            Assert.That(updatedMessage.Content, Is.EqualTo("New content"));
            Assert.That(updatedMessage.IsRead, Is.False);
        }

        [Test]
        public async Task UpdateMessageAsync_ShouldThrowKeyNotFoundException_WhenSenderDoesNotMatch()
        {
            var message = await AddMessageAsync(senderId: Guid.NewGuid());
            var model = new CreateNotificationMessageViewModel { Content = "New content" };

            Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateMessageAsync(model, message.MessageID, Guid.NewGuid()));
        }

        [Test]
        public async Task DeleteMessageAsync_ShouldSoftDeleteOwnMessage()
        {
            var senderId = Guid.NewGuid();
            var message = await AddMessageAsync(senderId: senderId);

            var result = await _service.DeleteMessageAsync(message.MessageID, senderId);

            Assert.That(result, Is.True);
            var deletedMessage = await _context.Messages.SingleAsync(m => m.MessageID == message.MessageID);
            Assert.That(deletedMessage.IsDeleted, Is.True);
        }

        [Test]
        public async Task DeleteMessageAsync_ShouldReturnFalse_WhenSenderDoesNotMatch()
        {
            var message = await AddMessageAsync(senderId: Guid.NewGuid());

            var result = await _service.DeleteMessageAsync(message.MessageID, Guid.NewGuid());

            Assert.That(result, Is.False);
            var unchangedMessage = await _context.Messages.SingleAsync(m => m.MessageID == message.MessageID);
            Assert.That(unchangedMessage.IsDeleted, Is.False);
        }

        [Test]
        public async Task DeleteMessageAsync_ShouldReturnFalse_WhenMessageIdIsInvalid()
        {
            var result = await _service.DeleteMessageAsync(Guid.Empty, Guid.NewGuid());

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task MarkMessageAsReadAsync_ShouldMarkUnreadMessageAsRead()
        {
            var message = await AddMessageAsync(isRead: false);

            var result = await _service.MarkMessageAsReadAsync(message.MessageID);

            Assert.That(result, Is.True);
            var readMessage = await _context.Messages.SingleAsync(m => m.MessageID == message.MessageID);
            Assert.That(readMessage.IsRead, Is.True);
        }

        [Test]
        public async Task MarkMessageAsReadAsync_ShouldReturnFalse_WhenAlreadyRead()
        {
            var message = await AddMessageAsync(isRead: true);

            var result = await _service.MarkMessageAsReadAsync(message.MessageID);

            Assert.That(result, Is.False);
        }

        private async Task<Message> AddMessageAsync(Guid? senderId = null, string content = "Message", bool isRead = false)
        {
            var message = new Message
            {
                MessageID = Guid.NewGuid(),
                Content = content,
                SenderID = senderId ?? Guid.NewGuid(),
                NotificationID = Guid.NewGuid(),
                SentAt = DateTime.UtcNow,
                IsRead = isRead
            };

            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
            return message;
        }
    }
}
