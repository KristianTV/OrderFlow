using OrderFlow.Data.Repository.Contracts;
using OrderFlow.ViewModels.Notification;

namespace OrderFlow.Services.Core.Contracts
{
    public interface INotificationService: IRepository
    {
        Task CreateNotificationAsync(CreateNotificationViewModel createPayment, Guid senderId);
        Task<IEnumerable<IndexNotificationViewModel>?> GetAllNotificationsForUserAsync(Guid userId);
        Task ReadAsync(Guid id);
        Task UnreadAsync(Guid id);
        Task SoftDelete(int i);
        Task<bool> UpdateNotificationAsync(CreateNotificationViewModel createNotification, Guid notification, Guid userId);
    }
}
