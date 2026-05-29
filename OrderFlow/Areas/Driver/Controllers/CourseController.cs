using Microsoft.AspNetCore.Mvc;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Course;

namespace OrderFlow.Areas.Driver.Controllers
{
    public class CourseController : BaseDriverController
    {
        private const int IndexPageSize = 12;
        private readonly ILogger<CourseController> _logger;
        private readonly ITruckCourseService _truckCourseService;
        private readonly ITruckService _truckService;
        private readonly IRealtimeNotifier _realtimeNotifier;

        public CourseController(ILogger<CourseController> logger,
                                    ITruckCourseService truckCourseService,
                                    ITruckService truckService,
                                    IRealtimeNotifier? realtimeNotifier = null)
        {
            _logger = logger;
            _truckCourseService = truckCourseService;
            _truckService = truckService;
            _realtimeNotifier = realtimeNotifier ?? NullRealtimeNotifier.Instance;
        }

        [HttpGet]
        public async Task<IActionResult> Index(bool? hideCompleted, string? searchId = null, string? statusFilter = null, string? sortOrder = null, int page = 1)
        {
            try
            {
                if (!Guid.TryParse(this.GetUserId(), out Guid userID))
                {
                    _logger.LogError("User ID is not valid.");
                    return RedirectToAction("Error", "Home");
                }

                List<IndexCourseViewModel> indexCourses = (await _truckCourseService.GetCoursesAsync(userID, new CourseQueryModel
                {
                    HideCompleted = hideCompleted.GetValueOrDefault(),
                    SearchId = searchId,
                    StatusFilter = statusFilter,
                    SortOrder = sortOrder,
                    Page = page,
                    PageSize = IndexPageSize + 1
                })).ToList();

                bool hasMore = indexCourses.Count > IndexPageSize;
                HttpContext?.Response?.Headers.TryAdd("X-Has-More", hasMore.ToString().ToLowerInvariant());
                indexCourses = indexCourses.Take(IndexPageSize).ToList();

                if (IsAjaxRequest())
                {
                    ViewData["CourseArea"] = "Driver";
                    return PartialView("~/Views/Shared/_CourseRows.cshtml", indexCourses);
                }

                ViewBag.HasMore = hasMore;

                return View(indexCourses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving orders.");
                return BadRequest();
            }
        }

        private bool IsAjaxRequest()
        {
            return HttpContext?.Request?.Headers.XRequestedWith.ToString() == "XMLHttpRequest";
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
            catch (Exception)
            {
                _logger.LogError("An error occurred while retrieving details for course with ID {courseId}.", courseId);
                return BadRequest();
            }

            if (course == null)
            {

                _logger.LogError("An error occurred while retrieving details for course with ID {courseId}.", courseId);
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
            await _realtimeNotifier.EntityChangedAsync(new RealtimeEntityChanged
            {
                Entity = "Course",
                Action = "Completed",
                Id = courseId,
                Roles = new[] { UserRoles.Admin.ToString(), UserRoles.Speditor.ToString(), UserRoles.Driver.ToString() }
            });

            return RedirectToAction(nameof(Detail), "Course", new { id = courseId });
        }
    }
}
