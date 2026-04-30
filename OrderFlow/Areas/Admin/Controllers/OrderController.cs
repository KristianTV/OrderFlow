using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Order;

namespace OrderFlow.Areas.Admin.Controllers
{
    public class OrderController : BaseAdminController
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(ILogger<OrderController> logger, IOrderService orderService, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _orderService = orderService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(bool? hideCompleted, string? searchId = null, string? statusFilter = null, string? sortOrder = null)
        {
            try
            {
                IEnumerable<IndexOrderViewModel> indexOrders = await _orderService.GetAdminOrdersAsync(new OrderQueryModel
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
                _logger.LogError(ex, "An error occurred while retrieving orders.");
                return BadRequest();
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            AdminCreateOrderViewModel adminCreateOrderViewModel = new AdminCreateOrderViewModel
            {
                Users = GetUsersInRole(UserRoles.User.ToString()),
            };

            return View(adminCreateOrderViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminCreateOrderViewModel createOrderViewModel)
        {
            if (!ModelState.IsValid)
            {
                createOrderViewModel.Users = GetUsersInRole(UserRoles.User.ToString());
                return View(createOrderViewModel);
            }

            try
            {
                if (!await _orderService.CreateOrderAsync(createOrderViewModel, createOrderViewModel.UsersId))
                {
                    ModelState.AddModelError(string.Empty, "Failed to create the order. Please check the provided details and try again.");
                    createOrderViewModel.Users = GetUsersInRole(UserRoles.User.ToString());
                    return View(createOrderViewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new order.");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                createOrderViewModel.Users = GetUsersInRole(UserRoles.User.ToString());
                return View(createOrderViewModel);
            }

            return RedirectToAction(nameof(Index), "Order");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogError(id, "Order ID must be provided.");
                return NotFound();
            }

            if (!Guid.TryParse(id, out Guid orderId))
            {
                _logger.LogError(id, "Invalid Order ID format.");
                return BadRequest();
            }

            AdminCreateOrderViewModel? createOrderViewModel = null;
            try
            {
                createOrderViewModel = await _orderService.GetAdminOrderForEditAsync(orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving order with ID {OrderId} for editing.", orderId);
                return BadRequest();
            }

            if (createOrderViewModel == null)
            {
                _logger.LogError(nameof(createOrderViewModel), $"Order with ID {orderId} was not found.");
                return NotFound();
            }

            createOrderViewModel.Users = GetUsersInRole(UserRoles.User.ToString());

            return View(createOrderViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminCreateOrderViewModel createOrderViewModel, string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogError(id, "Order ID must be provided.");
                ModelState.AddModelError(nameof(id), "Order ID must be provided.");
                return NotFound();
            }

            if (!Guid.TryParse(id, out Guid orderId))
            {
                _logger.LogError(id, "Invalid Order ID format.");
                ModelState.AddModelError(nameof(orderId), "Invalid Order ID format.");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                createOrderViewModel.Users = GetUsersInRole(UserRoles.User.ToString());
                return View(createOrderViewModel);
            }

            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);

                if (order == null)
                {
                    return NotFound($"Order with ID {orderId} was not found.");
                }

                if (!await _orderService.UpdateOrderAsync(createOrderViewModel, orderId, createOrderViewModel.UsersId))
                {
                    ModelState.AddModelError(string.Empty, "Failed to update the order. The order may have been modified by another user.");
                    createOrderViewModel.Users = GetUsersInRole(UserRoles.User.ToString());
                    return View(createOrderViewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating order with ID {OrderId}.", orderId);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                createOrderViewModel.Users = GetUsersInRole(UserRoles.User.ToString());
                return View(createOrderViewModel);
            }

            return RedirectToAction(nameof(Detail), "Order", new { id = id });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogError(id, "Order ID must be provided.");
                return NotFound();
            }

            if (!Guid.TryParse(id, out Guid orderId))
            {
                _logger.LogError(id, "Invalid Order ID format.");
                return BadRequest();
            }

            DetailsOrderViewModel? order = null;
            try
            {
                order = await _orderService.GetOrderDetailsAsync(orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving details for order with ID {OrderId}.", orderId);
                return BadRequest();
            }

            if (order == null)
            {

                _logger.LogError(nameof(order), "An error occurred while retrieving details for order with ID {OrderId}.", orderId);
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogError(id, "Order ID must be provided.");
                ModelState.AddModelError(nameof(id), "Order ID must be provided.");
                return NotFound();
            }

            if (!Guid.TryParse(id, out Guid orderId))
            {
                _logger.LogError(id, "Invalid Order ID format.");
                ModelState.AddModelError(nameof(orderId), "Invalid Order ID format.");
                return BadRequest();
            }

            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);

                if (order == null)
                {
                    _logger.LogError(nameof(order), "An error occurred while retrieving details for order with ID {OrderId}.", orderId);
                    ModelState.AddModelError(nameof(order), $"Order with ID {orderId} was not found.");
                    return NotFound();
                }

                if (order.IsCanceled)
                {
                    return RedirectToAction(nameof(Detail), "Order", new { id = id });
                }

                if (!await _orderService.CancelOrderAsync(orderId, order.UserID))
                {
                    _logger.LogError(nameof(order), "An error occurred while retrieving details for order with ID {OrderId}.", orderId);
                    ModelState.AddModelError(nameof(order), "Failed to cancel the order. It might already be in a state that cannot be canceled.");
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while canceling order with ID {OrderId}.", orderId);
                ModelState.AddModelError(string.Empty, "An internal server error occurred. Please try again later.");
                return BadRequest();
            }

            return RedirectToAction(nameof(Detail), "Order", new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reactivate(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogError(id, "Order ID must be provided.");
                ModelState.AddModelError(nameof(id), "Order ID must be provided.");
                return NotFound();
            }

            if (!Guid.TryParse(id, out Guid orderId))
            {
                _logger.LogError(id, "Invalid Order ID format.");
                ModelState.AddModelError(nameof(orderId), "Invalid Order ID format.");
                return BadRequest();
            }

            try
            {
                if (!await _orderService.ReactivateOrderAsync(orderId))
                {
                    _logger.LogError(string.Empty, "Failed to reactivate the order. It might not be in a cancelable state.");
                    ModelState.AddModelError(string.Empty, "Failed to reactivate the order. It might not be in a cancelable state.");
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while reactivating order with ID {OrderId}.", orderId);
                ModelState.AddModelError(string.Empty, "An internal server error occurred. Please try again later.");
                return BadRequest();
            }

            return RedirectToAction(nameof(Detail), "Order", new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(string? id, string? status)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogError(id, "Order ID must be provided.");
                ModelState.AddModelError(nameof(id), "Order ID must be provided.");
                return NotFound();
            }

            if (!Guid.TryParse(id, out Guid orderId))
            {
                _logger.LogError(id, "Invalid Order ID format.");
                ModelState.AddModelError(nameof(orderId), "Invalid Order ID format.");
                return BadRequest();
            }

            try
            {
                if (!await _orderService.ChangeOrderStatusAsync(orderId, status))
                {
                    _logger.LogError(string.Empty, $"Failed to change the order status to '{status}'. The status might be invalid or the order is not in a valid state for this change.");
                    ModelState.AddModelError(string.Empty, $"Failed to change the order status to '{status}'. The status might be invalid or the order is not in a valid state for this change.");
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while changing status of order with ID {OrderId} to {Status}.", orderId, status);
                ModelState.AddModelError(string.Empty, "An internal server error occurred. Please try again later.");
                return BadRequest();
            }

            return RedirectToAction(nameof(Detail), "Order", new { id = id });
        }

        private Dictionary<Guid, string?>? GetUsersInRole(string roleName)
        {
            try
            {
                return _userManager.GetUsersInRoleAsync(roleName)
                                   .Result
                                   .ToDictionary(
                                        user => user.Id,
                                        user => user.UserName
                                        );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting users in role {RoleName}.", roleName);
                return new Dictionary<Guid, string?>();
            }
        }
    }
}
