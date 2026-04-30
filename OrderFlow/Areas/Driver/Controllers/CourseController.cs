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

                IEnumerable<IndexCourseViewModel> indexAllCourses = await _truckCourseService.GetCoursesAsync(userID, new CourseQueryModel
                {
                    HideCompleted = hideCompleted.GetValueOrDefault(),
                    SearchId = searchId,
                    StatusFilter = statusFilter,
                    SortOrder = sortOrder
                });

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
                course = await _truckCourseService.GetCourseDetailsAsync(courseId, userID);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(string? id)
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

            await _truckCourseService.CompleteCourseAsync(courseId);

            return RedirectToAction(nameof(Detail), "Course", new { id = courseId });
        }
    }
}
