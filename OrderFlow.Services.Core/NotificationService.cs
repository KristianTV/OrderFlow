using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Data.Repository;
using OrderFlow.Services.Core.Commands;
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
            return this.All<Notification>().AsQueryable();
        }

        public async Task CreateNotificationAsync(CreateNotificationViewModel createNotification, Guid senderId)
        {
            if (createNotification == null)
            {
                throw new ArgumentNullException("Create Notification View Model cannot be null.");
            }

            if (createNotification.ReceiverId == Guid.Empty)
            {
                throw new ArgumentException("Receiver ID cannot be empty.");
            }

            await this.AddAsync(new Notification
            {
                Title = createNotification.Title,
                Message = createNotification.Message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                IsDeleted = false,
                ReceiverID = createNotification.ReceiverId,
                SenderID = senderId,
                OrderID = createNotification.OrderId
            });

            await this.SaveChangesAsync();
        }

        public async Task<IEnumerable<DriverIndexNotificationViewModel>?> GetAllNotificationsAsync(Guid userId)
        {
            var notification = await this.GetAll()
                                          .OrderBy(n => n.IsRead)
                                          .ThenByDescending(n => n.CreatedAt)
                                          .Select(notification => new DriverIndexNotificationViewModel
                                          {
                                              NotificationID = notification.NotificationID,
                                              Title = notification.Title,
                                              CreatedAt = notification.CreatedAt,
                                              IsRead = notification.IsRead,
                                              OrderId = notification.OrderID,
                                              TruckId = notification.TruckID,
                                              SenderName = notification.Sender!.UserName,
                                              isMarkable = notification.ReceiverID.Equals(userId)

                                          })
                                          .ToListAsync();

            return notification;
        }

        public async Task<IEnumerable<IndexNotificationViewModel>?> GetAllNotificationsForUserAsync(Guid userId)
        {
            var notification = await this.GetAll()
                                         .Include(n => n.Sender)
                                         .Where(n => n.ReceiverID.Equals(userId))
                                         .OrderBy(n => n.IsRead)
                                         .ThenByDescending(n => n.CreatedAt)
                                         .Select(notification => new IndexNotificationViewModel
                                         {
                                             NotificationID = notification.NotificationID,
                                             Title = notification.Title,
                                             CreatedAt = notification.CreatedAt,
                                             IsRead = notification.IsRead,
                                             OrderId = notification.OrderID,
                                             SenderName = notification.Sender!.UserName
                                         })
                                         .ToListAsync();

            return notification;
        }

        public async Task ReadAsync(Guid i)
        {
            Notification? notification = await this.DbSet<Notification>().Where(x => x.NotificationID.Equals(i)).SingleOrDefaultAsync();

            if (notification != null)
            {
                if (notification.IsRead)
                {
                    return;
                }

                notification.IsRead = true;
                await this.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Notification not found.");
            }
        }

        public async Task SoftDelete(Guid i)
        {
            Notification? notification = await this.DbSet<Notification>().Where(x => x.NotificationID.Equals(i)).SingleOrDefaultAsync();

            if (notification != null)
            {
                if (notification.IsDeleted)
                {
                    return;
                }

                notification.IsDeleted = true;
                await this.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Notification not found.");
            }
        }

        public async Task UnreadAsync(Guid id)
        {
            Notification? notification = await this.GetAll()
                                                   .Where(x => x.NotificationID.Equals(id)).SingleOrDefaultAsync();

            if (notification != null)
            {
                if (!notification.IsRead)
                {
                    return;
                }

                notification.IsRead = false;
                await this.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Notification not found.");
            }
        }

        public async Task<bool> UpdateNotificationAsync(CreateNotificationViewModel createNotification, Guid notification, Guid userId)
        {
            if (createNotification == null)
            {
                throw new ArgumentNullException(nameof(createNotification), "CreateNotificationViewModel cannot be null.");
            }

            Notification? existingNotification = await this.GetAll()
                                                           .Where(n => n.NotificationID.Equals(notification) && n.SenderID.Equals(userId))
                                                           .SingleOrDefaultAsync();

            if (existingNotification == null)
            {
                return false;
            }

            existingNotification.Title = createNotification.Title;
            existingNotification.Message = createNotification.Message;
            existingNotification.ReceiverID = createNotification.ReceiverId;
            existingNotification.OrderID = createNotification.OrderId;
            existingNotification.CreatedAt = DateTime.UtcNow;
            existingNotification.IsRead = false;

            await this.SaveChangesAsync();

            return true;
        }

        public async Task CreateNotificationAsync(AdminCreateNotificationViewModel createNotification, Guid senderId)
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
                ReceiverID = createNotification.ReceiverId,
                SenderID = senderId,
                OrderID = createNotification.OrderId,
                TruckID = createNotification.TruckId,
            });

            await this.SaveChangesAsync();
        }

        public async Task<bool> UpdateNotificationAsync(AdminCreateNotificationViewModel createNotification, Guid notification, Guid userId)
        {
            if (createNotification == null)
            {
                throw new ArgumentNullException(nameof(createNotification), "CreateNotificationViewModel cannot be null.");
            }

            Notification? existingNotification = await this.GetAll()
                                                           .Where(n => n.NotificationID.Equals(notification) && n.SenderID.Equals(userId))
                                                           .SingleOrDefaultAsync();

            if (existingNotification == null)
            {
                return false;
            }

            existingNotification.Title = createNotification.Title;
            existingNotification.Message = createNotification.Message;
            existingNotification.ReceiverID = createNotification.ReceiverId;
            existingNotification.OrderID = createNotification.OrderId;
            existingNotification.TruckID = createNotification.TruckId;
            existingNotification.CreatedAt = DateTime.UtcNow;
            existingNotification.IsRead = false;

            await this.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<DriverIndexNotificationViewModel>?> GetAllNotificationsForDriverAsync(Guid userId)
        {
            var notification = await this.GetAll()
                                         .Include(n => n.Sender)
                                         .Where(n => n.ReceiverID.Equals(userId))
                                         .OrderBy(n => n.IsRead)
                                         .ThenByDescending(n => n.CreatedAt)
                                         .Select(notification => new DriverIndexNotificationViewModel
                                         {
                                             NotificationID = notification.NotificationID,
                                             Title = notification.Title,
                                             CreatedAt = notification.CreatedAt,
                                             IsRead = notification.IsRead,
                                             OrderId = notification.OrderID,
                                             TruckId = notification.TruckID,
                                             SenderName = notification.Sender!.UserName
                                         })
                                         .ToListAsync();

            return notification;
        }

        public async Task<bool> SendSystemNotificationAsync(NotificationCommand notification, bool save = true)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification), "Notification command cannot be null.");
            }

            await this.AddAsync(new Notification
            {
                Title = notification.Title,
                Message = notification.Message,
                ReceiverID = notification.ReceiverID,
                OrderID = notification.OrderID,
                CourseID = notification.CourseID,
                TruckID = notification.TruckID,
                CanRespond = notification.CanRespond,
                CreatedAt = DateTime.UtcNow,
            });

            if (save)
            {
                return await this.SaveChangesAsync() > 0;
            }
            return true;
        }
    }
}
