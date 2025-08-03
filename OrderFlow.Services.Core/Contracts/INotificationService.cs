using OrderFlow.Data.Models;
using OrderFlow.Data.Repository.Contracts;
using OrderFlow.ViewModels.Notification;

namespace OrderFlow.Services.Core.Contracts
{
    public interface INotificationService : IRepository
    {
        IQueryable<Notification> GetAll();
        Task CreateNotificationAsync(CreateNotificationViewModel createPayment, Guid senderId);
        Task<IEnumerable<IndexNotificationViewModel>?> GetAllNotificationsForUserAsync(Guid userId);
        Task ReadAsync(Guid id);
        Task UnreadAsync(Guid id);
        Task SoftDelete(Guid id);
        Task<bool> UpdateNotificationAsync(CreateNotificationViewModel createNotification, Guid notification, Guid userId);
        Task<IEnumerable<IndexNotificationViewModel>?> GetAllNotificationsAsync(Guid userId);
    }
}
