using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Truck;

namespace OrderFlow.Areas.Driver.Controllers
{
    public class TruckController : BaseDriverController
    {
        private readonly ILogger<TruckController> _logger;
        private readonly ITruckService _truckService;
        private readonly IOrderService _orderService;
        private readonly ITruckOrderService _truckOrderService;

        public TruckController(ILogger<TruckController> logger, ITruckService truckService, IOrderService orderService,ITruckOrderService truckOrderService)
        {
            _logger = logger;
            _truckService = truckService;
            _orderService = orderService;
            _truckOrderService = truckOrderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (Guid.TryParse(this.GetUserId(), out Guid driverId) == false)
            {
                return BadRequest("Invalid Driver ID format.");
            }

            var orders = await _truckService.GetAll()
                                            .AsNoTracking()
                                            .Include(t => t.Driver)
                                            .Where(t => t.DriverID == driverId)
                                            .Select(truck => new IndexTruckViewModel
                                            {
                                                TruckID = truck.TruckID,
                                                DriverName = truck.Driver!.UserName!,
                                                LicensePlate = truck.LicensePlate,
                                                Capacity = truck.Capacity,
                                                Status = truck.Status.ToString()
                                            })
                                            .ToListAsync();

            return View(orders);
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
                return BadRequest("Invalid Truck ID format.");
            }

            if (!Guid.TryParse(this.GetUserId(), out Guid driverId))
            {
                return BadRequest("Invalid Driver ID format.");
            }

            var order = _truckService.GetAll()
                                     .AsNoTracking()
                                     .Include(o => o.Driver)
                                     .Include(to => to.TruckOrders)
                                     .ThenInclude(to => to.Order)
                                     .Where(o => o.TruckID.Equals(truckID) && o.DriverID.Equals(driverId))
                                     .Select(o => new DetailsTruckViewModel
                                     {
                                         TruckID = o.TruckID,
                                         DriverName = o.Driver!.UserName!,
                                         LicensePlate = o.LicensePlate,
                                         Capacity = o.Capacity,
                                         Status = o.Status.ToString(),
                                         TruckOrders = o.TruckOrders
                                     }).SingleOrDefault();

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
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

            if (!Guid.TryParse(this.GetUserId(), out Guid driverId))
            {
                return BadRequest("Invalid Driver ID format.");
            }

            var orders = await _orderService.GetAll()
                                            .AsNoTracking()
                                            .Include(o => o.OrderTrucks)
                                            .ThenInclude(to => to.Truck)
                                            .Where(o => o.OrderTrucks != null &&
                                                        o.OrderTrucks.Any(to => to.TruckID.Equals(truckID) && 
                                                                                to.Truck!.DriverID.Equals(driverId)&&
                                                                                to.Status.Equals(TruckOrderStatus.Assigned))
                                                        )
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

            await _orderService.CompleteOrderAsync(orderID, _truckOrderService,_truckService);

            return RedirectToAction(nameof(Index), "Truck");
        }
    }
}
