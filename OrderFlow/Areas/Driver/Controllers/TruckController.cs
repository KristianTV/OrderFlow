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
                var trucks = (await _truckService.GetTrucksAsync(driverId, new TruckQueryModel { Status = status })).ToList();


                if (trucks == null || !trucks.Any())
                {
                    _logger.LogInformation("No trucks found for driver with ID: {DriverId}", driverId);
                    ModelState.AddModelError("Trucks", "No trucks found for this driver.");
                    return View(new List<IndexTruckViewModel>());
                }

                ViewData["CurrentStatus"] = string.IsNullOrWhiteSpace(status) || status.Equals("all", StringComparison.OrdinalIgnoreCase)
                    ? "All"
                    : status;

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
                DetailsTruckViewModel? truckDetail = await _truckService.GetTruckDetailsAsync(truckID, driverId);

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
