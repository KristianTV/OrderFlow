using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services;
using OrderFlow.Services.Contracts;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Order;

namespace OrderFlow.Areas.Admin.Controllers
{
    public class OrderController : BaseAdminController
    {
        private const int IndexPageSize = 12;
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService _orderService;
        private readonly INotificationService _notificationService;
        private readonly IMailService _mailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRealtimeNotifier _realtimeNotifier;

        public OrderController(ILogger<OrderController> logger,
                               IOrderService orderService,
                               INotificationService notificationService,
                               IMailService mailService,
                               UserManager<ApplicationUser> userManager,
                               IRealtimeNotifier? realtimeNotifier = null)
        {
            _logger = logger;
            _orderService = orderService;
            _mailService = mailService;
            _notificationService = notificationService;
            _userManager = userManager;
            _realtimeNotifier = realtimeNotifier ?? NullRealtimeNotifier.Instance;
        }

        [HttpGet]
        public async Task<IActionResult> Index(bool? hideCompleted, string? searchId = null, string? statusFilter = null, string? sortOrder = null, int page = 1)
        {
            try
            {
                List<IndexOrderViewModel> indexOrders = (await _orderService.GetAdminOrdersAsync(new OrderQueryModel
                {
                    HideCompleted = hideCompleted.GetValueOrDefault(),
                    SearchId = searchId,
                    StatusFilter = statusFilter,
                    SortOrder = sortOrder,
                    Page = page,
                    PageSize = IndexPageSize + 1
                })).ToList();

                bool hasMore = indexOrders.Count > IndexPageSize;
                HttpContext?.Response?.Headers.TryAdd("X-Has-More", hasMore.ToString().ToLowerInvariant());
                indexOrders = indexOrders.Take(IndexPageSize).ToList();

                if (IsAjaxRequest())
                {
                    ViewData["OrderArea"] = "Admin";
                    return PartialView("~/Views/Shared/_OrderCards.cshtml", indexOrders);
                }

                ViewBag.HasMore = hasMore;

                return View(indexOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving orders.");
                return BadRequest();
            }
        }

        private bool IsAjaxRequest()
        {
            return HttpContext?.Request?.Headers.XRequestedWith.ToString() == "XMLHttpRequest";
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

            Guid createdOrderId = _orderService.GetAll()
                                               .Where(o => o.UserID.Equals(createOrderViewModel.UsersId))
                                               .OrderByDescending(o => o.OrderDate)
                                               .Select(o => o.OrderID)
                                               .FirstOrDefault();
            await _realtimeNotifier.EntityChangedAsync(new RealtimeEntityChanged
            {
                Entity = "Order",
                Action = "Created",
                Id = createdOrderId,
                UserIds = new[] { createOrderViewModel.UsersId },
                Roles = new[] { UserRoles.Admin.ToString(), UserRoles.Speditor.ToString() }
            });

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
                _logger.LogError($"Order with ID {orderId} was not found.");
                return NotFound();
            }

            createOrderViewModel.Users = GetUsersInRole(UserRoles.User.ToString());

            ViewBag.OrderId = orderId;

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
                    ViewBag.OrderId = orderId;
                    return View(createOrderViewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating order with ID {OrderId}.", orderId);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                createOrderViewModel.Users = GetUsersInRole(UserRoles.User.ToString());
                ViewBag.OrderId = orderId;
                return View(createOrderViewModel);
            }

            await _realtimeNotifier.EntityChangedAsync(new RealtimeEntityChanged
            {
                Entity = "Order",
                Action = "Updated",
                Id = orderId,
                UserIds = new[] { createOrderViewModel.UsersId },
                Roles = new[] { UserRoles.Admin.ToString(), UserRoles.Speditor.ToString() }
            });

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

                _logger.LogError("An error occurred while retrieving details for order with ID {OrderId}.", orderId);
                return NotFound();
            }

            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> Invoice(string? id)
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

            try
            {
                InvoiceViewModel? orderInvoice = await _orderService.GetOrderInvoiceDetailsAsync(orderId);

                if (orderInvoice == null)
                {
                    return NotFound();
                }

                return View("~/Views/Order/Invoice.cshtml", orderInvoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving invoice for order with ID {OrderId}.", orderId);
                return BadRequest();
            }
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

            Guid orderUserId = Guid.Empty;
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);

                if (order == null)
                {
                    _logger.LogError("An error occurred while retrieving details for order with ID {OrderId}.", orderId);
                    ModelState.AddModelError(nameof(order), $"Order with ID {orderId} was not found.");
                    return NotFound();
                }

                if (order.IsCanceled)
                {
                    return RedirectToAction(nameof(Detail), "Order", new { id = id });
                }

                if (!await _orderService.CancelOrderAsync(orderId, order.UserID))
                {
                    _logger.LogError("An error occurred while retrieving details for order with ID {OrderId}.", orderId);
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

            await _realtimeNotifier.EntityChangedAsync(new RealtimeEntityChanged
            {
                Entity = "Order",
                Action = "Cancelled",
                Id = orderId,
                UserIds = new[] { orderUserId },
                Roles = new[] { UserRoles.Admin.ToString(), UserRoles.Speditor.ToString() }
            });
            await _realtimeNotifier.NotificationCountChangedAsync(orderUserId);

            await _notificationService.SendSystemNotificationAsync(new Services.Core.Commands.NotificationCommand
            {
                ReceiverID = orderUserId,
                Title = "Order Cancelled",
                Message = $"Your order with ID {orderId} has been cancelled by an administrator. Please check the order details for more information.",
                OrderID = orderId
            });

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

            Order? reactivatedOrder = await _orderService.GetOrderByIdAsync(orderId);
            await _realtimeNotifier.EntityChangedAsync(new RealtimeEntityChanged
            {
                Entity = "Order",
                Action = "Reactivated",
                Id = orderId,
                UserIds = reactivatedOrder != null ? new[] { reactivatedOrder.UserID } : Enumerable.Empty<Guid>(),
                Roles = new[] { UserRoles.Admin.ToString(), UserRoles.Speditor.ToString() }
            });
            if (reactivatedOrder != null)
            {
                await _realtimeNotifier.NotificationCountChangedAsync(reactivatedOrder.UserID);
            }

            await _notificationService.SendSystemNotificationAsync(new Services.Core.Commands.NotificationCommand
            {
                ReceiverID = reactivatedOrder?.UserID ?? Guid.Empty,
                Title = "Order Reactivated",
                Message = $"Your order with ID {orderId} has been reactivated by an administrator. Please check the order details for more information.",
                OrderID = orderId
            });

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

            Order? changedOrder = await _orderService.GetOrderByIdAsync(orderId);
            await _realtimeNotifier.EntityChangedAsync(new RealtimeEntityChanged
            {
                Entity = "Order",
                Action = "StatusChanged",
                Id = orderId,
                UserIds = changedOrder != null ? new[] { changedOrder.UserID } : Enumerable.Empty<Guid>(),
                Roles = new[] { UserRoles.Admin.ToString(), UserRoles.Speditor.ToString() }
            });
            if (changedOrder != null)
            {
                await _realtimeNotifier.NotificationCountChangedAsync(changedOrder.UserID);
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
