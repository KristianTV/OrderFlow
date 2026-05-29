using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Truck;

namespace OrderFlow.Areas.Admin.Controllers
{
    public class TruckController : BaseAdminController
    {
        private const int IndexPageSize = 12;
        private readonly ILogger<TruckController> _logger;
        private readonly ITruckService _truckService;
        private readonly IOrderService _orderService;
        private readonly ICourseOrderService _truckOrderService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRealtimeNotifier _realtimeNotifier;

        public TruckController(ILogger<TruckController> logger,
                                ITruckService truckService,
                                UserManager<ApplicationUser> userManager,
                                IOrderService orderService,
                                ICourseOrderService truckOrderService,
                                IRealtimeNotifier? realtimeNotifier = null)
        {
            _logger = logger;
            _truckService = truckService;
            _userManager = userManager;
            _orderService = orderService;
            _truckOrderService = truckOrderService;
            _realtimeNotifier = realtimeNotifier ?? NullRealtimeNotifier.Instance;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? status = null, string? search = null, string? sortOrder = null, int page = 1)
        {
            try
            {
                List<IndexTruckViewModel> trucks = (await _truckService.GetTrucksAsync(query: new TruckQueryModel
                {
                    Search = search,
                    Status = status,
                    SortOrder = sortOrder,
                    Page = page,
                    PageSize = IndexPageSize + 1
                })).ToList();

                bool hasMore = trucks.Count > IndexPageSize;
                HttpContext?.Response?.Headers.TryAdd("X-Has-More", hasMore.ToString().ToLowerInvariant());
                trucks = trucks.Take(IndexPageSize).ToList();

                ViewData["CurrentStatus"] = string.IsNullOrWhiteSpace(status) || status.Equals("all", StringComparison.OrdinalIgnoreCase)
                    ? "All"
                    : status;
                ViewData["CurrentSearch"] = search;
                ViewData["CurrentSortOrder"] = sortOrder;

                if (IsAjaxRequest())
                {
                    ViewData["TruckArea"] = "Admin";
                    return PartialView("~/Views/Shared/_TruckCards.cshtml", trucks);
                }

                ViewBag.HasMore = hasMore;

                return View(trucks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the list of trucks.");
                TempData["Error"] = "An unexpected error occurred. Please try again later.";
                return View(new List<IndexTruckViewModel>());
            }
        }

        private bool IsAjaxRequest()
        {
            return HttpContext?.Request?.Headers.XRequestedWith.ToString() == "XMLHttpRequest";
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var users = await GetUsersInRoleAsync(UserRoles.Driver.ToString());

                if (users == null || !users.Any())
                {
                    _logger.LogWarning("No drivers found to create a new truck.");
                    TempData["Error"] = "No drivers available. Please add a driver first.";
                    return View(new CreateTruckViewModel());
                }

                CreateTruckViewModel createTruckViewModel = new CreateTruckViewModel
                {
                    Drivers = users!
                };

                return View(createTruckViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while preparing the truck creation form.");
                TempData["Error"] = "An unexpected error occurred. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTruckViewModel createTruckViewModel)
        {
            if (!ModelState.IsValid)
            {
                createTruckViewModel.Drivers = await GetUsersInRoleAsync(UserRoles.Driver.ToString());
                return View(createTruckViewModel);
            }

            try
            {
                if (await _truckService.CreateTruckAsync(createTruckViewModel))
                {
                    TempData["Success"] = "Truck created successfully.";
                    await NotifyTruckChangedAsync("Created", null, createTruckViewModel.DriverID);
                    return RedirectToAction(nameof(Index), "Truck");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to create truck. Please check the provided details.");
                    createTruckViewModel.Drivers = await GetUsersInRoleAsync(UserRoles.Driver.ToString());
                    return View(createTruckViewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new truck.");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                createTruckViewModel.Drivers = await GetUsersInRoleAsync(UserRoles.Driver.ToString());
                return View(createTruckViewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid truckId))
            {
                _logger.LogWarning("Edit GET: Invalid or missing Truck ID '{TruckId}'", id);
                TempData["Error"] = "Invalid or missing truck ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                CreateTruckViewModel? createTruckViewModel = await _truckService.GetTruckForEditAsync(truckId);

                if (createTruckViewModel == null)
                {
                    _logger.LogWarning("Edit GET: Truck with ID '{TruckId}' not found.", truckId);
                    TempData["Error"] = "Truck not found.";
                    return RedirectToAction(nameof(Index));
                }

                createTruckViewModel.Drivers = await GetUsersInRoleAsync(UserRoles.Driver.ToString());

                ViewBag.TruckId = truckId;
                return View(createTruckViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving truck details for ID '{TruckId}'.", truckId);
                TempData["Error"] = "An unexpected error occurred. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateTruckViewModel createTruckViewModel, string? id)
        {
            Guid truckId = Guid.Empty;
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out truckId))
            {
                _logger.LogWarning("Edit POST: Invalid or missing Truck ID '{TruckId}'", id);
                ModelState.AddModelError(string.Empty, "Invalid or missing truck ID.");
                createTruckViewModel.Drivers = await GetUsersInRoleAsync(UserRoles.Driver.ToString());
                ViewBag.TruckId = truckId;
                return View(createTruckViewModel);
            }

            if (!ModelState.IsValid)
            {
                createTruckViewModel.Drivers = await GetUsersInRoleAsync(UserRoles.Driver.ToString());
                return View(createTruckViewModel);
            }

            try
            {
                if (await _truckService.UpdateTruckAsync(createTruckViewModel, truckId))
                {
                    TempData["Success"] = "Truck updated successfully.";
                    await NotifyTruckChangedAsync("Updated", truckId, createTruckViewModel.DriverID);
                    return RedirectToAction(nameof(Detail), "Truck", new { id = id });
                }
                else
                {
                    _logger.LogWarning("Edit POST: UpdateTruckAsync failed for Truck ID '{TruckId}'.", truckId);
                    ModelState.AddModelError(string.Empty, "Failed to update truck. It may no longer exist.");
                    createTruckViewModel.Drivers = await GetUsersInRoleAsync(UserRoles.Driver.ToString());
                    ViewBag.TruckId = truckId;
                    return View(createTruckViewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating truck with ID '{TruckId}'.", truckId);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                createTruckViewModel.Drivers = await GetUsersInRoleAsync(UserRoles.Driver.ToString());
                return View(createTruckViewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid truckID))
            {
                _logger.LogWarning("Detail GET: Invalid or missing Truck ID '{TruckId}'", id);
                TempData["Error"] = "Invalid or missing truck ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                DetailsTruckViewModel? truckDetail = await _truckService.GetTruckDetailsAsync(truckID);

                if (truckDetail == null)
                {
                    _logger.LogWarning("Detail GET: Truck with ID '{TruckId}' not found.", truckID);
                    TempData["Error"] = "Truck not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(truckDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving truck details for ID '{TruckId}'.", truckID);
                TempData["Error"] = "An unexpected error occurred. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid truckID))
            {
                _logger.LogWarning("Delete POST: Invalid or missing Truck ID '{TruckId}'.", id);
                TempData["Error"] = "Invalid or missing truck ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                bool success = await _truckService.SoftDeleteTruckAsync(truckID);
                if (success)
                {
                    TempData["Success"] = "Truck successfully deleted.";
                    _logger.LogInformation("Truck ID: {TruckId} soft deleted.", truckID);
                    await NotifyTruckChangedAsync("Deleted", truckID);
                }
                else
                {
                    TempData["Error"] = "Failed to delete the truck. It may no longer exist.";
                    _logger.LogWarning("SoftDeleteTruckAsync failed for Truck ID: {TruckId}", truckID);
                }

                return RedirectToAction(nameof(Index), "Truck");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting truck with ID '{TruckId}'.", truckID);
                TempData["Error"] = "An unexpected error occurred while deleting the truck. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task<Dictionary<Guid, string>> GetUsersInRoleAsync(string roleName)
        {
            try
            {
                var users = await _userManager.GetUsersInRoleAsync(roleName);
                return users.ToDictionary(user => user.Id, user => user.UserName ?? string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve users in role '{RoleName}'.", roleName);
                return new Dictionary<Guid, string>();
            }
        }

        private async Task NotifyTruckChangedAsync(string action, Guid? truckId = null, Guid? driverId = null)
        {
            await _realtimeNotifier.EntityChangedAsync(new RealtimeEntityChanged
            {
                Entity = "Truck",
                Action = action,
                Id = truckId,
                UserIds = driverId.HasValue ? new[] { driverId.Value } : Enumerable.Empty<Guid>(),
                Roles = new[] { UserRoles.Admin.ToString(), UserRoles.Speditor.ToString(), UserRoles.Driver.ToString() }
            });
        }
    }
}
