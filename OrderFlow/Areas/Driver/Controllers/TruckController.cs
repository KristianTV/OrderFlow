using Microsoft.AspNetCore.Mvc;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Truck;

namespace OrderFlow.Areas.Driver.Controllers
{
    public class TruckController : BaseDriverController
    {
        private const int IndexPageSize = 12;
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
        public async Task<IActionResult> Index(string? status = null, string? search = null, string? sortOrder = null, int page = 1)
        {
            if (!Guid.TryParse(this.GetUserId(), out Guid driverId))
            {
                _logger.LogWarning("Invalid Driver ID format from session.");
                ModelState.AddModelError("DriverId", "Invalid Driver ID format.");
                return BadRequest();
            }

            try
            {
                List<IndexTruckViewModel> indexTrucks = (await _truckService.GetTrucksAsync(driverId, new TruckQueryModel
                {
                    Search = search,
                    Status = status,
                    SortOrder = sortOrder,
                    Page = page,
                    PageSize = IndexPageSize + 1
                })).ToList();

                bool hasMore = indexTrucks.Count > IndexPageSize;
                HttpContext?.Response?.Headers.TryAdd("X-Has-More", hasMore.ToString().ToLowerInvariant());
                indexTrucks = indexTrucks.Take(IndexPageSize).ToList();

                ViewData["CurrentStatus"] = string.IsNullOrWhiteSpace(status) || status.Equals("all", StringComparison.OrdinalIgnoreCase)
                    ? "All"
                    : status;
                ViewData["CurrentSearch"] = search;
                ViewData["CurrentSortOrder"] = sortOrder;

                if (IsAjaxRequest())
                {
                    ViewData["TruckArea"] = "Driver";
                    return PartialView("~/Views/Shared/_TruckCards.cshtml", indexTrucks);
                }

                ViewBag.HasMore = hasMore;

                return View(indexTrucks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving trucks for driver {DriverId}.", driverId);
                return BadRequest();
            }
        }

        private bool IsAjaxRequest()
        {
            return HttpContext?.Request?.Headers.XRequestedWith.ToString() == "XMLHttpRequest";
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
