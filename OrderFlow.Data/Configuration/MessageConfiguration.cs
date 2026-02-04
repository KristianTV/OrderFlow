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


        }
    }
}
