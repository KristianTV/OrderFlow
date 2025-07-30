using OrderFlow.Data.Repository.Contracts;
using OrderFlow.ViewModels.Notification;

namespace OrderFlow.Services.Core.Contracts
{
    public interface INotificationService: IRepository
    {
        Task CreateNotificationAsync(CreateNotificationViewModel createPayment, Guid senderId);
        Task<IEnumerable<IndexNotificationViewModel>?> GetAllNotificationsForUserAsync(Guid userId);
        Task Read(int id);
        Task SoftDelete(int i);
    }
}
