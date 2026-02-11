using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Course;
using OrderFlow.ViewModels.Order;

namespace OrderFlow.Areas.Admin.Controllers
{
    public class CourseController : BaseAdminController
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly ITruckCourseService _truckCourseService;
        private readonly ICourseOrderService _courseOrderService;
        private readonly ITruckService _truckService;
        private readonly INotificationService _notificationService;


        public CourseController(ILogger<DashboardController> logger,
                                    ITruckCourseService truckCourseService,
                                    ICourseOrderService courseOrderService,
                                    ITruckService truckService,
                                    INotificationService notificationService)
        {
            _logger = logger;
            _truckCourseService = truckCourseService;
            _courseOrderService = courseOrderService;
            _truckService = truckService;
            _notificationService = notificationService;
        }
        private async Task<Dictionary<Guid, string>> GetAvailableTrucksAsync()
        {
            return await _truckService.GetAll()
                                      .Where(t => t.Status.Equals(TruckStatus.Available))
                                      .ToDictionaryAsync(truck => truck.TruckID,
                                                         truck => truck.LicensePlate);
        }

        [HttpGet]
        public async Task<IActionResult> Index(bool? hideCompleted, string? searchId = null, string? statusFilter = null, string? sortOrder = null)
        {
            try
            {
                var courses = _truckCourseService.GetAll().AsNoTracking();

                if (!string.IsNullOrEmpty(searchId))
                {
                    courses = courses.Where(o => o.TruckCourseID.ToString().Contains(searchId));
                }

                if (!string.IsNullOrEmpty(statusFilter))
                {
                    if (!Enum.TryParse(statusFilter, out CourseStatus courseStatus))
                    {
                        _logger.LogWarning("Invalid status filter provided.");
                        return BadRequest();
                    }
                    courses = courses.Where(o => o.Status.Equals(courseStatus));
                }

                if (hideCompleted.HasValue && hideCompleted.Value)
                {
                    courses = courses.Where(o => o.Status != CourseStatus.Delivered);
                }

                switch (sortOrder)
                {
                    case "date_desc":
                        courses = courses.OrderByDescending(o => o.AssignedDate);
                        break;
                    case "date_asc":
                        courses = courses.OrderBy(o => o.AssignedDate);
                        break;
                    default:
                        courses = courses.OrderBy(o => o.AssignedDate);
                        break;
                }

                IEnumerable<IndexCourseViewModel> indexAllCourses = await courses.Select((tc) => new IndexCourseViewModel
                {
                    TruckCourseID = tc.TruckCourseID,
                    TruckID = tc.TruckID,
                    PickupAddress = tc.PickupAddress,
                    DeliverAddress = tc.DeliverAddress,
                    AssignedDate = tc.AssignedDate,
                    DeliveryDate = tc.DeliveryDate,
                    Status = tc.Status,
                    Income = tc.Income
                })
                                                                                   .ToListAsync();

                return View(indexAllCourses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving orders.");
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            CreateCourseViewModel createCourseViewModel = new CreateCourseViewModel
            {
                AvailableTruckIDs = await _truckService.GetAll()
                                                       .Where(t => t.Status.Equals(TruckStatus.Available))
                                                       .ToDictionaryAsync(truck => truck.TruckID,
                                                                          truck => truck.LicensePlate)
            };

            return View(createCourseViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCourseViewModel createCourseViewModel)
        {
            if (!ModelState.IsValid)
            {
                createCourseViewModel.AvailableTruckIDs = await GetAvailableTrucksAsync();
                return View(createCourseViewModel);
            }

            try
            {
                if (!await _truckCourseService.CreateCourseAsync(createCourseViewModel))
                {
                    ModelState.AddModelError(string.Empty, "Failed to create the order. Please check the provided details and try again.");
                    createCourseViewModel.AvailableTruckIDs = await GetAvailableTrucksAsync(); ;
                    return View(createCourseViewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new order.");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                createCourseViewModel.AvailableTruckIDs = await GetAvailableTrucksAsync();
                return View(createCourseViewModel);
            }

            return RedirectToAction(nameof(Index), "Course");
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            //if (string.IsNullOrEmpty(id))
            //{
            //    _logger.LogError(id, "Order ID must be provided.");
            //    return NotFound();
            //}

            //if (!Guid.TryParse(id, out Guid orderId))
            //{
            //    _logger.LogError(id, "Invalid Order ID format.");
            //    return BadRequest();
            //}

            //AdminCreateOrderViewModel? createOrderViewModel = null;
            //try
            //{
            //    createOrderViewModel = await _orderService.GetAll()
            //                                                   .AsNoTracking()
            //                                                   .Where(o => o.OrderID.Equals(orderId))
            //                                                   .Select(o => new AdminCreateOrderViewModel
            //                                                   {
            //                                                       UsersId = o.UserID,
            //                                                       DeliveryAddress = o.DeliveryAddress,
            //                                                       PickupAddress = o.PickupAddress,
            //                                                       LoadCapacity = o.LoadCapacity,
            //                                                       DeliveryInstructions = o.DeliveryInstructions
            //                                                   }).SingleOrDefaultAsync();
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "An error occurred while retrieving order with ID {OrderId} for editing.", orderId);
            //    return BadRequest();
            //}

            //if (createOrderViewModel == null)
            //{
            //    _logger.LogError(nameof(createOrderViewModel), $"Order with ID {orderId} was not found.");
            //    return NotFound();
            //}

            //createOrderViewModel.Users = GetUsersInRole(UserRoles.User.ToString());

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminCreateOrderViewModel createOrderViewModel, string? id)
        {
            //if (string.IsNullOrEmpty(id))
            //{
            //    _logger.LogError(id, "Order ID must be provided.");
            //    ModelState.AddModelError(nameof(id), "Order ID must be provided.");
            //    return NotFound();
            //}

            //if (!Guid.TryParse(id, out Guid orderId))
            //{
            //    _logger.LogError(id, "Invalid Order ID format.");
            //    ModelState.AddModelError(nameof(orderId), "Invalid Order ID format.");
            //    return BadRequest();
            //}

            //if (!ModelState.IsValid)
            //{
            //    createOrderViewModel.Users = GetUsersInRole(UserRoles.User.ToString());
            //    return View(createOrderViewModel);
            //}

            //try
            //{
            //    var order = await _orderService.GetAll()
            //                                    .AsNoTracking()
            //                                    .Where(o => o.OrderID.Equals(orderId))
            //                                    .SingleOrDefaultAsync();

            //    if (order == null)
            //    {
            //        return NotFound($"Order with ID {orderId} was not found.");
            //    }

            //    if (!await _orderService.UpdateOrderAsync(createOrderViewModel, orderId, createOrderViewModel.UsersId))
            //    {
            //        ModelState.AddModelError(string.Empty, "Failed to update the order. The order may have been modified by another user.");
            //        createOrderViewModel.Users = GetUsersInRole(UserRoles.User.ToString());
            //        return View(createOrderViewModel);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "An error occurred while updating order with ID {OrderId}.", orderId);
            //    ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
            //    createOrderViewModel.Users = GetUsersInRole(UserRoles.User.ToString());
            //    return View(createOrderViewModel);
            //}

            return RedirectToAction(nameof(Detail), "Create", new { id = id });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogError(id, "Course ID must be provided.");
                return BadRequest();
            }

            if (!Guid.TryParse(id, out Guid courseId))
            {
                _logger.LogError(id, "Invalid Course ID format.");
                return NotFound();
            }

            DetailsCourseViewModel? course = null;
            try
            {
                course = await _truckCourseService.GetAll()
                                                  .AsNoTracking()
                                                  .Include(tc => tc.CourseOrders)
                                                  .Include(tc => tc.Truck)
                                                  .Where(tc => tc.TruckCourseID.Equals(courseId))
                                                  .Select(tc => new DetailsCourseViewModel
                                                  {
                                                      TruckCourseID = tc.TruckCourseID,
                                                      TruckID = tc.TruckID,
                                                      TruckPlates = tc.Truck.LicensePlate ?? string.Empty,
                                                      PickupAddress = tc.PickupAddress,
                                                      DeliverAddress = tc.DeliverAddress,
                                                      AssignedDate = tc.AssignedDate,
                                                      DeliveryDate = tc.DeliveryDate,
                                                      Status = tc.Status,
                                                      Income = tc.Income,
                                                      AssinedOrders = tc.CourseOrders.Select(co => new IndexOrderViewModel
                                                      {
                                                          OrderID = co.OrderID,
                                                          OrderDate = co.Order.OrderDate,
                                                          DeliveryAddress = co.Order.DeliveryAddress,
                                                          PickupAddress = co.Order.PickupAddress,
                                                          Status = co.Order.Status.ToString(),
                                                          isCanceled = co.Order.IsCanceled
                                                      })
                                                      .ToList(),

                                                  })
                                                  .SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving details for course with ID {courseId}.", courseId);
                return BadRequest();
            }

            if (course == null)
            {

                _logger.LogError(nameof(course), "An error occurred while retrieving details for course with ID {courseId}.", courseId);
                return NotFound();
            }

            return View(course);
        }
    }
}
