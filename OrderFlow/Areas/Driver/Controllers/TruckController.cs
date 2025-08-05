using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Truck;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Detail(string? id)
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

            DetailsTruckViewModel? truckDetail = await _truckService.GetAll()
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
                                                                        Status = o.Status.ToString()
                                                                    }).SingleOrDefaultAsync();

            if(truckDetail == null)
            {
                return NotFound();
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

            var orders = await _truckOrderService.GetAll()
                                                 .AsNoTracking()
                                                 .Where(to => to.Status.Equals(TruckOrderStatus.Assigned))
                                                 .Select(to => new TruckOrderVewModel
                                                 {
                                                     OrderId = to.OrderID,
                                                     DeliverAddress = to.DeliverAddress,
                                                     AssignedDate = to.AssignedDate,
                                                     Status=to.Status.ToString(),
                                                 })
                                                 .ToListAsync();

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
