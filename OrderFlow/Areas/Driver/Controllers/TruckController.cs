using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public TruckController(ILogger<TruckController> logger,
                                ITruckService truckService,
                                IOrderService orderService,
                                ITruckOrderService truckOrderService)
        {
            _logger = logger;
            _truckService = truckService;
            _orderService = orderService;
            _truckOrderService = truckOrderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!Guid.TryParse(this.GetUserId(), out Guid driverId))
            {
                _logger.LogWarning("Invalid Driver ID format from session.");
                ModelState.AddModelError("DriverId", "Invalid Driver ID format.");
                return BadRequest();
            }

            try
            {
                var trucks = await _truckService.GetAll()
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

                if (trucks == null || !trucks.Any())
                {
                    _logger.LogInformation("No trucks found for driver with ID: {DriverId}", driverId);
                    ModelState.AddModelError("Trucks", "No trucks found for this driver.");
                    return View(new List<IndexTruckViewModel>());
                }

                return View(trucks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving trucks for driver {DriverId}.", driverId);
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid truckID) || !Guid.TryParse(this.GetUserId(), out Guid driverId))
            {
                _logger.LogWarning("Invalid ID or Driver ID format. Truck ID: {TruckID}, Driver ID: {DriverId}", id, this.GetUserId());
                return BadRequest();
            }

            try
            {
                var truckDetail = await _truckService.GetAll()
                                                     .AsNoTracking()
                                                     .Include(o => o.Driver)
                                                     .Where(o => o.TruckID.Equals(truckID) && o.DriverID.Equals(driverId))
                                                     .Select(o => new DetailsTruckViewModel
                                                     {
                                                         TruckID = o.TruckID,
                                                         DriverName = o.Driver!.UserName!,
                                                         LicensePlate = o.LicensePlate,
                                                         Capacity = o.Capacity,
                                                         Status = o.Status.ToString(),
                                                         TruckOrders = new List<TruckOrderVewModel>()
                                                     })
                                                     .SingleOrDefaultAsync();

                if (truckDetail == null)
                {
                    _logger.LogWarning("Truck with ID {TruckID} not found or does not belong to driver {DriverId}.", truckID, driverId);
                    return NotFound("Truck not found or does not belong to the driver.");
                }

                truckDetail.TruckOrders = await _truckOrderService.GetAll()
                                                                  .AsNoTracking()
                                                                  .Where(to => to.TruckID.Equals(truckID))
                                                                  .Select(to => new TruckOrderVewModel
                                                                  {
                                                                      OrderId = to.OrderID,
                                                                      DeliverAddress = to.DeliverAddress,
                                                                      AssignedDate = to.AssignedDate,
                                                                      DeliveryDate = to.DeliveryDate,
                                                                      Status = to.Status.ToString()
                                                                  })
                                                                  .OrderByDescending(to => to.AssignedDate)
                                                                  .ToListAsync();

                return View(truckDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving details for truck {TruckID} for driver {DriverId}.", truckID, driverId);
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> AssignedOrders(string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid truckID) || !Guid.TryParse(this.GetUserId(), out Guid driverId))
            {
                _logger.LogWarning("Invalid Truck ID or Driver ID format. Truck ID: {TruckID}, Driver ID: {DriverId}", id, this.GetUserId());
                return BadRequest();
            }

            try
            {
                var truckExists = await _truckService.GetAll()
                                                     .AnyAsync(t => t.TruckID.Equals(truckID) && t.DriverID.Equals(driverId));

                if (!truckExists)
                {
                    _logger.LogWarning("Attempt to access assigned orders for a truck ({TruckID}) that does not belong to driver {DriverId}.", truckID, driverId);
                    return NotFound("Truck not found or does not belong to the user.");
                }

                var orders = await _truckOrderService.GetAll()
                                                     .AsNoTracking()
                                                     .Where(to => to.TruckID.Equals(truckID) && to.Status.Equals(TruckOrderStatus.Assigned))
                                                     .Select(to => new TruckOrderVewModel
                                                     {
                                                         OrderId = to.OrderID,
                                                         DeliverAddress = to.DeliverAddress,
                                                         AssignedDate = to.AssignedDate,
                                                         Status = to.Status.ToString(),
                                                         DeliveryDate = to.DeliveryDate,
                                                     })
                                                     .ToListAsync();

                if (orders == null)
                {
                    orders = new List<TruckOrderVewModel>();
                }

                return View(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving assigned orders for truck {TruckID} for driver {DriverId}.", truckID, driverId);
                return BadRequest();
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> ChangeStatusToCompleted(string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid orderID) || !Guid.TryParse(this.GetUserId(), out Guid driverId))
            {
                _logger.LogWarning("Invalid Order ID or Driver ID format. Order ID: {OrderID}, Driver ID: {DriverId}", id, this.GetUserId());
                ModelState.AddModelError("OrderId", "Invalid Order ID format.");
                return BadRequest("Invalid request parameters.");
            }

            try
            {
                var orderAndTruckExist = await _truckOrderService.GetAll()
                    .AnyAsync(to => to.OrderID.Equals(orderID) && to.Truck.DriverID.Equals(driverId));

                if (!orderAndTruckExist)
                {
                    _logger.LogWarning("Attempt to complete an order ({OrderID}) not assigned to driver {DriverId}.", orderID, driverId);
                    ModelState.AddModelError("OrderId", "Order not found or not assigned to your truck.");
                    return NotFound("Order not found or not assigned to your truck.");
                }

                await _orderService.CompleteOrderAsync(orderID, _truckOrderService, _truckService);

                return RedirectToAction(nameof(Index), "Truck");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while completing order {OrderID} for driver {DriverId}.", orderID, driverId);
                ModelState.AddModelError("OrderId", "An error occurred while completing the order.");
                return BadRequest();
            }
        }
    }
}