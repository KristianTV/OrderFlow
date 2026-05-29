using Microsoft.AspNetCore.Mvc;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.TruckSpending;

namespace OrderFlow.Areas.Driver.Controllers
{
    public class TruckSpendingController : BaseDriverController
    {
        private const int IndexPageSize = 12;
        private readonly ILogger<TruckSpendingController> _logger;
        private readonly ITruckSpendingService _truckSpendingService;
        private readonly IRealtimeNotifier _realtimeNotifier;

        public TruckSpendingController(
            ILogger<TruckSpendingController> logger,
            ITruckSpendingService truckSpendingService,
            IRealtimeNotifier? realtimeNotifier = null)
        {
            _logger = logger;
            _truckSpendingService = truckSpendingService;
            _realtimeNotifier = realtimeNotifier ?? NullRealtimeNotifier.Instance;
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid? truckId = null, Guid? courseId = null, string? search = null, string? sortOrder = null, int page = 1)
        {
            if (!TryGetDriverId(out Guid driverId))
            {
                return BadRequest();
            }

            ViewBag.Trucks = await _truckSpendingService.GetTruckOptionsAsync(driverId);
            ViewBag.Courses = await _truckSpendingService.GetCourseOptionsAsync(truckId, driverId);
            ViewBag.CurrentTruckId = truckId;
            ViewBag.CurrentCourseId = courseId;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentSortOrder = sortOrder;

            List<IndexTruckSpendingViewModel> indexSpendings = (await _truckSpendingService.GetTruckSpendingsAsync(driverId, new TruckSpendingQueryModel
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

        [HttpGet]
        public async Task<IActionResult> Courses(Guid? truckId = null)
        {
            if (!TryGetDriverId(out Guid driverId))
            {
                return BadRequest();
            }

            Dictionary<Guid, string> courses = await _truckSpendingService.GetCourseOptionsAsync(truckId, driverId);

            return Json(courses.Select(course => new
            {
                id = course.Key,
                text = course.Value
            }));
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid? truckId = null, Guid? courseId = null)
        {
            if (!TryGetDriverId(out Guid driverId))
            {
                return BadRequest();
            }

            CreateTruckSpendingViewModel model = new CreateTruckSpendingViewModel
            {
                TruckID = truckId,
                TruckCourseID = courseId
            };

            await PopulateOptionsAsync(model, driverId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTruckSpendingViewModel model)
        {
            if (!TryGetDriverId(out Guid driverId))
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await PopulateOptionsAsync(model, driverId);
                return View(model);
            }

            if (await _truckSpendingService.CreateTruckSpendingAsync(model, driverId))
            {
                TempData["Success"] = "Truck spending created successfully.";
                await NotifyTruckSpendingChangedAsync("Created", null, driverId);
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Failed to create truck spending. Check the selected truck and course.");
            await PopulateOptionsAsync(model, driverId);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            if (!TryGetDriverId(out Guid driverId))
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid spendingId))
            {
                TempData["Error"] = "Invalid or missing spending ID.";
                return RedirectToAction(nameof(Index));
            }

            CreateTruckSpendingViewModel? model = await _truckSpendingService.GetTruckSpendingForEditAsync(spendingId, driverId);
            if (model == null)
            {
                TempData["Error"] = "Truck spending not found.";
                return RedirectToAction(nameof(Index));
            }

            await PopulateOptionsAsync(model, driverId);
            ViewBag.SpendingId = spendingId;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateTruckSpendingViewModel model, string? id)
        {
            if (!TryGetDriverId(out Guid driverId))
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid spendingId))
            {
                TempData["Error"] = "Invalid or missing spending ID.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                await PopulateOptionsAsync(model, driverId);
                ViewBag.SpendingId = spendingId;
                return View(model);
            }

            if (await _truckSpendingService.UpdateTruckSpendingAsync(spendingId, model, driverId))
            {
                TempData["Success"] = "Truck spending updated successfully.";
                await NotifyTruckSpendingChangedAsync("Updated", spendingId, driverId);
                return RedirectToAction(nameof(Detail), new { id = spendingId });
            }

            ModelState.AddModelError(string.Empty, "Failed to update truck spending. Check the selected truck and course.");
            await PopulateOptionsAsync(model, driverId);
            ViewBag.SpendingId = spendingId;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string? id)
        {
            if (!TryGetDriverId(out Guid driverId))
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid spendingId))
            {
                TempData["Error"] = "Invalid or missing spending ID.";
                return RedirectToAction(nameof(Index));
            }

            DetailsTruckSpendingViewModel? spending = await _truckSpendingService.GetTruckSpendingDetailsAsync(spendingId, driverId);
            if (spending == null)
            {
                TempData["Error"] = "Truck spending not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(spending);
        }

        private bool TryGetDriverId(out Guid driverId)
        {
            if (!Guid.TryParse(GetUserId(), out driverId))
            {
                _logger.LogWarning("Invalid Driver ID format from session.");
                return false;
            }

            return true;
        }

        private async Task PopulateOptionsAsync(CreateTruckSpendingViewModel model, Guid driverId)
        {
            model.AvailableTrucks = await _truckSpendingService.GetTruckOptionsAsync(driverId);
            model.AvailableCourses = model.TruckID.HasValue
                ? await _truckSpendingService.GetCourseOptionsAsync(model.TruckID, driverId)
                : new Dictionary<Guid, string>();
        }

        private bool IsAjaxRequest()
        {
            return HttpContext?.Request?.Headers.XRequestedWith.ToString() == "XMLHttpRequest";
        }

        private async Task NotifyTruckSpendingChangedAsync(string action, Guid? spendingId, Guid driverId)
        {
            await _realtimeNotifier.EntityChangedAsync(new RealtimeEntityChanged
            {
                Entity = "TruckSpending",
                Action = action,
                Id = spendingId,
                UserIds = new[] { driverId },
                Roles = new[] { UserRoles.Admin.ToString(), UserRoles.Speditor.ToString() }
            });
        }
    }
}
