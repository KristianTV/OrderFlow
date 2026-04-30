using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Hubs;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Order;

namespace OrderFlow.Controllers
{
    public class OrderController : BaseController
    {

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService _orderService;
        private readonly IHubContext<OrderHub> _hubContext;

        public OrderController(ILogger<OrderController> logger,
                               IOrderService orderService,
                               UserManager<ApplicationUser> userManager,
                               IHubContext<OrderHub> hubContext)
        {
            _logger = logger;
            _orderService = orderService;
            _hubContext = hubContext;
            _userManager = userManager;
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
                IEnumerable<IndexOrderViewModel> indexOrders = await _orderService.GetUserOrdersAsync(userId, new OrderQueryModel
                {
                    HideCompleted = hideCompleted.GetValueOrDefault(),
                    SearchId = searchId,
                    StatusFilter = statusFilter,
                    SortOrder = sortOrder
                });

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
            return View(new CreateOrderViewModel());
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

            IEnumerable<string> staff = await GetUsersByRolesAsync(UserRoles.Admin.ToString(), UserRoles.Speditor.ToString());

            await _hubContext.Clients.Users(staff).SendAsync("ReceiveOrderUpdate",
            new IndexOrderViewModel
            {
                OrderID = Guid.NewGuid(),
                OrderDate = DateTime.UtcNow,
                DeliveryAddress = createOrderViewModel.DeliveryAddress,
                PickupAddress = createOrderViewModel.PickupAddress,
                Status = OrderStatus.Pending.ToString(),
                isCanceled = false
            });
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
                CreateOrderViewModel? createOrderViewModel = await _orderService.GetOrderForEditAsync(orderId, userId);

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
                DetailsOrderViewModel? order = await _orderService.GetOrderDetailsAsync(orderId, userId);

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
        protected async Task<IEnumerable<string>> GetUsersByRolesAsync(params string[] roles)
        {
            var userIds = new List<string>();
            foreach (var role in roles)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role);
                userIds.AddRange(usersInRole.Select(u => u.Id.ToString()));
            }
            return userIds.Distinct();
        }
    }
}
