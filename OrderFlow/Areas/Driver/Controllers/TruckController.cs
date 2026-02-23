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
        private readonly ICourseOrderService _truckOrderService;

        public TruckController(ILogger<TruckController> logger,
                                ITruckService truckService,
                                IOrderService orderService,
                                ICourseOrderService truckOrderService)
        {
            _logger = logger;
            _truckService = truckService;
            _orderService = orderService;
            _truckOrderService = truckOrderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? status = null)
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
                                                         Status = o.Status.ToString()
                                                     })
                                                     .SingleOrDefaultAsync();

                if (truckDetail == null)
                {
                    _logger.LogWarning("Truck with ID {TruckID} not found or does not belong to driver {DriverId}.", truckID, driverId);
                    return NotFound("Truck not found or does not belong to the driver.");
                }

                return View(truckDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving details for truck {TruckID} for driver {DriverId}.", truckID, driverId);
                return BadRequest();
            }
        }
    }
}