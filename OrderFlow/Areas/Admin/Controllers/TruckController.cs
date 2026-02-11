using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Truck;

namespace OrderFlow.Areas.Admin.Controllers
{
    public class TruckController : BaseAdminController
    {
        private readonly ILogger<TruckController> _logger;
        private readonly ITruckService _truckService;
        private readonly IOrderService _orderService;
        private readonly ICourseOrderService _truckOrderService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TruckController(ILogger<TruckController> logger,
                                ITruckService truckService,
                                UserManager<ApplicationUser> userManager,
                                IOrderService orderService,
                                ICourseOrderService truckOrderService)
        {
            _logger = logger;
            _truckService = truckService;
            _userManager = userManager;
            _orderService = orderService;
            _truckOrderService = truckOrderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? status = null)
        {
            try
            {
                var trucks = await _truckService.GetAll()
                                                 .AsNoTracking()
                                                 .Include(t => t.Driver)
                                                 .Select(truck => new IndexTruckViewModel
                                                 {
                                                     TruckID = truck.TruckID,
                                                     DriverName = truck.Driver!.UserName!,
                                                     LicensePlate = truck.LicensePlate,
                                                     Capacity = truck.Capacity,
                                                     Status = truck.Status.ToString(),
                                                 })
                                                 .ToListAsync();

                if (trucks == null || !trucks.Any())
                {
                    _logger.LogInformation("No trucks found .");
                    ModelState.AddModelError("Trucks", "No trucks found .");
                    return View(new List<IndexTruckViewModel>());
                }

                if (status != null)
                {
                    if (status.ToLower().Equals("all"))
                    {
                        ViewData["CurrentStatus"] = "All";
                        status = null;
                    }
                }

                if (status != null)
                {
                    if (!Enum.TryParse<TruckStatus>(status, true, out TruckStatus truckStatus))
                    {
                        _logger.LogWarning("Invalid truck status filter provided: {0}", status);
                        ModelState.AddModelError(nameof(status), string.Join("Invalid truck status filter provided: {0}", status));
                        return BadRequest();
                    }

                    switch (truckStatus)
                    {
                        case TruckStatus.Available:
                            trucks = trucks.Where(t => t.Status.Equals(TruckStatus.Available.ToString())).ToList();
                            ViewData["CurrentStatus"] = TruckStatus.Available.ToString();
                            break;
                        case TruckStatus.Unavailable:
                            trucks = trucks.Where(t => t.Status.Equals(TruckStatus.Unavailable.ToString())).ToList();

                            ViewData["CurrentStatus"] = TruckStatus.Unavailable.ToString();
                            break;
                        case TruckStatus.UnderMaintenance:
                            trucks = trucks.Where(t => t.Status.Equals(TruckStatus.UnderMaintenance.ToString())).ToList();

                            ViewData["CurrentStatus"] = TruckStatus.UnderMaintenance.ToString();
                            break;
                        case TruckStatus.Delayed:
                            trucks = trucks.Where(t => t.Status.Equals(TruckStatus.Delayed.ToString())).ToList();
                            ViewData["CurrentStatus"] = TruckStatus.Delayed.ToString();
                            break;
                        default:
                            _logger.LogWarning("Invalid status filter provided: {0}", truckStatus);
                            ModelState.AddModelError(nameof(truckStatus), string.Join("Invalid status filter provided: {0}", truckStatus));
                            return BadRequest();
                    }
                }


                return View(trucks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the list of trucks.");
                TempData["Error"] = "An unexpected error occurred. Please try again later.";
                return View(new List<IndexTruckViewModel>());
            }
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
                    Drivers = users
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
                CreateTruckViewModel? createTruckViewModel = await _truckService.GetAll()
                                                                                .AsNoTracking()
                                                                                .Where(o => o.TruckID.Equals(truckId))
                                                                                .Select(o => new CreateTruckViewModel
                                                                                {
                                                                                    DriverID = o.DriverID,
                                                                                    LicensePlate = o.LicensePlate,
                                                                                    Capacity = o.Capacity,
                                                                                })
                                                                                .SingleOrDefaultAsync();

                if (createTruckViewModel == null)
                {
                    _logger.LogWarning("Edit GET: Truck with ID '{TruckId}' not found.", truckId);
                    TempData["Error"] = "Truck not found.";
                    return RedirectToAction(nameof(Index));
                }

                createTruckViewModel.Drivers = await GetUsersInRoleAsync(UserRoles.Driver.ToString());
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
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid truckId))
            {
                _logger.LogWarning("Edit POST: Invalid or missing Truck ID '{TruckId}'", id);
                ModelState.AddModelError(string.Empty, "Invalid or missing truck ID.");
                createTruckViewModel.Drivers = await GetUsersInRoleAsync(UserRoles.Driver.ToString());
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
                    return RedirectToAction(nameof(Detail), "Truck", new { id = id });
                }
                else
                {
                    _logger.LogWarning("Edit POST: UpdateTruckAsync failed for Truck ID '{TruckId}'.", truckId);
                    ModelState.AddModelError(string.Empty, "Failed to update truck. It may no longer exist.");
                    createTruckViewModel.Drivers = await GetUsersInRoleAsync(UserRoles.Driver.ToString());
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
                DetailsTruckViewModel? truckDetail = await _truckService.GetAll()
                                                                        .AsNoTracking()
                                                                        .Include(o => o.Driver)
                                                                        .Where(o => o.TruckID.Equals(truckID))
                                                                        .Select(o => new DetailsTruckViewModel
                                                                        {
                                                                            TruckID = o.TruckID,
                                                                            DriverName = o.Driver!.UserName!,
                                                                            LicensePlate = o.LicensePlate,
                                                                            Capacity = o.Capacity,
                                                                            Status = o.Status.ToString(),
                                                                        })
                                                                        .SingleOrDefaultAsync();

                if (truckDetail == null)
                {
                    _logger.LogWarning("Detail GET: Truck with ID '{TruckId}' not found.", truckID);
                    TempData["Error"] = "Truck not found.";
                    return RedirectToAction(nameof(Index));
                }

                truckDetail.TruckOrders = await _truckOrderService.GetAll()
                                                                    .AsNoTracking()
                                                                    .Include(co => co.TruckCourse)
                                                                    .Where(to => to.TruckCourse.TruckID.Equals(truckID))
                                                                    .Select(to => new TruckOrderVewModel
                                                                    {
                                                                        OrderId = to.OrderID,
                                                                        DeliverAddress = to.TruckCourse.DeliverAddress,
                                                                        AssignedDate = to.TruckCourse.AssignedDate,
                                                                        DeliveryDate = to.TruckCourse.DeliveryDate,
                                                                        Status = to.TruckCourse.Status.ToString()
                                                                    })
                                                                    .OrderByDescending(to => to.AssignedDate)
                                                                    .ToListAsync();

                return View(truckDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving truck details for ID '{TruckId}'.", truckID);
                TempData["Error"] = "An unexpected error occurred. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> AssignOrders(string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid truckID))
            {
                _logger.LogWarning("AssignOrders GET: Invalid or missing Truck ID '{TruckId}'", id);
                TempData["Error"] = "Invalid or missing truck ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                AssignOrdersToTruckViewModel? truck = await _truckService.GetAll()
                                                                        .AsNoTracking()
                                                                        .Include(t => t.Driver)
                                                                        .Where(t => t.TruckID.Equals(truckID))
                                                                        .Select(t => new AssignOrdersToTruckViewModel
                                                                        {
                                                                            LicensePlate = t.LicensePlate,
                                                                            Capacity = t.Capacity,
                                                                            DriverName = t.Driver!.UserName!
                                                                        })
                                                                        .SingleOrDefaultAsync();

                if (truck == null)
                {
                    _logger.LogWarning("AssignOrders GET: Truck with ID '{TruckId}' not found.", truckID);
                    TempData["Error"] = "Truck not found.";
                    return RedirectToAction(nameof(Index));
                }

                truck.Orders = await _orderService.GetAll()
                                                  .AsNoTracking()
                                                  .Where(o => new[] { OrderStatus.Pending, OrderStatus.Delayed, OrderStatus.OnHold }.Contains(o.Status) && o.LoadCapacity <= truck.Capacity)
                                                  .Select(o => new OrderViewModel
                                                  {
                                                      OrderID = o.OrderID,
                                                      DeliveryAddress = o.DeliveryAddress,
                                                      PickupAddress = o.PickupAddress,
                                                      OrderStatus = o.Status.ToString(),
                                                      LoadCapacity = o.LoadCapacity,
                                                  }).ToListAsync();

                double loadedCapacity = truck.Orders.Sum(o => o.LoadCapacity);

                truck.LoadedCapacity = loadedCapacity;

                if (truck.Orders == null || !truck.Orders.Any())
                {
                    _logger.LogInformation("No available orders found for truck with ID '{TruckId}'.", truckID);
                }

                return View(truck);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while preparing to assign orders for Truck ID '{TruckId}'.", truckID);
                TempData["Error"] = "An unexpected error occurred. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignOrders(AssignOrdersToTruckViewModel assignOrders, string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid truckID))
            {
                _logger.LogWarning("AssignOrders POST: Invalid or missing Truck ID '{TruckId}'.", id);
                TempData["Error"] = "Invalid or missing truck ID.";
                return RedirectToAction(nameof(Index));
            }

            if (assignOrders.Orders == null || !assignOrders.Orders.Any(o => o.IsSelected))
            {
                TempData["Error"] = "No orders were selected for assignment.";
                return RedirectToAction(nameof(AssignOrders), new { id = id });
            }

            try
            {
                //bool success = await _truckOrderService.AssignOrdersToTruckAsync(assignOrders.Orders.Where(o => o.IsSelected), truckID) > 0;
                bool success = false; // Placeholder for actual implementation
                if (success)
                {
                    TempData["Success"] = "Orders successfully assigned to the truck.";
                    _logger.LogInformation("Orders assigned to Truck ID: {TruckId}", truckID);
                }
                else
                {
                    TempData["Error"] = "Failed to assign orders to the truck. Please try again.";
                    _logger.LogError("Failed to assign orders to Truck ID: {TruckId}", truckID);
                }

                return RedirectToAction(nameof(Detail), "Truck", new { id = truckID });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while assigning orders to Truck ID: {TruckId}.", truckID);
                TempData["Error"] = "An unexpected error occurred while assigning orders. Please try again.";
                return RedirectToAction(nameof(Detail), "Truck", new { id = id });
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveOrder(string? truckId, string? orderId)
        {
            if (string.IsNullOrEmpty(truckId) || !Guid.TryParse(truckId, out Guid truckID))
            {
                _logger.LogWarning("RemoveOrder POST: Invalid or missing Truck ID '{TruckId}'", truckId);
                TempData["Error"] = "Invalid or missing truck ID.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrEmpty(orderId) || !Guid.TryParse(orderId, out Guid orderID))
            {
                _logger.LogWarning("RemoveOrder POST: Invalid or missing Order ID '{OrderId}'", orderId);
                TempData["Error"] = "Invalid or missing order ID.";
                return RedirectToAction(nameof(Detail), new { id = truckID });
            }

            try
            {
                //bool success = await _truckOrderService.RemoveOrderFromTruckAsync(truckID, orderID);
                bool success = false; // Placeholder for actual implementation
                if (success)
                {
                    TempData["Success"] = "Order successfully removed from the truck.";
                    _logger.LogInformation("Order ID '{OrderId}' removed from Truck ID '{TruckId}'", orderID, truckID);
                }
                else
                {
                    TempData["Error"] = "Failed to remove the order. It may not be assigned to this truck.";
                    _logger.LogWarning("RemoveOrderFromTruckAsync failed for Truck ID '{TruckId}' and Order ID '{OrderId}'", truckID, orderID);
                }

                return RedirectToAction(nameof(Detail), "Truck", new { id = truckID });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing order ID '{OrderId}' from Truck ID '{TruckId}'.", orderID, truckID);
                TempData["Error"] = "An unexpected error occurred. Please try again.";
                return RedirectToAction(nameof(Detail), "Truck", new { id = truckID });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatusToCompleted(string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid orderID))
            {
                _logger.LogWarning("ChangeStatusToCompleted POST: Invalid or missing Order ID '{OrderId}'", id);
                TempData["Error"] = "Invalid or missing order ID.";
                return RedirectToAction(nameof(Index), "Order");
            }

            try
            {
                bool success = await _orderService.ChangeStatusToCompletedAsync(orderID);

                if (success)
                {
                    TempData["Success"] = "Order status successfully changed to Completed.";
                    _logger.LogInformation("Order ID '{OrderId}' status changed to completed.", orderID);
                }
                else
                {
                    TempData["Error"] = "Failed to change order status. The order may not exist or its status is not valid for this action.";
                    _logger.LogWarning("ChangeStatusToCompletedAsync failed for Order ID '{OrderId}'", orderID);
                }

                return RedirectToAction(nameof(Index), "Truck");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while changing the status of order ID '{OrderId}' to completed.", orderID);
                TempData["Error"] = "An unexpected error occurred while updating order status. Please try again.";
                return RedirectToAction(nameof(Index), "Truck");
            }
        }

        private async Task<Dictionary<Guid, string?>> GetUsersInRoleAsync(string roleName)
        {
            try
            {
                var users = await _userManager.GetUsersInRoleAsync(roleName);
                return users.ToDictionary(user => user.Id, user => user.UserName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve users in role '{RoleName}'.", roleName);
                return new Dictionary<Guid, string?>();
            }
        }
    }
}