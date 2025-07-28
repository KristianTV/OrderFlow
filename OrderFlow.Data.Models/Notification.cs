using Microsoft.AspNet.Identity.EntityFramework;

namespace OrderFlow.Data.Models
{
    public class Notification
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public bool IsRead { get; set; } = false;

        public Guid ReceiverId { get; set; }
        public IdentityUser Receiver { get; set; } = null!;

        public Guid? SenderId { get; set; }
        public IdentityUser? Sender { get; set; }

        public Guid? OrderId { get; set; }
        public Order? Order { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
