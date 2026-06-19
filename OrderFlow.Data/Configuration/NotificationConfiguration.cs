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

            builder.HasData(SeedNotifications());
        }

        private List<Notification> SeedNotifications()
        {
            return new List<Notification>
            {
                CreateSystemNotification(0, SeedDataIds.RegularUserId, "Order received",
                    "Your order has been received and is waiting to be assigned.", orderId: SeedDataIds.OrderIds[16]),
                CreateSystemNotification(1, SeedDataIds.CompanyUserId, "Order completed",
                    "Your order has been delivered successfully.", orderId: SeedDataIds.OrderIds[14], isRead: true),
                CreateSystemNotification(2, SeedDataIds.DriverUserId, "Course assigned",
                    "A new course has been assigned to your truck.", truckId: SeedDataIds.TruckIds[0], courseId: SeedDataIds.CourseIds[0]),
                CreateSystemNotification(3, SeedDataIds.DriverTwoUserId, "Course completed",
                    "The assigned course has been completed.", truckId: SeedDataIds.TruckIds[3], courseId: SeedDataIds.CourseIds[3], isRead: true),
                CreateSystemNotification(4, SeedDataIds.RegularUserId, "Payment confirmed",
                    "The payment for your completed order has been confirmed.", orderId: SeedDataIds.OrderIds[2], paymentId: SeedDataIds.PaymentIds[8]),
                CreateSystemNotification(5, SeedDataIds.DriverUserId, "Truck spending recorded",
                    "A truck spending entry has been recorded.", truckId: SeedDataIds.TruckIds[0],
                    courseId: SeedDataIds.CourseIds[0], truckSpendingId: SeedDataIds.TruckSpendingIds[0]),

                CreateStaffNotification(6, SeedDataIds.AdminUserId, SeedDataIds.RegularUserId,
                    "Delivery details required", "Please confirm the delivery contact details.", true, SeedDataIds.OrderIds[0]),
                CreateStaffNotification(7, SeedDataIds.SpeditorUserId, SeedDataIds.CompanyUserId,
                    "Pickup time updated", "The pickup time for your order has been updated.", false, SeedDataIds.OrderIds[10]),
                CreateStaffNotification(8, SeedDataIds.AdminUserId, SeedDataIds.DriverUserId,
                    "Vehicle document check", "Please confirm that the vehicle documents are available.", true,
                    truckId: SeedDataIds.TruckIds[0]),
                CreateStaffNotification(9, SeedDataIds.SpeditorUserId, SeedDataIds.DriverTwoUserId,
                    "Route information", "Additional route information is available for your course.", true,
                    truckId: SeedDataIds.TruckIds[3], courseId: SeedDataIds.CourseIds[3]),
                CreateStaffNotification(10, SeedDataIds.AdminUserId, SeedDataIds.CompanyUserId,
                    "Invoice available", "The invoice for your completed order is now available.", false, SeedDataIds.OrderIds[14]),
                CreateStaffNotification(11, SeedDataIds.SpeditorUserId, SeedDataIds.RegularUserId,
                    "Delivery window", "Please confirm whether the proposed delivery window is suitable.", true, SeedDataIds.OrderIds[1])
            };
        }

        private static Notification CreateSystemNotification(
            int index,
            Guid receiverId,
            string title,
            string message,
            Guid? orderId = null,
            Guid? truckId = null,
            Guid? courseId = null,
            Guid? paymentId = null,
            Guid? truckSpendingId = null,
            bool isRead = false)
            => new()
            {
                NotificationID = SeedDataIds.NotificationIds[index],
                ReceiverID = receiverId,
                SenderID = null,
                Title = title,
                Message = message,
                CreatedAt = new DateTime(2026, 6, 15).AddHours(index),
                IsRead = isRead,
                IsDeleted = false,
                CanRespond = false,
                OrderID = orderId,
                TruckID = truckId,
                CourseID = courseId,
                PaymentID = paymentId,
                TruckSpendingID = truckSpendingId
            };

        private static Notification CreateStaffNotification(
            int index,
            Guid senderId,
            Guid receiverId,
            string title,
            string message,
            bool canRespond,
            Guid? orderId = null,
            Guid? truckId = null,
            Guid? courseId = null)
            => new()
            {
                NotificationID = SeedDataIds.NotificationIds[index],
                SenderID = senderId,
                ReceiverID = receiverId,
                Title = title,
                Message = message,
                CreatedAt = new DateTime(2026, 6, 16).AddHours(index),
                IsRead = index % 2 == 0,
                IsDeleted = false,
                CanRespond = canRespond,
                OrderID = orderId,
                TruckID = truckId,
                CourseID = courseId
            };
    }
}
