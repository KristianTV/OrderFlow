using OrderFlow.Data.Models;
using OrderFlow.Data.Repository.Contracts;
using OrderFlow.Services.Core.Commands;
using OrderFlow.ViewModels.Notification;

namespace OrderFlow.Services.Core.Contracts
{
    public interface INotificationService : IRepository
    {
        IQueryable<Notification> GetAll();
        Task CreateNotificationAsync(CreateNotificationViewModel createNotification, Guid senderId);
        Task CreateNotificationAsync(AdminCreateNotificationViewModel createNotification, Guid senderId);
        Task<IEnumerable<IndexNotificationViewModel>?> GetAllNotificationsForUserAsync(Guid userId);
        Task<IEnumerable<DriverIndexNotificationViewModel>?> GetAllNotificationsForDriverAsync(Guid userId);
        Task ReadAsync(Guid id);
        Task UnreadAsync(Guid id);
        Task SoftDelete(Guid id);
        Task<bool> UpdateNotificationAsync(CreateNotificationViewModel createNotification, Guid notification, Guid userId);
        Task<bool> UpdateNotificationAsync(AdminCreateNotificationViewModel createNotification, Guid notification, Guid userId);
        Task<IEnumerable<DriverIndexNotificationViewModel>?> GetAllNotificationsAsync(Guid userId);
        Task<bool> SendSystemNotificationAsync(NotificationCommand notification, bool save = true);
    }
}
