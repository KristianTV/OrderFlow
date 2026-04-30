using Microsoft.AspNetCore.Mvc;
using OrderFlow.Data.Models;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Course;

namespace OrderFlow.Areas.Admin.Controllers
{
    public class CourseController : BaseAdminController
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
                IEnumerable<IndexCourseViewModel> indexAllCourses = await _truckCourseService.GetCoursesAsync(null, new CourseQueryModel
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
        public async Task<IActionResult> Create()
        {
            CreateCourseViewModel createCourseViewModel = new CreateCourseViewModel
            {
                AvailableTruckIDs = await _truckService.GetAvailableTruckOptionsAsync()
            };

            return View(createCourseViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCourseViewModel createCourseViewModel)
        {
            if (!ModelState.IsValid)
            {
                createCourseViewModel.AvailableTruckIDs = await _truckService.GetAvailableTruckOptionsAsync();
                return View(createCourseViewModel);
            }

            try
            {
                if (!await _truckCourseService.CreateCourseAsync(createCourseViewModel))
                {
                    ModelState.AddModelError(string.Empty, "Failed to create the order. Please check the provided details and try again.");
                    createCourseViewModel.AvailableTruckIDs = await _truckService.GetAvailableTruckOptionsAsync();
                    return View(createCourseViewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new order.");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                createCourseViewModel.AvailableTruckIDs = await _truckService.GetAvailableTruckOptionsAsync();
                return View(createCourseViewModel);
            }

            return RedirectToAction(nameof(Index), "Course");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogError(id, "Course ID must be provided.");
                return NotFound();
            }

            if (!Guid.TryParse(id, out Guid CourseId))
            {
                _logger.LogError(id, "Invalid Course ID format.");
                return BadRequest();
            }

            CreateCourseViewModel? createCourseViewModel = null;
            try
            {
                createCourseViewModel = await _truckCourseService.GetCourseForEditAsync(CourseId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving course with ID {CourseId} for editing.", CourseId);
                return BadRequest();
            }

            if (createCourseViewModel == null)
            {
                _logger.LogError(nameof(createCourseViewModel), $"Course with ID {CourseId} was not found.");
                return NotFound();
            }

            createCourseViewModel.AvailableTruckIDs = await _truckService.GetAvailableTruckOptionsAsync();

            return View(createCourseViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateCourseViewModel createCourseViewModel, string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogError(id, "Course ID must be provided.");
                ModelState.AddModelError(nameof(id), "Course ID must be provided.");
                return NotFound();
            }

            if (!Guid.TryParse(id, out Guid courseId))
            {
                _logger.LogError(id, "Invalid Course ID format.");
                ModelState.AddModelError(nameof(courseId), "Invalid Course ID format.");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                createCourseViewModel.AvailableTruckIDs = await _truckService.GetAvailableTruckOptionsAsync();
                return View(createCourseViewModel);
            }

            try
            {
                if (await _truckCourseService.ExistsAsync<TruckCourse>(courseId))
                {
                    return NotFound($"Course with ID {courseId} was not found.");
                }

                if (!await _truckCourseService.UpdateCourseAsync(createCourseViewModel, courseId))
                {
                    ModelState.AddModelError(string.Empty, "Failed to update the course. The course may have been modified by another user.");
                    createCourseViewModel.AvailableTruckIDs = await _truckService.GetAvailableTruckOptionsAsync();
                    return View(createCourseViewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating course with ID {courseId}.", courseId);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                createCourseViewModel.AvailableTruckIDs = await _truckService.GetAvailableTruckOptionsAsync();
                return View(createCourseViewModel);
            }

            return RedirectToAction(nameof(Detail), "Course", new { id = id });
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
                course = await _truckCourseService.GetCourseDetailsAsync(courseId);
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
        public async Task<IActionResult> Delete(string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid courseID))
            {
                _logger.LogWarning("Delete POST: Invalid or missing Course ID '{CourseID}'.", id);
                TempData["Error"] = "Invalid or missing Course ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                bool success = await _truckCourseService.DeleteCourseAsync(courseID);
                if (success)
                {
                    TempData["Success"] = "Course successfully deleted.";
                    _logger.LogInformation("Course ID: {CourseId} soft deleted.", courseID);
                }
                else
                {
                    TempData["Error"] = "Failed to delete the course. It may no longer exist.";
                    _logger.LogWarning("SoftDeleteTruckAsync failed for Course ID: {CourseId}", courseID);
                }

                return RedirectToAction(nameof(Index), "Course");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting course with ID '{CourseId}'.", courseID);
                TempData["Error"] = "An unexpected error occurred while deleting the course. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
