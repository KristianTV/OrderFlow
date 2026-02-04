using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Data.Models;
using OrderFlow.GCommon;

namespace OrderFlow.Data.Configuration
{
    internal class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.NotificationID);

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

            builder.Property(n => n.IsDeleted)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(n => n.CanRespond)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.HasOne(n => n.Receiver)
                   .WithMany()
                   .HasForeignKey(n => n.ReceiverID)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(n => n.Sender)
                   .WithMany()
                   .HasForeignKey(n => n.SenderID)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(n => n.Order)
                   .WithMany(o => o.Notifications)
                   .HasForeignKey(n => n.OrderID)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(n => n.Truck)
                   .WithMany(o => o.Notifications)
                   .HasForeignKey(n => n.TruckID)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(n => n.Course)
                   .WithMany(c => c.Notifications)
                   .HasForeignKey(n => n.CourseID)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(n => n.Payment)
                   .WithMany(p => p.Notifications)
                   .HasForeignKey(n => n.PaymentID)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(n => n.TruckSpending)
                   .WithMany(ts => ts.Notifications)
                   .HasForeignKey(n => n.TruckSpendingID)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(n => n.Messages)
                   .WithOne(m => m.Notification)
                   .HasForeignKey(m => m.NotificationID)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasQueryFilter(n => !n.IsDeleted);
        }
    }
}
