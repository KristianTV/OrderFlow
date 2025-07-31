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
        private readonly ITruckOrderService _truckOrderService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TruckController(ILogger<TruckController> logger, ITruckService truckService, UserManager<ApplicationUser> userManager, IOrderService orderService, ITruckOrderService truckOrderService)
        {
            _logger = logger;
            _truckService = truckService;
            _userManager = userManager;
            _orderService = orderService;
            _truckOrderService = truckOrderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var orders = await _truckService.All<Truck>()
                                            .AsNoTracking()
                                            .Include(t => t.Driver)
                                            .Select(truck => new IndexTruckViewModel
                                            {
                                                TruckID = truck.TruckID,
                                                DriverName = truck.Driver!.UserName!,
                                                LicensePlate = truck.LicensePlate,
                                                Capacity = truck.Capacity,
                                                Status = truck.Status
                                            })
                                            .ToListAsync();

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var users = await _userManager.GetUsersInRoleAsync("Driver");

            if (users == null || !users.Any())
            {
                ModelState.AddModelError(string.Empty, "No drivers available. Please add a driver first.");
                return View(new CreateTruckViewModel());
            }

            Dictionary<Guid, string> drivers = users.ToDictionary(
                user => user.Id,
                user => user.UserName
            );

            if (drivers == null || !drivers.Any())
            {
                ModelState.AddModelError(string.Empty, "No drivers available. Please add a driver first.");
                return View(new CreateTruckViewModel());
            }

            CreateTruckViewModel createTruckViewModel = new CreateTruckViewModel
            {
                Drivers = drivers
            };

            return View(createTruckViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTruckViewModel createTruckViewModel)
        {
            if (!ModelState.IsValid)
                return View(createTruckViewModel);

            if (!await _truckService.CreateTruckAsync(createTruckViewModel))
                return View(createTruckViewModel);

            return RedirectToAction(nameof(Index), "Truck");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!Guid.TryParse(id, out Guid truckId))
            {
                return BadRequest("Invalid Truck ID format.");
            }

            var users = await _userManager.GetUsersInRoleAsync("Driver");

            if (users == null || !users.Any())
            {
                ModelState.AddModelError(string.Empty, "No drivers available. Please add a driver first.");
                return View(new CreateTruckViewModel());
            }

            Dictionary<Guid, string> drivers = users.ToDictionary(
                user => user.Id,
                user => user.UserName
            );

            if (drivers == null || !drivers.Any())
            {
                ModelState.AddModelError(string.Empty, "No drivers available. Please add a driver first.");
                return View(new CreateTruckViewModel());
            }


            CreateTruckViewModel? createTruckViewModel = await _truckService.All<Truck>()
                                                                            .Where(o => o.TruckID.Equals(truckId))
                                                                            .Select(o => new CreateTruckViewModel
                                                                            {
                                                                                DriverID = o.DriverID,
                                                                                LicensePlate = o.LicensePlate,
                                                                                Capacity = o.Capacity,
                                                                                Drivers = drivers
                                                                            }).SingleOrDefaultAsync();

            if (createTruckViewModel == null)
            {
                return NotFound();
            }

            return View(createTruckViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CreateTruckViewModel createTruckViewModel, string? id)
        {
            if (!ModelState.IsValid)
                return View(createTruckViewModel);

            if (id == null)
            {
                return NotFound();
            }

            if (!await _truckService.UpdateTruckAsync(createTruckViewModel, Guid.Parse(id)))
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Detail), "Truck", new { id = id });
        }

        [HttpGet]
        public IActionResult Detail(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            if (!Guid.TryParse(id, out Guid truckID))
            {
                return BadRequest("Invalid Order ID format.");
            }

            var order = _truckService.All<Truck>()
                                     .AsNoTracking()
                                     .Include(o => o.Driver)
                                     .Include(to => to.TruckOrders)
                                     .ThenInclude(to => to.Order)
                                     .Where(o => o.TruckID.Equals(truckID))
                                     .Select(o => new DetailsTruckViewModel
                                     {
                                         TruckID = o.TruckID,
                                         DriverName = o.Driver!.UserName!,
                                         LicensePlate = o.LicensePlate,
                                         Capacity = o.Capacity,
                                         Status = o.Status,
                                         TruckOrders = o.TruckOrders
                                     }).SingleOrDefault();

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> AssignOrders(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            if (!Guid.TryParse(id, out Guid truckID))
            {
                return BadRequest("Invalid Order ID format.");
            }

            var orders = await _orderService.All<Order>()
                                            .AsNoTracking()
                                            .Where(o => new[] { OrderStatus.Pending,
                                                                OrderStatus.Delayed,
                                                                OrderStatus.OnHold
                                                                }.Contains(o.Status))
                                            .Select(o => new OrderViewModel
                                            {
                                                OrderID = o.OrderID,
                                                DeliveryAddress = o.DeliveryAddress,
                                                PickupAddress = o.PickupAddress,
                                                OrderStatus = o.Status.ToString()
                                            }).ToListAsync();

            if (orders == null)
            {
                return BadRequest("No Order were found.");
            }

            var truck = await _truckService.All<Truck>()
                                           .AsNoTracking()
                                           .Include(t => t.Driver)
                                           .Where(t => t.TruckID.Equals(truckID))
                                           .Select(t => new AssignOrdersToTruckViewModel
                                           {
                                               LicensePlate = t.LicensePlate,
                                               Capacity = t.Capacity,
                                               DriverName = t.Driver!.UserName!,
                                               Orders = orders
                                           }).SingleOrDefaultAsync();

            return View(truck);
        }

        [HttpPost]
        public async Task<IActionResult> AssignOrders(AssignOrdersToTruckViewModel assignOrders, string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            if (!Guid.TryParse(id, out Guid truckID))
            {
                return BadRequest("Invalid Order ID format.");
            }

            if (assignOrders == null || assignOrders.Orders == null || !assignOrders.Orders.Any())
            {
                ModelState.AddModelError(string.Empty, "No orders selected for assignment.");
                return RedirectToAction(nameof(AssignOrders), "Truck", new { id = id });
            }

            await _truckOrderService.AssignOrdersToTruckAsync(assignOrders.Orders.Where(o => o.IsSelected == true), truckID);

            return RedirectToAction(nameof(Detail), "Truck", new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            if (!Guid.TryParse(id, out Guid truckID))
            {
                return BadRequest("Invalid Truck ID format.");
            }

            await _truckService.SoftDeleteTruckAsync(truckID);

            return RedirectToAction(nameof(Index), "Truck");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveOrder(string? truckId, string? orderId)
        {
            if (string.IsNullOrEmpty(truckId) || string.IsNullOrEmpty(orderId))
            {
                return NotFound();
            }

            if (!Guid.TryParse(truckId, out Guid truckID))
            {
                return BadRequest("Invalid Truck ID format.");
            }

            if (!Guid.TryParse(orderId, out Guid orderID))
            {
                return BadRequest("Invalid Order ID format.");
            }

            await _truckOrderService.RemoveOrderFromTruckAsync(truckID, orderID);

            return RedirectToAction(nameof(Detail), "Truck", new { id = truckID });
        }

        [HttpGet]
        public async Task<IActionResult> AssignedOrders(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            if (!Guid.TryParse(id, out Guid truckID))
            {
                return BadRequest("Invalid Order ID format.");
            }

            var orders = await _orderService.All<Order>()
                                            .AsNoTracking()
                                            .Where(o => new[] { OrderStatus.InProgress }.Contains(o.Status))
                                            .Select(o => new AssignedOrdersToTruckViewModel
                                            {
                                                OrderID = o.OrderID,
                                                DeliveryAddress = o.DeliveryAddress,
                                                PickupAddress = o.PickupAddress,
                                                OrderStatus = o.Status.ToString()
                                            }).ToListAsync();

            if (orders == null)
            {
                return BadRequest("No Order were found.");
            }

            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatusToCompleted(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            if (!Guid.TryParse(id, out Guid orderID))
            {
                return BadRequest("Invalid Truck ID format.");
            }

            await _orderService.ChangeStatusToCompletedAsync(orderID);
            return RedirectToAction(nameof(Detail), "Truck", new { id = id });
        }
    }
}
