using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Order;

namespace OrderFlow.Controllers
{
    public class OrderController : BaseController
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService _orderService;

        public OrderController(ILogger<OrderController> logger, IOrderService orderService)
        {
            _logger = logger;
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(bool? hideCompleted, string? searchId = null, string? statusFilter = null, string? sortOrder = null)
        {
            if (!Guid.TryParse(this.GetUserId(), out Guid userId))
            {
                _logger.LogWarning("Invalid user ID format received in Index action.");
                return BadRequest();
            }

            try
            {
                var orders = _orderService.GetAll().AsNoTracking();

                if (!string.IsNullOrEmpty(searchId))
                {
                    orders = orders.Where(o => o.OrderID.ToString().Contains(searchId));
                }

                if (!string.IsNullOrEmpty(statusFilter))
                {
                    if (!Enum.TryParse(statusFilter, true, out OrderStatus orderStatus))
                    {
                        _logger.LogWarning("Invalid status filter '{StatusFilter}' received in Index action.", statusFilter);
                        return BadRequest();
                    }
                    orders = orders.Where(o => o.Status.Equals(orderStatus));
                }

                if (hideCompleted.GetValueOrDefault())
                {
                    orders = orders.Where(o => o.Status != OrderStatus.Completed);
                }

                switch (sortOrder)
                {
                    case "date_desc":
                        orders = orders.OrderByDescending(o => o.OrderDate);
                        break;
                    case "date_asc":
                        orders = orders.OrderBy(o => o.OrderDate);
                        break;
                    default:
                        orders = orders.OrderBy(o => o.OrderDate);
                        break;
                }

                IEnumerable<IndexOrderViewModel> indexOrders = await orders.Where(o => o.UserID.Equals(userId))
                                                                           .Select(order => new IndexOrderViewModel
                                                                           {
                                                                               OrderID = order.OrderID,
                                                                               OrderDate = order.OrderDate,
                                                                               DeliveryAddress = order.DeliveryAddress,
                                                                               PickupAddress = order.PickupAddress,
                                                                               Status = order.Status.ToString(),
                                                                               isCanceled = order.isCanceled
                                                                           }).ToListAsync();

                return View(indexOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving orders for user ID: {UserId}.", userId);
                return BadRequest();
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderViewModel createOrderViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(createOrderViewModel);
            }

            if (!Guid.TryParse(this.GetUserId(), out Guid userId))
            {
                _logger.LogWarning("Invalid user ID format received during order creation.");
                return BadRequest();
            }

            try
            {
                if (!await _orderService.CreateOrderAsync(createOrderViewModel, userId))
                {
                    ModelState.AddModelError(string.Empty, "Failed to create the order. Please try again.");
                    return View(createOrderViewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating an order for user ID: {UserId}.", userId);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                return View(createOrderViewModel);
            }

            return RedirectToAction(nameof(Index), "Order");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid orderId))
            {
                return BadRequest();
            }

            if (!Guid.TryParse(this.GetUserId(), out Guid userId))
            {
                _logger.LogWarning("Invalid user ID format received during Edit action for order ID: {OrderId}.", orderId);
                return BadRequest();
            }

            try
            {
                CreateOrderViewModel? createOrderViewModel = await _orderService.GetAll()
                                                                                .AsNoTracking()
                                                                                .Where(o => o.OrderID.Equals(orderId) && o.UserID.Equals(userId))
                                                                                .Select(o => new CreateOrderViewModel
                                                                                {
                                                                                    DeliveryAddress = o.DeliveryAddress,
                                                                                    PickupAddress = o.PickupAddress,
                                                                                    DeliveryInstructions = o.DeliveryInstructions,
                                                                                    LoadCapacity = o.LoadCapacity,
                                                                                }).SingleOrDefaultAsync();

                if (createOrderViewModel == null)
                {
                    _logger.LogInformation("Order with ID {OrderId} not found or user ID {UserId} does not match.", orderId, userId);
                    return NotFound();
                }

                return View(createOrderViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving order {OrderId} for user ID {UserId}.", orderId, userId);
                return BadRequest();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateOrderViewModel createOrderViewModel, string? id)
        {
            if (!ModelState.IsValid)
            {
                return View(createOrderViewModel);
            }

            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid orderId))
            {
                _logger.LogWarning("Invalid order ID format received during order update.");
                return BadRequest();
            }

            if (!Guid.TryParse(this.GetUserId(), out Guid userId))
            {
                _logger.LogWarning("Invalid user ID format received during order update for order ID: {OrderId}.", orderId);
                return BadRequest();
            }

            try
            {
                if (!await _orderService.UpdateOrderAsync(createOrderViewModel, orderId, userId))
                {
                    _logger.LogInformation("Update failed for order {OrderId}. The order may not exist or the user ID {UserId} does not match.", orderId, userId);
                    ModelState.AddModelError(string.Empty, "Failed to update the order. The order may not exist or you do not have permission.");
                    return View(createOrderViewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating order {OrderId} for user ID: {UserId}.", orderId, userId);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                return View(createOrderViewModel);
            }

            return RedirectToAction(nameof(Detail), "Order", new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid orderId))
            {
                _logger.LogWarning("Invalid or missing Order ID received during Detail action.");
                return BadRequest();
            }

            if (!Guid.TryParse(this.GetUserId(), out Guid userId))
            {
                _logger.LogWarning("Invalid user ID format received during Detail action for order ID: {OrderId}.", orderId);
                return BadRequest();
            }

            try
            {
                var order = await _orderService.GetAll()
                                               .AsNoTracking()
                                               .Include(o => o.User)
                                               .Include(o => o.Payments)
                                               .Include(o => o.OrderTrucks)
                                               .ThenInclude(to => to.Truck)
                                               .Where(o => o.OrderID.Equals(orderId) && o.UserID.Equals(userId))
                                               .Select(o => new DetailsOrderViewModel
                                               {
                                                   OrderID = o.OrderID,
                                                   UserName = o.User!.UserName!,
                                                   OrderDate = o.OrderDate,
                                                   DeliveryDate = o.DeliveryDate,
                                                   DeliveryAddress = o.DeliveryAddress,
                                                   PickupAddress = o.PickupAddress,
                                                   LoadCapacity = o.LoadCapacity,
                                                   DeliveryInstructions = o.DeliveryInstructions,
                                                   Status = o.Status.ToString(),
                                                   isCanceled = o.isCanceled,
                                                   TrucksLicensePlates = o.OrderTrucks.Select(to => to.Truck.LicensePlate).ToList(),
                                                   Payments = o.Payments.Select(payment => new PaymentViewModel
                                                   {
                                                       Id = payment.Id,
                                                       PaymentDate = payment.PaymentDate,
                                                       Amount = payment.Amount,
                                                       PaymentDescription = payment.PaymentDescription
                                                   }).ToList(),
                                                   TotalPrice = o.Payments.Sum(p => p.Amount)
                                               }).SingleOrDefaultAsync();

                if (order == null)
                {
                    _logger.LogInformation("Order with ID {OrderId} not found or user ID {UserId} does not match.", orderId, userId);
                    return NotFound();
                }

                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving order details for order ID: {OrderId} and user ID: {UserId}.", orderId, userId);
                return BadRequest();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid orderId))
            {
                _logger.LogWarning(id == null ? "Order ID is null or empty." : "Invalid Order ID format received during cancel action.");
                return BadRequest();
            }

            if (!Guid.TryParse(this.GetUserId(), out Guid userId))
            {
                _logger.LogWarning("Invalid user ID format received during cancel action for order ID: {OrderId}.", orderId);
                return BadRequest();
            }

            try
            {
                if (!await _orderService.CancelOrderAsync(orderId, userId))
                {
                    _logger.LogInformation("Cancel failed for order {OrderId}. The order may not exist or the user ID {UserId} does not match.", orderId, userId);
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while canceling order {OrderId} for user ID: {UserId}.", orderId, userId);
                return BadRequest();
            }

            return RedirectToAction(nameof(Index), "Order");
        }
    }
}