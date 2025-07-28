using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Data.Models;
using OrderFlow.GCommon;
using System.ComponentModel.DataAnnotations;

namespace OrderFlow.Data.Configuration
{
    internal class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(ValidationConstants.Notification.TitleMaxLenght);

            builder.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(ValidationConstants.Notification.MessageMaxLength);

            builder.Property(n => n.CreatedAt)
                .IsRequired();

            builder.Property(n => n.IsRead)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasOne(n => n.Receiver)
                    .WithMany()
                    .HasForeignKey(n => n.ReceiverId);

            builder.HasOne(n => n.Sender)
                .WithMany()
                .HasForeignKey(n => n.SenderId);

            builder.HasOne(n => n.Order)
                .WithMany(o => o.Notifications)
                .HasForeignKey(n => n.OrderId);

            builder.Property(n => n.IsDeleted)
                   .IsRequired()
                   .HasDefaultValue(false);
        }
    }
}
