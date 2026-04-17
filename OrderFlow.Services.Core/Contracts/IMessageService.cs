using OrderFlow.Data.Models;
using OrderFlow.Data.Repository.Contracts;
using OrderFlow.ViewModels.Message;

namespace OrderFlow.Services.Core.Contracts
{
    public interface IMessageService : IRepository
    {
        IQueryable<Message> GetAll();
        Task<Message?> GetMessageByIdAsync(Guid? messageId);
        Task<IEnumerable<Message?>> GetMessagesByNotificationIdAsync(Guid? notificationId);
        Task<Message> CreateMessageAsync(CreateNotificationMessageViewModel createNotificationMessageViewModel, bool save = true);
        Task<Message> UpdateMessageAsync(CreateNotificationMessageViewModel createNotificationMessageViewModel, Guid? messageId, Guid? senderId, bool save = true);
        Task<bool> DeleteMessageAsync(Guid? messageId, bool save = true);
        Task<bool> MarkMessageAsReadAsync(Guid? messageId, bool save = true);
    }
}
