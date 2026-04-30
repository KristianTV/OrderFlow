using OrderFlow.Data.Models;
using OrderFlow.Services.Core.Commands;


namespace OrderFlow.Services.Core.Extensions
{
    public static class TruckCourseNotificationExtensions
    {
        public static NotificationCommand ToNotification(
           this TruckCourse course,
           string title,
           string message)
        {
            if (course.Truck == null)
            {
                throw new ArgumentException("TruckCourse must have a Truck to create a notification.");
            }

            return new NotificationCommand
            {
                Title = title,
                Message = message,
                CourseID = course.TruckCourseID,
                TruckID = course.TruckID,
                ReceiverID = course.Truck!.DriverID,
                CanRespond = false,
            };
        }
    }
}
