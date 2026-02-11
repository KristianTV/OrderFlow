using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Dashboard;

namespace OrderFlow.Areas.Admin.Controllers
{
    public class DashboardController : BaseAdminController
    {

        private readonly ILogger<DashboardController> _logger;
        private readonly IOrderService _orderService;
        private readonly ICourseOrderService _truckOrderService;
        private readonly ITruckService _truckService;
        private readonly INotificationService _notificationService;


        public DashboardController(ILogger<DashboardController> logger,
                                    IOrderService orderService,
                                    ICourseOrderService truckOrderService,
                                    ITruckService truckService,
                                    INotificationService notificationService)
        {
            _logger = logger;
            _orderService = orderService;
            _truckOrderService = truckOrderService;
            _truckService = truckService;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {

            int totalActiveOrders = await _orderService.GetAll()
                                                       .AsNoTracking()
                                                       .Where(o => !new[] { OrderStatus.Cancelled, OrderStatus.Failed, OrderStatus.Completed }.Contains(o.Status))
                                                       .CountAsync();
            int totalCompletedOrdersawait = await _orderService.GetAll()
                                                               .AsNoTracking()
                                                               .Where(o => o.Status.Equals(OrderStatus.Completed))
                                                               .CountAsync();
            int totalCancelledOrders = await _orderService.GetAll()
                                                            .AsNoTracking()
                                                            .Where(o => o.Status.Equals(OrderStatus.Cancelled))
                                                            .CountAsync();

            int totalActiveTrucks = await _truckService.GetAll()
                                                       .AsNoTracking()
                                                       .Where(t => t.Status.Equals(TruckStatus.Available))
                                                       .CountAsync();
            int totalInactiveTrucks = await _truckService.GetAll()
                                                           .AsNoTracking()
                                                           .Where(t => t.Status.Equals(TruckStatus.Unavailable))
                                                           .CountAsync();

            int totalNotifications = await _notificationService.GetAll()
                                                            .AsNoTracking()
                                                            .CountAsync();

            int totalActiveNotifications = await _notificationService.GetAll()
                                                            .AsNoTracking()
                                                            .Where(n => !n.IsRead)
                                                            .CountAsync();
            int totalActiveTruckOrders = await _truckOrderService.GetAll()
                                                                  .AsNoTracking()
                                                                  .Include(t => t.TruckCourse)
                                                                  .Where(t => t.TruckCourse.Status.Equals(CourseStatus.Assigned))
                                                                  .CountAsync();
            int totalCompletedTruckOrders = await _truckOrderService.GetAll()
                                                                  .AsNoTracking()
                                                                  .Where(t => t.TruckCourse.Status.Equals(CourseStatus.Delivered))
                                                                  .CountAsync();

            AdminIndexDashboardViewModel? dashboardViewModel = new AdminIndexDashboardViewModel
            {
                TotalActiveOrders = totalActiveOrders,
                TotalCompletedOrders = totalCompletedOrdersawait,
                TotalCancelledOrders = totalCancelledOrders,
                TotalActiveTrucks = totalActiveTrucks,
                TotalInactiveTrucks = totalInactiveTrucks,
                TotalNotifications = totalNotifications,
                TotalActiveNotifications = totalActiveNotifications,
                TotalActiveTruckOrders = totalActiveTruckOrders,
                TotalCompletedTruckOrders = totalCompletedTruckOrders
            };

            return View(dashboardViewModel);
        }
    }
}
