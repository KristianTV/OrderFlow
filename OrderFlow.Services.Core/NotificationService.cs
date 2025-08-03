using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Data.Repository;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Notification;

namespace OrderFlow.Services.Core
{
    public class NotificationService : BaseRepository, INotificationService
    {

        public NotificationService(OrderFlowDbContext _context) : base(_context)
        {
        }

        public IQueryable<Notification> GetAll()
        {
            return this.GetAll().AsQueryable();
        }

        public async Task CreateNotificationAsync(CreateNotificationViewModel createNotification, Guid senderId)
        {
            if (createNotification == null)
            {
                throw new ArgumentNullException(nameof(createNotification), "CreateNotificationViewModel cannot be null.");
            }

            await this.AddAsync(new Notification
            {
                Title = createNotification.Title,
                Message = createNotification.Message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                IsDeleted = false,
                ReceiverId = createNotification.ReceiverId,
                SenderId = senderId,
                OrderId = createNotification.OrderId
            });

            await this.SaveChangesAsync();
        }

        public async Task<IEnumerable<IndexNotificationViewModel>?> GetAllNotificationsAsync(Guid userId)
        {
            var notification = await this.GetAll()
                                          .OrderBy(n => n.IsRead)
                                          .ThenByDescending(n => n.CreatedAt)
                                          .Select(notification => new IndexNotificationViewModel
                                          {
                                              NotificationID = notification.Id,
                                              Title = notification.Title,
                                              CreatedAt = notification.CreatedAt,
                                              IsRead = notification.IsRead,
                                              OrderId = notification.OrderId,
                                              SenderName = notification.Sender!.UserName,
                                              isMarkable = notification.ReceiverId.Equals(userId)

                                          })
                                          .ToListAsync();

            return notification;
        }

        public async Task<IEnumerable<IndexNotificationViewModel>?> GetAllNotificationsForUserAsync(Guid userId)
        {
            var notification = await this.GetAll()
                                         .Include(n => n.Sender)
                                         .Where(n => n.ReceiverId.Equals(userId))
                                         .OrderBy(n => n.IsRead)
                                         .ThenByDescending(n => n.CreatedAt)
                                         .Select(notification => new IndexNotificationViewModel
                                         {
                                             NotificationID = notification.Id,
                                             Title = notification.Title,
                                             CreatedAt = notification.CreatedAt,
                                             IsRead = notification.IsRead,
                                             OrderId = notification.OrderId,
                                             SenderName = notification.Sender!.UserName
                                         })
                                         .ToListAsync();

            return notification;
        }

        public async Task ReadAsync(Guid i)
        {
            Notification? notification = await this.DbSet<Notification>().Where(x => x.Id.Equals(i)).SingleOrDefaultAsync();

            if (notification != null)
            {
                if (notification.IsRead)
                {
                    return;
                }

                notification.IsRead = true;
                await this.SaveChangesAsync();
            }
        }

        public async Task SoftDelete(Guid i)
        {
            Notification? notification = await this.DbSet<Notification>().Where(x => x.Id.Equals(i)).SingleOrDefaultAsync();

            if (notification != null)
            {
                if (notification.IsDeleted)
                {
                    return;
                }

                notification.IsDeleted = true;
                await this.SaveChangesAsync();
            }
        }

        public async Task UnreadAsync(Guid id)
        {
            Notification? notification = await this.DbSet<Notification>().Where(x => x.Id.Equals(id)).SingleOrDefaultAsync();

            if (notification != null)
            {
                if (!notification.IsRead)
                {
                    return;
                }

                notification.IsRead = false;
                await this.SaveChangesAsync();
            }
        }

        public async Task<bool> UpdateNotificationAsync(CreateNotificationViewModel createNotification, Guid notification, Guid userId)
        {
            if (createNotification == null)
            {
                throw new ArgumentNullException(nameof(createNotification), "CreateNotificationViewModel cannot be null.");
            }

            Notification? existingNotification = await this.DbSet<Notification>()
                                                           .Where(n => n.Id.Equals(notification) && n.SenderId.Equals(userId))
                                                           .SingleOrDefaultAsync();

            if (existingNotification == null)
            {
                return false;
            }

            existingNotification.Title = createNotification.Title;
            existingNotification.Message = createNotification.Message;
            existingNotification.ReceiverId = createNotification.ReceiverId;
            existingNotification.OrderId = createNotification.OrderId;
            existingNotification.CreatedAt = DateTime.UtcNow;
            existingNotification.IsRead = false;

            await this.SaveChangesAsync();

            return true;
        }
    }
}
