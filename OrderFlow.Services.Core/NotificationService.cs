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

        public async Task CreateNotificationAsync(CreateNotificationViewModel createPayment,Guid senderId)
        {
            if(createPayment == null)
            {
                throw new ArgumentNullException(nameof(createPayment), "CreateNotificationViewModel cannot be null.");
            }

            await this.AddAsync(new Notification
            {
                Title = createPayment.Title,
                Message = createPayment.Message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                IsDeleted = false,
                ReceiverId = createPayment.ReceiverId,
                SenderId = senderId,
                OrderId = createPayment.OrderId
            });

            await this.SaveChangesAsync();
        }

        public async Task<IEnumerable<IndexNotificationViewModel>?> GetAllNotificationsForUserAsync(Guid userId)
        {
            var notification = await this.All<Notification>()
                                         .Include(n => n.Sender)
                                         .Where(n => n.ReceiverId.Equals(userId))
                                         .OrderBy(n => n.IsRead)
                                         .ThenByDescending(n => n.CreatedAt)
                                         .Select(notification => new IndexNotificationViewModel
                                          {
                                             Title = notification.Title,
                                             Message = notification.Message,
                                             CreatedAt = notification.CreatedAt,
                                             IsRead = notification.IsRead,
                                             OrderId = notification.OrderId.ToString(),
                                             SenderName = notification.Sender!.UserName
                                          })
                                         .ToListAsync();

            return notification;
        }

        public async Task Read(int i)
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

        public async Task SoftDelete(int i)
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
    }
}
