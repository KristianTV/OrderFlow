using OrderFlow.Data.Models;
using OrderFlow.Services.Core.Commands;

namespace OrderFlow.Services.Core.Extensions
{
    public static class OrderNotificationExtensions
    {
        public static NotificationCommand ToNotification(
           this Order order,
           string title,
           string message)
        {
            return new NotificationCommand
            {
                Title = title,
                Message = message,
                OrderID = order.OrderID,
                ReceiverID = order.UserID,
                CanRespond = false,
            };
        }
    }
}
