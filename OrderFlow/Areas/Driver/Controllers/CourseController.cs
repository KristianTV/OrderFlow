using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Course;
using OrderFlow.ViewModels.Order;

namespace OrderFlow.Areas.Driver.Controllers
{
    public class CourseController : BaseDriverController
    {
        private readonly ILogger<CourseController> _logger;
        private readonly ITruckCourseService _truckCourseService;
        private readonly ITruckService _truckService;

        public CourseController(ILogger<CourseController> logger,
                                    ITruckCourseService truckCourseService,
                                    ITruckService truckService)
        {
            _logger = logger;
            _truckCourseService = truckCourseService;
            _truckService = truckService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(bool? hideCompleted, string? searchId = null, string? statusFilter = null, string? sortOrder = null)
        {
            try
            {
                if (!Guid.TryParse(this.GetUserId(), out Guid userID))
                {
                    _logger.LogError("User ID is not valid.");
                    return RedirectToAction("Error", "Home");
                }

                var courses = _truckCourseService.GetAll()
                                                 .AsNoTracking()
                                                 .Include(tc => tc.Truck)
                                                 .Where(tc => tc.Truck != null &&
                                                              tc.Truck.DriverID.Equals(userID));

                if (!string.IsNullOrEmpty(searchId))
                {
                    courses = courses.Where(tc => tc.TruckCourseID.ToString().Contains(searchId));
                }

                if (!string.IsNullOrEmpty(statusFilter))
                {
                    if (!Enum.TryParse(statusFilter, out CourseStatus courseStatus))
                    {
                        _logger.LogWarning("Invalid status filter provided.");
                        return BadRequest();
                    }
                    courses = courses.Where(tc => tc.Status.Equals(courseStatus));
                }

                if (hideCompleted.HasValue && hideCompleted.Value)
                {
                    courses = courses.Where(tc => tc.Status != CourseStatus.Delivered);
                }

                switch (sortOrder)
                {
                    case "date_desc":
                        courses = courses.OrderByDescending(tc => tc.AssignedDate);
                        break;
                    case "date_asc":
                        courses = courses.OrderBy(tc => tc.AssignedDate);
                        break;
                    default:
                        courses = courses.OrderBy(tc => tc.AssignedDate);
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

            if (!Guid.TryParse(this.GetUserId(), out Guid userID))
            {
                _logger.LogError("User ID is not valid.");
                return RedirectToAction("Error", "Home");
            }

            DetailsCourseViewModel? course = null;
            try
            {
                course = await _truckCourseService.GetAll()
                                                  .AsNoTracking()
                                                  .Include(tc => tc.CourseOrders)
                                                  .Include(tc => tc.Truck)
                                                  .Where(tc => tc.TruckCourseID.Equals(courseId) &&
                                                               tc.Truck != null &&
                                                               tc.Truck.DriverID.Equals(userID))
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
