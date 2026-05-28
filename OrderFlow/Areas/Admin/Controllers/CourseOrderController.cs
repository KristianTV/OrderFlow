using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.CourseOrder;

namespace OrderFlow.Areas.Admin.Controllers
{
    public class CourseOrderController : BaseAdminController
    {
        private readonly ILogger<CourseOrderController> _logger;
        private readonly ICourseOrderService _courseOrderService;
        private readonly IOrderService _orderService;
        private readonly ITruckCourseService _truckCourseService;

        public CourseOrderController(ICourseOrderService courseOrderService,
                                     ITruckCourseService truckCourseService,
                                     IOrderService orderService,
                                     ILogger<CourseOrderController> logger)
        {
            _courseOrderService = courseOrderService;
            _truckCourseService = truckCourseService;
            _orderService = orderService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrdersByCourseId(string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid courseID))
            {
                return BadRequest("Invalid course ID.");
            }

            var orders = await _courseOrderService.GetByCourseIdAsync(courseID);
            if (orders == null || !orders.Any())
            {
                return NotFound("No orders found for this course.");
            }
            return Ok(orders);
        }

        [HttpGet]
        public async Task<IActionResult> AssignOrders(string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid courseID))
            {
                return BadRequest("Invalid course ID.");
            }

            if (!await _truckCourseService.ExistsAsync<TruckCourse>(courseID))
            {
                _logger.LogError("Course not found.");
                ModelState.AddModelError(nameof(courseID), "Course not found.");
                return NotFound();
            }


            var course = await _truckCourseService.GetAll()
                                                  .Include(c => c.Truck)
                                                  .Include(c => c.CourseOrders)
                                                  .Where(c => c.TruckCourseID.Equals(courseID))
                                                  .SingleOrDefaultAsync();
            if (course == null)
            {
                return NotFound("No course found.");
            }

            var assignedOrderIds = course.CourseOrders
                                         .Select(co => co.OrderID)
                                         .ToList();

            var maxCapacity = course.Truck?.Capacity ?? 0;

            AssignOrdersToCourseViewModel assignOrders = new AssignOrdersToCourseViewModel
            {
                CourseId = courseID,
                Capacity = maxCapacity,
                Orders = await _orderService.GetAll()
                                            .Where(o => (o.Status.Equals(OrderStatus.Pending) ||
                                                          o.Status.Equals(OrderStatus.OnHold) ||
                                                          o.Status.Equals(OrderStatus.InProgress) ||
                                                          o.Status.Equals(OrderStatus.Delayed)) &&
                                                          o.LoadCapacity <= maxCapacity)
                                            .Select(o => new OrderViewModel
                                            {
                                                OrderID = o.OrderID,
                                                DeliveryAddress = o.DeliveryAddress,
                                                PickupAddress = o.PickupAddress,
                                                LoadCapacity = o.LoadCapacity,
                                                OrderStatus = o.Status.ToString(),
                                                IsSelected = assignedOrderIds.Contains(o.OrderID)
                                            }).ToListAsync()
            };

            return View(assignOrders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignOrders(string? id, AssignOrdersToCourseViewModel assignOrders)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogError("Order ID must be provided.");
                ModelState.AddModelError(nameof(id), "Order ID must be provided.");
                return NotFound();
            }

            if (!Guid.TryParse(id, out Guid courseID))
            {
                _logger.LogError("Invalid Order ID format.");
                ModelState.AddModelError(nameof(courseID), "Invalid Order ID format.");
                return BadRequest();
            }

            if (!await _truckCourseService.ExistsAsync<TruckCourse>(courseID))
            {
                _logger.LogError("Course not found.");
                ModelState.AddModelError(nameof(courseID), "Course not found.");
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var course = await _truckCourseService.GetAll()
                                                  .Include(c => c.Truck)
                                                  .Include(c => c.CourseOrders)
                                                  .Where(c => c.TruckCourseID.Equals(courseID))
                                                  .SingleOrDefaultAsync();
                if (course == null)
                {
                    return NotFound("No course found.");
                }

                var assignedOrderIds = course.CourseOrders
                                             .Select(co => co.OrderID)
                                             .ToList();

                var maxCapacity = course.Truck?.Capacity ?? 0;

                assignOrders.Orders = await _orderService.GetAll()
                                                         .Where(o => (o.Status.Equals(OrderStatus.Pending) ||
                                                                     o.Status.Equals(OrderStatus.Delayed) ||
                                                                     o.Status.Equals(OrderStatus.OnHold) ||
                                                                     o.Status.Equals(OrderStatus.InProgress)) &&
                                                                     o.LoadCapacity <= maxCapacity)
                                                         .Select(o => new OrderViewModel
                                                         {
                                                             OrderID = o.OrderID,
                                                             DeliveryAddress = o.DeliveryAddress,
                                                             PickupAddress = o.PickupAddress,
                                                             LoadCapacity = o.LoadCapacity,
                                                             OrderStatus = o.Status.ToString(),
                                                             IsSelected = assignedOrderIds.Contains(o.OrderID)
                                                         }).ToListAsync();
                return View(assignOrders);
            }


            var selectedOrders = assignOrders.Orders
                                             .Where(o => o.IsSelected)
                                             .Select(o => o.OrderID)
                                             .ToList();

            if (!selectedOrders.Any())
            {
                return BadRequest("No orders selected for assignment.");
            }

            foreach (var order in selectedOrders)
            {
                if (await _courseOrderService.AddOrderToCourseAsync(order, courseID))
                {
                    ModelState.AddModelError(nameof(assignOrders.Orders), $"Order {order} assigned successfully.");
                }
                else
                {
                    ModelState.AddModelError(nameof(assignOrders.Orders), $"Failed to assign order {order}.");
                    _logger.LogError($"Failed to assign order {order} to course {courseID}.");
                }

            }

            return RedirectToAction("Detail", "Course", new { id = courseID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveOrder(string? orderId, string? courseId)
        {
            if (string.IsNullOrEmpty(orderId) || !Guid.TryParse(orderId, out Guid orderID) ||
                string.IsNullOrEmpty(courseId) || !Guid.TryParse(courseId, out Guid courseID))
            {
                return BadRequest("Invalid order or course ID.");
            }
            if (await _courseOrderService.RemoveOrderFromCourseAsync(orderID, courseID))
            {
                return RedirectToAction("Detail", "Course", new { id = courseID });
            }
            return NotFound("Order not found in the course.");
        }
    }
}