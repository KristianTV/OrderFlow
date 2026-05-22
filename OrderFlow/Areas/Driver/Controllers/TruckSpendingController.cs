using Microsoft.AspNetCore.Mvc;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.TruckSpending;

namespace OrderFlow.Areas.Driver.Controllers
{
    public class TruckSpendingController : BaseDriverController
    {
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
        public async Task<IActionResult> Index(Guid? truckId = null, Guid? courseId = null, string? search = null, string? sortOrder = null)
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

            IEnumerable<IndexTruckSpendingViewModel> spendings = await _truckSpendingService.GetTruckSpendingsAsync(driverId, new TruckSpendingQueryModel
            {
                TruckID = truckId,
                TruckCourseID = courseId,
                Search = search,
                SortOrder = sortOrder
            });

            if (IsAjaxRequest())
            {
                return PartialView("_SpendingList", spendings);
            }

            return View(spendings);
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
            return Request.Headers.XRequestedWith == "XMLHttpRequest";
        }
    }
}
