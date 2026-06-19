using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Data.Models;
using OrderFlow.GCommon;

namespace OrderFlow.Data.Configuration
{

    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(m => m.MessageID);

            builder.Property(m => m.Content)
                   .IsRequired()
                   .HasMaxLength(ValidationConstants.Message.ContentMaxLength);

            builder.Property(m => m.SentAt)
                   .IsRequired();

            builder.Property(m => m.IsRead)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(m => m.IsDeleted)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Notification)
                .WithMany()
                .HasForeignKey(m => m.NotificationID)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasData(SeedMessages());
        }

        private List<Message> SeedMessages()
        {
            return new List<Message>
            {
                CreateMessage(0, 6, SeedDataIds.AdminUserId, SeedDataIds.RegularUserId,
                    "Please send the contact name and phone number.", 0),
                CreateMessage(1, 6, SeedDataIds.RegularUserId, SeedDataIds.AdminUserId,
                    "The contact person is Ivan Petrov, phone 0888000001.", 1),
                CreateMessage(2, 8, SeedDataIds.AdminUserId, SeedDataIds.DriverUserId,
                    "Are the insurance and technical inspection documents in the truck?", 0),
                CreateMessage(3, 8, SeedDataIds.DriverUserId, SeedDataIds.AdminUserId,
                    "Yes, both documents are available.", 1),
                CreateMessage(4, 9, SeedDataIds.SpeditorUserId, SeedDataIds.DriverTwoUserId,
                    "Use the eastern bypass because of roadworks.", 0),
                CreateMessage(5, 9, SeedDataIds.DriverTwoUserId, SeedDataIds.SpeditorUserId,
                    "Understood, I will use the eastern bypass.", 1)
            };
        }

        private static Message CreateMessage(
            int messageIndex,
            int notificationIndex,
            Guid senderId,
            Guid receiverId,
            string content,
            int minuteOffset)
            => new()
            {
                MessageID = SeedDataIds.MessageIds[messageIndex],
                NotificationID = SeedDataIds.NotificationIds[notificationIndex],
                SenderID = senderId,
                ReceiverID = receiverId,
                Content = content,
                SentAt = new DateTime(2026, 6, 17).AddHours(notificationIndex).AddMinutes(minuteOffset),
                IsRead = minuteOffset == 0,
                IsDeleted = false
            };
    }
}
