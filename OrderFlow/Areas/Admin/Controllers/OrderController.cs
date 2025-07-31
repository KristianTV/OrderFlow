using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Models;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Order;

namespace OrderFlow.Areas.Admin.Controllers
{
    public class OrderController : BaseAdminController
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService _orderService;

        public OrderController(ILogger<OrderController> logger, IOrderService orderService)
        {
            _logger = logger;
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!Guid.TryParse(this.GetUserId(), out Guid userId))
            {
                return BadRequest("Invalid User ID format.");
            }

            var orders = await _orderService.All<Order>()
                                            .Where(o => o.UserID.Equals(userId))
                                            .AsNoTracking()
                                            .Select(order => new IndexOrderViewModel
                                            {
                                                OrderID = order.OrderID,
                                                OrderDate = order.OrderDate,
                                                DeliveryAddress = order.DeliveryAddress,
                                                PickupAddress = order.PickupAddress,
                                                Status = order.Status.ToString(),
                                                isCanceled = order.isCanceled
                                            }).ToListAsync();

            return View(orders);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderViewModel createOrderViewModel)
        {
            if (!ModelState.IsValid)
                return View(createOrderViewModel);

            if (!Guid.TryParse(this.GetUserId(), out Guid userId))
            {
                return BadRequest("Invalid User ID format.");
            }

            if (!await _orderService.CreateOrderAsync(createOrderViewModel, userId))
                return View(createOrderViewModel);

            return RedirectToAction(nameof(Index), "Order");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!Guid.TryParse(id, out Guid orderId))
            {
                return BadRequest("Invalid Order ID format.");
            }

            if (!Guid.TryParse(this.GetUserId(), out Guid userId))
            {
                return BadRequest("Invalid User ID format.");
            }

            CreateOrderViewModel? createOrderViewModel = await _orderService.All<Order>()
                                                                     .Where(o => o.OrderID.Equals(orderId) &&
                                                                                 o.UserID.Equals(userId))
                                                                     .Select(o => new CreateOrderViewModel
                                                                     {
                                                                         DeliveryAddress = o.DeliveryAddress,
                                                                         PickupAddress = o.PickupAddress,
                                                                         DeliveryInstructions = o.DeliveryInstructions
                                                                     }).SingleOrDefaultAsync();

            if (createOrderViewModel == null)
            {
                return NotFound();
            }

            return View(createOrderViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CreateOrderViewModel createOrderViewModel, string? id)
        {
            if (!ModelState.IsValid)
                return View(createOrderViewModel);

            if (id == null)
            {
                return NotFound();
            }

            if (!await _orderService.UpdateOrderAsync(createOrderViewModel, Guid.Parse(id), Guid.Parse(this.GetUserId())))
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Detail), "Order", new { id = id });
        }

        [HttpGet]
        public IActionResult Detail(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            if (!Guid.TryParse(id, out Guid orderId))
            {
                return BadRequest("Invalid Order ID format.");
            }

            if (!Guid.TryParse(this.GetUserId(), out Guid userId))
            {
                return BadRequest("Invalid User ID format.");
            }

            var order = _orderService.All<Order>()
                                     .AsNoTracking()
                                     .Include(o => o.User)
                                     .Include(o => o.Payments)
                                     .Include(o => o.TruckOrder)
                                     .Include(o => o.TruckOrder!.Truck)
                                     .Where(o => o.OrderID.Equals(Guid.Parse(id)) && o.UserID.Equals(userId))
                                     .Select(o => new DetailsOrderViewModel
                                     {
                                         OrderID = o.OrderID,
                                         UserName = o.User!.UserName!,
                                         OrderDate = o.OrderDate,
                                         DeliveryDate = o.DeliveryDate,
                                         DeliveryAddress = o.DeliveryAddress,
                                         PickupAddress = o.PickupAddress,
                                         DeliveryInstructions = o.DeliveryInstructions,
                                         Status = o.Status.ToString(),
                                         isCanceled = o.isCanceled,
                                         TruckLicensePlate = o.TruckOrder!.Truck.LicensePlate,
                                         Payments = o.Payments.ToList(),
                                         TotalPrice = o.Payments.ToList().Sum(p => p.Amount)
                                     }).SingleOrDefault();

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!Guid.TryParse(id, out Guid orderId))
            {
                return BadRequest("Invalid Order ID format.");
            }

            if (!Guid.TryParse(this.GetUserId(), out Guid userId))
            {
                return BadRequest("Invalid User ID format.");
            }

            if (!await _orderService.CancelOrderAsync(orderId, userId))
            {
                return BadRequest("Failed to cancel the order.");
            }

            return RedirectToAction(nameof(Index), "Order");
        }
    }
}
