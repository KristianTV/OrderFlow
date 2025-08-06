using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Services.Core;
using OrderFlow.ViewModels.Notification;

namespace OrderFlow.Tests.Services
{
    [TestFixture]
    public class NotificationServiceTests
    {
        private OrderFlowDbContext _context;
        private NotificationService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<OrderFlowDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new OrderFlowDbContext(options);
            _service = new NotificationService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private async Task<ApplicationUser> AddUser(string username)
        {
            var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = username, Email = $"{username}@test.com" };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        [Test]
        public async Task CreateNotificationAsync_ShouldCreateNotificationSuccessfully()
        {

            var sender = await AddUser("SenderUser");
            var receiver = await AddUser("ReceiverUser");
            var createNotificationViewModel = new CreateNotificationViewModel
            {
                Title = "Test Notification",
                Message = "This is a test message.",
                ReceiverId = receiver.Id,
                OrderId = Guid.NewGuid()
            };


            await _service.CreateNotificationAsync(createNotificationViewModel, sender.Id);


            var notification = await _context.Notifications.SingleOrDefaultAsync();
            Assert.That(notification, Is.Not.Null);
            Assert.That(notification.Title, Is.EqualTo("Test Notification"));
            Assert.That(notification.Message, Is.EqualTo("This is a test message."));
            Assert.That(notification.ReceiverId, Is.EqualTo(receiver.Id));
            Assert.That(notification.SenderId, Is.EqualTo(sender.Id));
            Assert.That(notification.IsRead, Is.False);
            Assert.That(notification.IsDeleted, Is.False);
            Assert.That(notification.CreatedAt.Date, Is.EqualTo(DateTime.UtcNow.Date));
        }

        [Test]
        public void CreateNotificationAsync_ShouldThrowArgumentNullException_WhenViewModelIsNull()
        {
            var senderId = Guid.NewGuid();

            Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateNotificationAsync((CreateNotificationViewModel)null!, senderId));
        }

        [Test]
        public void CreateNotificationAsync_ShouldThrowArgumentException_WhenReceiverIdIsEmpty()
        {
            var senderId = Guid.NewGuid();
            var createNotificationViewModel = new CreateNotificationViewModel
            {
                Title = "Test",
                Message = "Message",
                ReceiverId = Guid.Empty
            };

            var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.CreateNotificationAsync(createNotificationViewModel, senderId));
            Assert.That(ex.Message, Does.Contain("Receiver ID cannot be empty."));
        }

        [Test]
        public async Task CreateNotificationAsync_Admin_ShouldCreateNotificationSuccessfully()
        {
            var sender = await AddUser("AdminUser");
            var receiver = await AddUser("DriverUser");
            var createNotificationViewModel = new AdminCreateNotificationViewModel
            {
                Title = "Admin Notification",
                Message = "Admin message.",
                ReceiverId = receiver.Id,
                OrderId = Guid.NewGuid(),
                TruckId = Guid.NewGuid()
            };

            await _service.CreateNotificationAsync(createNotificationViewModel, sender.Id);

            var notification = await _context.Notifications.SingleOrDefaultAsync();
            Assert.That(notification, Is.Not.Null);
            Assert.That(notification.Title, Is.EqualTo("Admin Notification"));
            Assert.That(notification.ReceiverId, Is.EqualTo(receiver.Id));
            Assert.That(notification.SenderId, Is.EqualTo(sender.Id));
            Assert.That(notification.TruckId, Is.Not.Null);
        }

        [Test]
        public async Task GetAllNotificationsAsync_ShouldReturnNotificationsForDriver()
        {
            var driver = await AddUser("Driver1");
            var admin = await AddUser("Admin1");
            await _context.Notifications.AddRangeAsync(
                new Notification { Id = Guid.NewGuid(), Title = "New Order", ReceiverId = driver.Id, SenderId = admin.Id, CreatedAt = DateTime.UtcNow.AddMinutes(-5), IsRead = false, Sender = admin },
                new Notification { Id = Guid.NewGuid(), Title = "Completed Delivery", ReceiverId = driver.Id, SenderId = admin.Id, CreatedAt = DateTime.UtcNow.AddMinutes(-10), IsRead = true, Sender = admin },
                new Notification { Id = Guid.NewGuid(), Title = "Admin Message", ReceiverId = admin.Id, SenderId = driver.Id, CreatedAt = DateTime.UtcNow.AddMinutes(-1), IsRead = false, Sender = driver }
            );
            await _context.SaveChangesAsync();

            var notifications = await _service.GetAllNotificationsAsync(driver.Id);

            Assert.That(notifications, Is.Not.Null);
            var notificationList = notifications.ToList();
            Assert.That(notificationList.Count, Is.EqualTo(3));
            Assert.That(notificationList.Any(n => n.Title == "New Order"), Is.True);
            Assert.That(notificationList.Any(n => n.Title == "Completed Delivery"), Is.True);
            Assert.That(notificationList.Any(n => n.Title == "Admin Message"), Is.True);

            Assert.That(notificationList[0].IsRead, Is.False);
            Assert.That(notificationList[1].IsRead, Is.False);
            Assert.That(notificationList[2].IsRead, Is.True);

            Assert.That(notificationList[0].Title, Is.EqualTo("Admin Message"));
            Assert.That(notificationList[1].Title, Is.EqualTo("New Order"));
            Assert.That(notificationList[2].Title, Is.EqualTo("Completed Delivery"));

            Assert.That(notificationList[0].SenderName, Is.EqualTo(driver.UserName));
            Assert.That(notificationList[1].SenderName, Is.EqualTo(admin.UserName));
            Assert.That(notificationList[2].SenderName, Is.EqualTo(admin.UserName));

            Assert.That(notificationList[0].isMarkable, Is.False);
            Assert.That(notificationList[1].isMarkable, Is.True);
            Assert.That(notificationList[2].isMarkable, Is.True);
        }

        [Test]
        public async Task GetAllNotificationsAsync_ShouldReturnEmptyList_WhenNoNotificationsForDriver()
        {
            var driverId = Guid.NewGuid();

            var notifications = await _service.GetAllNotificationsAsync(driverId);

            Assert.That(notifications, Is.Not.Null);
            Assert.That(notifications, Is.Empty);
        }

        [Test]
        public async Task ReadAsync_ShouldMarkNotificationAsRead()
        {
            var notificationId = Guid.NewGuid();
            await _context.Notifications.AddAsync(new Notification { Id = notificationId, Title = "Unread", IsRead = false, CreatedAt = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            await _service.ReadAsync(notificationId);

            var notification = await _context.Notifications.SingleAsync(n => n.Id == notificationId);
            Assert.That(notification.IsRead, Is.True);
        }

        [Test]
        public async Task ReadAsync_ShouldDoNothingIfAlreadyRead()
        {
            var notificationId = Guid.NewGuid();
            await _context.Notifications.AddAsync(new Notification { Id = notificationId, Title = "Read", IsRead = true, CreatedAt = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            await _service.ReadAsync(notificationId);

            var notification = await _context.Notifications.SingleAsync(n => n.Id == notificationId);
            Assert.That(notification.IsRead, Is.True);
        }

        [Test]
        public async Task SoftDelete_ShouldMarkNotificationAsDeleted()
        {
            var notificationId = Guid.NewGuid();
            await _context.Notifications.AddAsync(new Notification { Id = notificationId, Title = "To Delete", IsDeleted = false, CreatedAt = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            await _service.SoftDelete(notificationId);

            var notification = await _context.Notifications.IgnoreQueryFilters().SingleAsync(n => n.Id == notificationId);
            Assert.That(notification.IsDeleted, Is.True);
        }

        [Test]
        public async Task SoftDelete_ShouldDoNothingIfAlreadyDeleted()
        {
            var notificationId = Guid.NewGuid();
            await _context.Notifications.AddAsync(new Notification { Id = notificationId, Title = "Already Deleted", IsDeleted = true, CreatedAt = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _service.SoftDelete(notificationId));
            Assert.That(ex.Message, Is.EqualTo("Notification not found."));

            var notification = await _context.Notifications.IgnoreQueryFilters().SingleAsync(n => n.Id == notificationId);
            Assert.That(notification.IsDeleted, Is.True);
        }

        [Test]
        public async Task UnreadAsync_ShouldMarkNotificationAsUnread()
        {
            var notificationId = Guid.NewGuid();
            await _context.Notifications.AddAsync(new Notification { Id = notificationId, Title = "Read", IsRead = true, CreatedAt = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            await _service.UnreadAsync(notificationId);

            var notification = await _context.Notifications.SingleAsync(n => n.Id == notificationId);
            Assert.That(notification.IsRead, Is.False);
        }

        [Test]
        public async Task UnreadAsync_ShouldDoNothingIfAlreadyUnread()
        {
            var notificationId = Guid.NewGuid();
            await _context.Notifications.AddAsync(new Notification { Id = notificationId, Title = "Unread", IsRead = false, CreatedAt = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            await _service.UnreadAsync(notificationId);

            var notification = await _context.Notifications.SingleAsync(n => n.Id == notificationId);
            Assert.That(notification.IsRead, Is.False);
        }

        [Test]
        public async Task UpdateNotificationAsync_ShouldUpdateNotificationSuccessfully()
        {
            var sender = await AddUser("SenderUser");
            var oldReceiver = await AddUser("OldReceiver");
            var newReceiver = await AddUser("NewReceiver");
            var notificationId = Guid.NewGuid();
            await _context.Notifications.AddAsync(new Notification
            {
                Id = notificationId,
                Title = "Old Title",
                Message = "Old Message",
                ReceiverId = oldReceiver.Id,
                SenderId = sender.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                IsRead = true,
                IsDeleted = false
            });
            await _context.SaveChangesAsync();

            var updateViewModel = new CreateNotificationViewModel
            {
                Title = "New Title",
                Message = "New Message",
                ReceiverId = newReceiver.Id,
                OrderId = Guid.NewGuid()
            };

            var result = await _service.UpdateNotificationAsync(updateViewModel, notificationId, sender.Id);

            Assert.That(result, Is.True);
            var updatedNotification = await _context.Notifications.SingleAsync(n => n.Id == notificationId);
            Assert.That(updatedNotification.Title, Is.EqualTo("New Title"));
            Assert.That(updatedNotification.Message, Is.EqualTo("New Message"));
            Assert.That(updatedNotification.ReceiverId, Is.EqualTo(newReceiver.Id));
            Assert.That(updatedNotification.IsRead, Is.False);
        }

        [Test]
        public async Task UpdateNotificationAsync_ShouldReturnFalse_WhenNotificationNotFound()
        {
            var senderId = Guid.NewGuid();
            var nonExistentId = Guid.NewGuid();
            var updateViewModel = new CreateNotificationViewModel { Title = "Title", Message = "Msg", ReceiverId = Guid.NewGuid() };

            var result = await _service.UpdateNotificationAsync(updateViewModel, nonExistentId, senderId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateNotificationAsync_ShouldReturnFalse_WhenUserIsNotSender()
        {
            var sender = await AddUser("OriginalSender");
            var unauthorizedUser = await AddUser("UnauthorizedUser");
            var receiver = await AddUser("Receiver");
            var notificationId = Guid.NewGuid();

            await _context.Notifications.AddAsync(new Notification
            {
                Id = notificationId,
                Title = "Original",
                Message = "Original",
                ReceiverId = receiver.Id,
                SenderId = sender.Id,
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            var updateViewModel = new CreateNotificationViewModel { Title = "New", Message = "New", ReceiverId = receiver.Id };

            var result = await _service.UpdateNotificationAsync(updateViewModel, notificationId, unauthorizedUser.Id);

            Assert.That(result, Is.False);
            var originalNotification = await _context.Notifications.SingleAsync(n => n.Id == notificationId);
            Assert.That(originalNotification.Title, Is.EqualTo("Original"));
        }

        [Test]
        public void SoftDelete_ShouldThrowInvalidOperationException_WhenNotificationNotFound()
        {
            var nonExistentId = Guid.NewGuid();

            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _service.SoftDelete(nonExistentId));
            Assert.That(ex.Message, Is.EqualTo("Notification not found."));
        }

        [Test]
        public void ReadAsync_ShouldThrowInvalidOperationException_WhenNotificationNotFound()
        {
            var nonExistentId = Guid.NewGuid();

            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _service.ReadAsync(nonExistentId));
            Assert.That(ex.Message, Is.EqualTo("Notification not found."));
        }

        [Test]
        public void UnreadAsync_ShouldThrowInvalidOperationException_WhenNotificationNotFound()
        {
            var nonExistentId = Guid.NewGuid();

            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _service.UnreadAsync(nonExistentId));
            Assert.That(ex.Message, Is.EqualTo("Notification not found."));
        }
    }
}
