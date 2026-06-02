using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
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

        public async Task<IEnumerable<DriverIndexNotificationViewModel>?> GetAllNotificationsAsync(Guid userId, NotificationQueryModel queryModel)
        {
            var notifications = this.GetAll()
                                    .Include(n => n.Sender)
                                    .AsQueryable();

            notifications = ApplyOrderQuery(notifications, queryModel);

            return await ProjectToDriverIndexNotificationViewModel(userId, notifications).ToListAsync();
        }

        public async Task<IEnumerable<IndexNotificationViewModel>?> GetAllNotificationsForUserAsync(Guid userId, NotificationQueryModel queryModel)
        {
            var notifications = this.GetAll()
                                    .Include(n => n.Sender)
                                    .Where(n => n.ReceiverID.Equals(userId));

            notifications = ApplyOrderQuery(notifications, queryModel);

            return await ProjectToIndexNotificationViewModel(notifications).ToListAsync();
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

            if (!await VerifyIds(createNotification))
            {
                throw new InvalidOperationException("Invalid IDs provided.");
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
                CourseID = createNotification.CourseId,
                PaymentID = createNotification.PaymentId,
                TruckSpendingID = createNotification.TruckSpendingId,
                CanRespond = createNotification.IsResponseEnabled
            });

            await this.SaveChangesAsync();
        }

        private async Task<bool> VerifyIds(AdminCreateNotificationViewModel createNotification)
        {
            if (!await this.ExistsAsync<ApplicationUser>(createNotification.ReceiverId))
            {
                throw new InvalidOperationException("Receiver not found.");
            }

            if (createNotification.OrderId != null && !await this.ExistsAsync<Order>(createNotification.OrderId))
            {
                throw new InvalidOperationException("Order not found.");
            }

            if (createNotification.TruckId != null && !await this.ExistsAsync<Truck>(createNotification.TruckId))
            {
                throw new InvalidOperationException("Truck not found.");
            }

            if (createNotification.CourseId != null && !await this.ExistsAsync<TruckCourse>(createNotification.CourseId))
            {
                throw new InvalidOperationException("Course not found.");
            }

            if (createNotification.PaymentId != null && !await this.ExistsAsync<Payment>(createNotification.PaymentId))
            {
                throw new InvalidOperationException("Payment not found.");
            }

            if (createNotification.TruckSpendingId != null && !await this.ExistsAsync<TruckSpending>(createNotification.TruckSpendingId))
            {
                throw new InvalidOperationException("Truck Spending not found.");
            }

            HashSet<string> receiverRoles = await GetReceiverRolesAsync(createNotification.ReceiverId);
            bool isStaffReceiver = receiverRoles.Contains(UserRoles.Admin.ToString()) ||
                                   receiverRoles.Contains(UserRoles.Speditor.ToString());
            bool isDriverReceiver = receiverRoles.Contains(UserRoles.Driver.ToString()) && !isStaffReceiver;
            bool isRegularUserReceiver = receiverRoles.Contains(UserRoles.User.ToString()) && !isStaffReceiver;

            if (createNotification.OrderId.HasValue)
            {
                Guid orderOwnerId = await this.All<Order>()
                                              .Where(order => order.OrderID == createNotification.OrderId.Value)
                                              .Select(order => order.UserID)
                                              .SingleAsync();

                if (isRegularUserReceiver && orderOwnerId != createNotification.ReceiverId)
                {
                    throw new InvalidOperationException("Order does not belong to the selected user.");
                }
            }

            if (createNotification.PaymentId.HasValue)
            {
                Guid paymentOrderId = await this.All<Payment>()
                                                .Where(payment => payment.PaymentID == createNotification.PaymentId.Value)
                                                .Select(payment => payment.OrderID)
                                                .SingleAsync();

                if (createNotification.OrderId.HasValue && paymentOrderId != createNotification.OrderId.Value)
                {
                    throw new InvalidOperationException("Payment does not belong to the selected order.");
                }

                if (isRegularUserReceiver)
                {
                    bool paymentBelongsToReceiver = await this.All<Order>()
                                                              .AnyAsync(order => order.OrderID == paymentOrderId &&
                                                                                 order.UserID == createNotification.ReceiverId);

                    if (!paymentBelongsToReceiver)
                    {
                        throw new InvalidOperationException("Payment does not belong to the selected user's order.");
                    }
                }
            }

            if (createNotification.TruckId.HasValue)
            {
                Guid driverId = await this.All<Truck>()
                                          .Where(truck => truck.TruckID == createNotification.TruckId.Value)
                                          .Select(truck => truck.DriverID)
                                          .SingleAsync();

                if (isDriverReceiver && driverId != createNotification.ReceiverId)
                {
                    throw new InvalidOperationException("Truck does not belong to the selected driver.");
                }
            }

            if (createNotification.CourseId.HasValue)
            {
                Guid? courseTruckId = await this.All<TruckCourse>()
                                                .Where(course => course.TruckCourseID == createNotification.CourseId.Value)
                                                .Select(course => course.TruckID)
                                                .SingleAsync();

                if (createNotification.TruckId.HasValue && courseTruckId != createNotification.TruckId.Value)
                {
                    throw new InvalidOperationException("Course does not belong to the selected truck.");
                }

                if (isDriverReceiver)
                {
                    bool courseBelongsToDriver = courseTruckId.HasValue &&
                                                 await this.All<Truck>()
                                                           .AnyAsync(truck => truck.TruckID == courseTruckId.Value &&
                                                                              truck.DriverID == createNotification.ReceiverId);

                    if (!courseBelongsToDriver)
                    {
                        throw new InvalidOperationException("Course does not belong to the selected driver's truck.");
                    }
                }
            }

            if (createNotification.TruckSpendingId.HasValue)
            {
                var spending = await this.All<TruckSpending>()
                                         .Where(truckSpending => truckSpending.TruckSpendingID == createNotification.TruckSpendingId.Value)
                                         .Select(truckSpending => new
                                         {
                                             truckSpending.TruckID,
                                             truckSpending.TruckCourseID
                                         })
                                         .SingleAsync();

                if (createNotification.TruckId.HasValue && spending.TruckID != createNotification.TruckId.Value)
                {
                    throw new InvalidOperationException("Truck spending does not belong to the selected truck.");
                }

                if (createNotification.CourseId.HasValue && spending.TruckCourseID != createNotification.CourseId.Value)
                {
                    throw new InvalidOperationException("Truck spending does not belong to the selected course.");
                }

                if (isDriverReceiver)
                {
                    bool spendingBelongsToDriver = spending.TruckID.HasValue &&
                                                   await this.All<Truck>()
                                                             .AnyAsync(truck => truck.TruckID == spending.TruckID.Value &&
                                                                                truck.DriverID == createNotification.ReceiverId);

                    if (!spendingBelongsToDriver)
                    {
                        throw new InvalidOperationException("Truck spending does not belong to the selected driver's truck.");
                    }
                }
            }

            return true;
        }

        private async Task<HashSet<string>> GetReceiverRolesAsync(Guid receiverId)
        {
            return await this.All<IdentityUserRole<Guid>>()
                             .Where(userRole => userRole.UserId == receiverId)
                             .Join(
                                 this.All<IdentityRole<Guid>>(),
                                 userRole => userRole.RoleId,
                                 role => role.Id,
                                 (userRole, role) => role.Name ?? string.Empty)
                             .ToHashSetAsync(StringComparer.OrdinalIgnoreCase);
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

            if (!await VerifyIds(createNotification))
            {
                throw new InvalidOperationException("Invalid IDs provided.");
            }

            existingNotification.Title = createNotification.Title;
            existingNotification.Message = createNotification.Message;
            existingNotification.ReceiverID = createNotification.ReceiverId;
            existingNotification.OrderID = createNotification.OrderId;
            existingNotification.TruckID = createNotification.TruckId;
            existingNotification.CourseID = createNotification.CourseId;
            existingNotification.PaymentID = createNotification.PaymentId;
            existingNotification.TruckSpendingID = createNotification.TruckSpendingId;
            existingNotification.CreatedAt = DateTime.UtcNow;
            existingNotification.IsRead = false;
            existingNotification.CanRespond = createNotification.IsResponseEnabled;

            await this.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<DriverIndexNotificationViewModel>?> GetAllNotificationsForDriverAsync(Guid userId, NotificationQueryModel queryModel)
        {
            var notifications = this.GetAll()
                                    .Include(n => n.Sender)
                                    .Where(n => n.ReceiverID.Equals(userId));

            notifications = ApplyOrderQuery(notifications, queryModel);

            return await ProjectToDriverIndexNotificationViewModel(userId, notifications).ToListAsync();
        }

        private static IQueryable<Notification> ApplyOrderQuery(IQueryable<Notification> notification, NotificationQueryModel? query)
        {
            if (query == null)
            {
                return notification.OrderByDescending(n => n.CreatedAt);
            }

            if (query!.HideSystemNotifications)
            {
                notification = notification.Where(n => n.SenderID != null);
            }

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                switch (query.SortBy.ToLower())
                {
                    case "unread":
                        notification = notification.Where(n => !n.IsRead);
                        break;
                    case "read":
                        notification = notification.Where(n => n.IsRead);
                        break;
                }
            }

            notification = notification.OrderBy(n => n.IsRead)
                                       .ThenByDescending(n => n.CreatedAt);

            return notification.Skip((Math.Max(query.Page, 1) - 1) * Math.Max(query.PageSize - 1, 1))
                               .Take(Math.Max(query.PageSize, 1));

        }
        private static IQueryable<IndexNotificationViewModel> ProjectToIndexNotificationViewModel(IQueryable<Notification> notification)
        {
            return notification.Select(notification => new IndexNotificationViewModel
            {
                NotificationID = notification.NotificationID,
                Title = notification.Title,
                CreatedAt = notification.CreatedAt,
                IsRead = notification.IsRead,
                OrderId = notification.OrderID,
                SenderName = notification.Sender != null ? notification.Sender.UserName : null,
                isMarkable = true
            });

        }
        private static IQueryable<DriverIndexNotificationViewModel> ProjectToDriverIndexNotificationViewModel(Guid userId, IQueryable<Notification> notifications)
        {
            return notifications.Select(notification => new DriverIndexNotificationViewModel
            {
                NotificationID = notification.NotificationID,
                Title = notification.Title,
                CreatedAt = notification.CreatedAt,
                IsRead = notification.IsRead,
                OrderId = notification.OrderID,
                TruckId = notification.TruckID,
                SenderName = notification.Sender != null ? notification.Sender.UserName : null,
                isMarkable = notification.ReceiverID.Equals(userId)
            });

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

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await this.GetAll()
                             .AsNoTracking()
                             .Where(n => n.ReceiverID.Equals(userId) && !n.IsRead && !n.IsDeleted)
                             .CountAsync();
        }
    }
}
