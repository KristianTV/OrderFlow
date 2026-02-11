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
        private readonly ICourseOrderService _truckOrderService;
        private readonly INotificationService _notificationService;


        public DashboardController(ILogger<DashboardController> logger,
                                    ICourseOrderService truckOrderService,
                                    INotificationService notificationService)
        {
            _logger = logger;
            _truckOrderService = truckOrderService;
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

            int totalActiveTruckOrders = await _truckOrderService.GetAll()
                                                                 .AsNoTracking()
                                                                 .Include(co => co.TruckCourse)
                                                                 .ThenInclude(tc => tc.Truck)
                                                                 .Where(t => t.TruckCourse.Status.Equals(CourseStatus.Assigned) &&
                                                                             t.TruckCourse.Truck.DriverID.Equals(userId))
                                                                 .CountAsync();

            int totalCompletedTruckOrders = await _truckOrderService.GetAll()
                                                                    .AsNoTracking()
                                                                    .Include(co => co.TruckCourse)
                                                                    .ThenInclude(tc => tc.Truck)
                                                                    .Where(t => t.TruckCourse.Equals(CourseStatus.Delivered) &&
                                                                                t.TruckCourse.Truck.DriverID.Equals(userId))
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
