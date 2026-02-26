using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Dashboard;

namespace OrderFlow.Areas.Driver.Controllers
{
    public class DashboardController : BaseDriverController
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly ITruckCourseService _truckCourseService;
        private readonly INotificationService _notificationService;


        public DashboardController(ILogger<DashboardController> logger,
                                    ITruckCourseService truckCourseService,
                                    INotificationService notificationService)
        {
            _logger = logger;
            _truckCourseService = truckCourseService;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            if (!Guid.TryParse(this.GetUserId(), out Guid userId))
            {
                _logger.LogError("User ID is not valid.");
                return RedirectToAction("Error", "Home");
            }

            int totalNotifications = await _notificationService.GetAll()
                                                            .AsNoTracking()
                                                            .Where(n => n.ReceiverID.Equals(userId))
                                                            .CountAsync();

            int totalActiveNotifications = await _notificationService.GetAll()
                                                            .AsNoTracking()
                                                            .Where(n => !n.IsRead && n.ReceiverID.Equals(userId))
                                                            .CountAsync();

            int totalActiveTruckOrders = await _truckCourseService.GetAll()
                                                                  .AsNoTracking()
                                                                  .Include(tc => tc.Truck)
                                                                  .Where(tc => tc.Truck != null &&
                                                                               tc.Truck.DriverID.Equals(userId) &&
                                                                               tc.Status.Equals(CourseStatus.Assigned))
                                                                  .CountAsync();

            int totalCompletedTruckOrders = await _truckCourseService.GetAll()
                                                                     .AsNoTracking()
                                                                     .Include(tc => tc.Truck)
                                                                     .Where(tc => tc.Truck != null &&
                                                                                  tc.Truck.DriverID.Equals(userId) &&
                                                                                  tc.Status.Equals(CourseStatus.Delivered))
                                                                     .CountAsync();

            DriverIndexDashboardViewModel? dashboardViewModel = new DriverIndexDashboardViewModel
            {
                TotalNotifications = totalNotifications,
                TotalActiveNotifications = totalActiveNotifications,
                TotalActiveTruckOrders = totalActiveTruckOrders,
                TotalCompletedTruckOrders = totalCompletedTruckOrders
            };

            return View(dashboardViewModel);
        }
    }
}
