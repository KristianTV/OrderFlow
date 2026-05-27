using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.TruckSpending;

namespace OrderFlow.Areas.Admin.Controllers
{
    public class TruckSpendingController : BaseAdminController
    {
        private const int IndexPageSize = 12;
        private readonly ILogger<TruckSpendingController> _logger;
        private readonly ITruckSpendingService _truckSpendingService;

        public TruckSpendingController(
            ILogger<TruckSpendingController> logger,
            ITruckSpendingService truckSpendingService)
        {
            _logger = logger;
            _truckSpendingService = truckSpendingService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid? truckId = null, Guid? courseId = null, string? search = null, string? sortOrder = null, int page = 1)
        {
            try
            {
                ViewBag.Trucks = await _truckSpendingService.GetTruckOptionsAsync();
                ViewBag.Courses = await _truckSpendingService.GetCourseOptionsAsync(truckId);
                ViewBag.CurrentTruckId = truckId;
                ViewBag.CurrentCourseId = courseId;
                ViewBag.CurrentSearch = search;
                ViewBag.CurrentSortOrder = sortOrder;

                List<IndexTruckSpendingViewModel> indexSpendings = (await _truckSpendingService.GetTruckSpendingsAsync(null, new TruckSpendingQueryModel
                {
                    TruckID = truckId,
                    TruckCourseID = courseId,
                    Search = search,
                    SortOrder = sortOrder,
                    Page = page,
                    PageSize = IndexPageSize + 1
                })).ToList();

                bool hasMore = indexSpendings.Count > IndexPageSize;
                HttpContext?.Response?.Headers.TryAdd("X-Has-More", hasMore.ToString().ToLowerInvariant());
                indexSpendings = indexSpendings.Take(IndexPageSize).ToList();

                if (IsAjaxRequest())
                {
                    return PartialView("_SpendingList", indexSpendings);
                }

                ViewBag.HasMore = hasMore;

                return View(indexSpendings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving truck spendings.");
                TempData["Error"] = "An unexpected error occurred while loading truck spendings.";
                return View(new List<IndexTruckSpendingViewModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Courses(Guid? truckId = null)
        {
            Dictionary<Guid, string> courses = await _truckSpendingService.GetCourseOptionsAsync(truckId);

            return Json(courses.Select(course => new
            {
                id = course.Key,
                text = course.Value
            }));
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid? truckId = null, Guid? courseId = null)
        {
            CreateTruckSpendingViewModel model = new CreateTruckSpendingViewModel
            {
                TruckID = truckId,
                TruckCourseID = courseId
            };

            await PopulateOptionsAsync(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTruckSpendingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateOptionsAsync(model);
                return View(model);
            }

            try
            {
                if (await _truckSpendingService.CreateTruckSpendingAsync(model))
                {
                    TempData["Success"] = "Truck spending created successfully.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "Failed to create truck spending. Check the selected truck and course.");
                await PopulateOptionsAsync(model);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a truck spending.");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                await PopulateOptionsAsync(model);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid spendingId))
            {
                TempData["Error"] = "Invalid or missing spending ID.";
                return RedirectToAction(nameof(Index));
            }

            CreateTruckSpendingViewModel? model = await _truckSpendingService.GetTruckSpendingForEditAsync(spendingId);
            if (model == null)
            {
                TempData["Error"] = "Truck spending not found.";
                return RedirectToAction(nameof(Index));
            }

            await PopulateOptionsAsync(model);
            ViewBag.SpendingId = spendingId;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateTruckSpendingViewModel model, string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid spendingId))
            {
                TempData["Error"] = "Invalid or missing spending ID.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                await PopulateOptionsAsync(model);
                ViewBag.SpendingId = spendingId;
                return View(model);
            }

            try
            {
                if (await _truckSpendingService.UpdateTruckSpendingAsync(spendingId, model))
                {
                    TempData["Success"] = "Truck spending updated successfully.";
                    return RedirectToAction(nameof(Detail), new { id = spendingId });
                }

                ModelState.AddModelError(string.Empty, "Failed to update truck spending. Check the selected truck and course.");
                await PopulateOptionsAsync(model);
                ViewBag.SpendingId = spendingId;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating truck spending {SpendingId}.", spendingId);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                await PopulateOptionsAsync(model);
                ViewBag.SpendingId = spendingId;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid spendingId))
            {
                TempData["Error"] = "Invalid or missing spending ID.";
                return RedirectToAction(nameof(Index));
            }

            DetailsTruckSpendingViewModel? spending = await _truckSpendingService.GetTruckSpendingDetailsAsync(spendingId);
            if (spending == null)
            {
                TempData["Error"] = "Truck spending not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(spending);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid spendingId))
            {
                TempData["Error"] = "Invalid or missing spending ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (await _truckSpendingService.DeleteTruckSpendingAsync(spendingId))
                {
                    TempData["Success"] = "Truck spending deleted successfully.";
                }
                else
                {
                    TempData["Error"] = "Truck spending could not be deleted.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting truck spending {SpendingId}.", spendingId);
                TempData["Error"] = "An unexpected error occurred while deleting the spending.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateOptionsAsync(CreateTruckSpendingViewModel model)
        {
            model.AvailableTrucks = await _truckSpendingService.GetTruckOptionsAsync();
            model.AvailableCourses = model.TruckID.HasValue
                                    ? await _truckSpendingService.GetCourseOptionsAsync(model.TruckID)
                                    : new Dictionary<Guid, string>();
        }

        private bool IsAjaxRequest()
        {
            return HttpContext?.Request?.Headers.XRequestedWith.ToString() == "XMLHttpRequest";
        }
    }
}
