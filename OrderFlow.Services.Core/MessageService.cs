using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Data.Repository;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Message;

namespace OrderFlow.Services.Core
{
    public class MessageService : BaseRepository, IMessageService
    {
        public MessageService(OrderFlowDbContext _context) : base(_context)
        {
        }

        public async Task<Message> CreateMessageAsync(CreateNotificationMessageViewModel createNotificationMessageViewModel, bool save = true)
        {
            if (createNotificationMessageViewModel == null)
                throw new ArgumentNullException(nameof(createNotificationMessageViewModel));

            if (createNotificationMessageViewModel.SenderID == null)
                throw new ArgumentException("SenderID cannot be null.");

            var message = new Message
            {
                Content = createNotificationMessageViewModel.Content,
                SenderID = createNotificationMessageViewModel.SenderID ?? Guid.Empty,
                ReceiverID = createNotificationMessageViewModel.ReceiverID ?? null,
                NotificationID = createNotificationMessageViewModel.NotificationID,
                SentAt = DateTime.UtcNow
            };

            await this.AddAsync(message);

            if (save)
            {
                bool saved = await this.SaveChangesAsync() > 0;
                if (!saved)
                    throw new Exception("Failed to save the message to the database.");
            }

            return message;
        }

        public async Task<bool> DeleteMessageAsync(Guid? messageId, Guid? senderId, bool save = true)
        {
            if (messageId == null || messageId.Value.Equals(Guid.Empty))
                return false;

            Message? message = await this.GetMessageByIdAsync(messageId);

            if (message == null || message.IsDeleted || message.SenderID.Equals(senderId))
                return false;

            message.IsDeleted = true;

            if (save)
                return await this.SaveChangesAsync() > 0;


            return true;
        }

        public IQueryable<Message> GetAll()
        {
            return this.All<Message>().AsQueryable();
        }

        public async Task<Message?> GetMessageByIdAsync(Guid? messageId)
        {
            return await this.GetAll().FirstOrDefaultAsync(m => m.MessageID == messageId);
        }

        public async Task<IEnumerable<Message?>> GetMessagesByNotificationIdAsync(Guid? notificationId)
        {
            return await this.GetAll()
                             .Where(m => m.NotificationID == notificationId)
                             .OrderBy(m => m.SentAt)
                             .ToListAsync();
        }

        public async Task<bool> MarkMessageAsReadAsync(Guid? messageId, bool save = true)
        {
            if (messageId == null || messageId.Value.Equals(Guid.Empty))
                return false;

            Message? message = await this.GetMessageByIdAsync(messageId);

            if (message == null || message.IsRead)
                return false;

            message.IsRead = true;

            if (save)
                return await this.SaveChangesAsync() > 0;

            return true;
        }

        public async Task<Message> UpdateMessageAsync(CreateNotificationMessageViewModel createNotificationMessageViewModel, Guid? messageId, Guid? senderId, bool save = true)
        {
            Message? message = this.GetAll().FirstOrDefault(m => m.MessageID == messageId && m.SenderID == senderId);

            if (message == null)
                throw new KeyNotFoundException($"Message with ID {messageId} not found or sender does not match.");

            message.Content = createNotificationMessageViewModel.Content;
            message.IsRead = false;

            if (save)
            {
                bool saved = await this.SaveChangesAsync() > 0;
                if (!saved)
                    throw new Exception("Failed to save the message to the database.");
            }

            return message;
        }
    }
}
