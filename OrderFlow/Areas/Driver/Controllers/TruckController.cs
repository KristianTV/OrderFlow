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

        public TruckController(ILogger<TruckController> logger, ITruckService truckService, IOrderService orderService)
        {
            _logger = logger;
            _truckService = truckService;
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (Guid.TryParse(this.GetUserId(), out Guid driverId) == false)
            {
                return BadRequest("Invalid Driver ID format.");
            }

            var orders = await _truckService.All<Truck>()
                                            .AsNoTracking()
                                            .Include(t => t.Driver)
                                            .Where(t => t.DriverID == driverId)
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

            var order = _truckService.All<Truck>()
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

            var orders = await _orderService.All<Order>()
                                            .AsNoTracking()
                                            .Include(o => o.TruckOrder)
                                            .ThenInclude(to => to.Truck)
                                            .Where(o => new[] { OrderStatus.InProgress }.Contains(o.Status) &&
                                                        o.TruckOrder != null &&
                                                        o.TruckOrder.TruckID.Equals(truckID) &&
                                                        o.TruckOrder.Truck.DriverID.Equals(driverId)
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

            await _orderService.ChangeStatusToCompletedAsync(orderID);
            return RedirectToAction(nameof(Detail), "Truck", new { id = id });
        }
    }
}


//**
//  Driver can do 
//  see assigned orders 
//  complete assigned orders
//  see completed orders
//  see all trucks that he drives
//  see details of a truck that he drives
//  
//
//
//**//