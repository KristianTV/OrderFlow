using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Notification;

namespace OrderFlow.Areas.Admin.Controllers
{
    public class NotificationController : BaseAdminController
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly INotificationService _notificationService;
        private readonly IOrderService _orderService;
        private readonly ITruckService _truckService;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationController(ILogger<NotificationController> logger, INotificationService notificationService, IOrderService orderService, ITruckService truckService, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _notificationService = notificationService;
            _orderService = orderService;
            _userManager = userManager;
            _truckService = truckService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? sortBy = null, bool hideSystemNotifications = false)
        {
            try
            {
                if (!Guid.TryParse(this.GetUserId(), out Guid userId))
                {
                    _logger.LogWarning("User ID is not valid.");
                    return BadRequest();
                }

                var notifications = await _notificationService.GetAllNotificationsAsync(userId);

                if (notifications == null || !notifications.Any())
                {
                    _logger.LogInformation("No notifications found for user with ID: {UserId}", userId);
                    return View(new List<DriverIndexNotificationViewModel>());
                }

                ViewData["hideSystemNotifications"] = hideSystemNotifications;
                if (hideSystemNotifications)
                {
                    notifications = notifications?.Where(n => !string.IsNullOrEmpty(n.SenderName)).ToList();
                }

                if (sortBy != null)
                {
                    switch (sortBy.ToLower())
                    {
                        case "all":
                            notifications = notifications?.OrderBy(n => n.IsRead).ToList();
                            ViewData["CurrentSort"] = "All";
                            break;
                        case "unread":
                            notifications = notifications?.Where(n => !n.IsRead).ToList();
                            ViewData["CurrentSort"] = "Unread";
                            break;
                        case "read":
                            notifications = notifications?.Where(n => n.IsRead).ToList();
                            ViewData["CurrentSort"] = "Read";
                            break;
                        default:
                            _logger.LogWarning("Invalid status filter provided: {0}", sortBy);
                            ModelState.AddModelError(nameof(sortBy), string.Join("Invalid status filter provided: {0}", sortBy));
                            return BadRequest();
                    }
                }

                if (notifications == null)
                {
                    return View(new List<object>());
                }

                return View(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving notifications.");
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create(string? orderId)
        {
            try
            {
                var createNotification = new AdminCreateNotificationViewModel
                {
                    Receivers = await GetUsers(),
                    Orders = await GetOrders(),
                    Trucks = await GetTrucks(),
                };

                if (!string.IsNullOrEmpty(orderId))
                {
                    if (!Guid.TryParse(orderId, out Guid orderGuid))
                    {
                        _logger.LogWarning("Invalid Order ID format: {0}", orderId);
                        return BadRequest();
                    }

                    var receiverId = await _orderService.GetAll()
                                                        .Where(o => o.OrderID.Equals(orderGuid))
                                                        .Select(o => o.UserID)
                                                        .SingleOrDefaultAsync();

                    createNotification.OrderId = orderGuid;
                    createNotification.ReceiverId = receiverId;
                }

                return View(createNotification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while preparing the create notification view.");
                return BadRequest();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminCreateNotificationViewModel createNotification)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    createNotification.Receivers = await GetUsers();
                    createNotification.Orders = await GetOrders();
                    createNotification.Trucks = await GetTrucks();
                    return View(createNotification);
                }

                if (!Guid.TryParse(this.GetUserId(), out var senderId))
                {
                    _logger.LogWarning("Sender ID is not valid.");
                    ModelState.AddModelError(nameof(senderId), "Sender ID is not valid.");
                    return BadRequest();
                }

                await _notificationService.CreateNotificationAsync(createNotification, senderId);

                return RedirectToAction(nameof(Index), "Notification");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a notification.");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");

                createNotification.Receivers = await GetUsers();
                createNotification.Orders = await GetOrders();
                createNotification.Trucks = await GetTrucks();
                return View(createNotification);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return NotFound();
                }

                if (!Guid.TryParse(id, out Guid notificationId))
                {
                    _logger.LogWarning("Invalid Notification ID format.");
                    return BadRequest();
                }

                if (!Guid.TryParse(this.GetUserId(), out Guid userId))
                {
                    _logger.LogWarning("Invalid User ID format.");
                    return BadRequest();
                }

                AdminCreateNotificationViewModel? createNotification = await _notificationService.GetAll()
                                                                                                .Where(n => n.NotificationID.Equals(notificationId) && n.SenderID.Equals(userId))
                                                                                                .Select(o => new AdminCreateNotificationViewModel
                                                                                                {
                                                                                                    Title = o.Title,
                                                                                                    Message = o.Message,
                                                                                                    ReceiverId = o.ReceiverID,
                                                                                                    OrderId = o.OrderID,
                                                                                                }).SingleOrDefaultAsync();

                if (createNotification == null)
                {
                    return NotFound();
                }

                createNotification.Receivers = await GetUsers();
                createNotification.Orders = await GetOrders();
                createNotification.Trucks = await GetTrucks();

                return View(createNotification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while preparing the edit notification view for ID: {0}", id);
                return BadRequest();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminCreateNotificationViewModel createNotification, string? id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return NotFound();
                }

                if (!Guid.TryParse(id, out Guid notificationId))
                {
                    _logger.LogWarning("Invalid Notification ID format.");
                    ModelState.AddModelError(nameof(notificationId), "Invalid Notification ID format.");
                    return BadRequest();
                }

                if (!ModelState.IsValid)
                {
                    createNotification.Receivers = await GetUsers();
                    createNotification.Orders = await GetOrders();
                    createNotification.Trucks = await GetTrucks();
                    return View(createNotification);
                }

                if (!Guid.TryParse(this.GetUserId(), out Guid userId))
                {
                    _logger.LogWarning("Invalid User ID format.");
                    ModelState.AddModelError(nameof(userId), "Invalid User ID format.");
                    return BadRequest();
                }

                if (!await _notificationService.UpdateNotificationAsync(createNotification, notificationId, userId))
                {
                    _logger.LogWarning("Failed to update notification with ID {NotificationId}. It might not exist or the user is not the sender.", notificationId);
                    return NotFound();
                }

                return RedirectToAction(nameof(Detail), "Notification", new { id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while editing notification with ID: {NotificationId}", id);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");

                createNotification.Receivers = await GetUsers();
                createNotification.Orders = await GetOrders();
                createNotification.Trucks = await GetTrucks();
                return View(createNotification);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string? id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return NotFound();
                }

                if (!Guid.TryParse(id, out Guid notificationID))
                {
                    _logger.LogWarning("Invalid Notification ID format.");
                    return BadRequest();
                }

                if (!Guid.TryParse(this.GetUserId(), out Guid userId))
                {
                    _logger.LogWarning("Invalid User ID format.");
                    return BadRequest();
                }

                DriverDetailsNotificationViewModel? notificationViewModel = await _notificationService.GetAll()
                                                                                                    .AsNoTracking()
                                                                                                    .Include(n => n.Sender)
                                                                                                    .Where(n => n.NotificationID.Equals(notificationID))
                                                                                                    .Select(n => new DriverDetailsNotificationViewModel
                                                                                                    {
                                                                                                        Title = n.Title,
                                                                                                        Message = n.Message,
                                                                                                        CreatedAt = n.CreatedAt,
                                                                                                        IsRead = n.IsRead,
                                                                                                        OrderId = n.OrderID,
                                                                                                        TruckId = n.TruckID,
                                                                                                        SenderName = n.Sender!.UserName,
                                                                                                        isMarkable = n.ReceiverID.Equals(userId)
                                                                                                    }).SingleOrDefaultAsync();

                if (notificationViewModel == null)
                {
                    return NotFound();
                }

                if (!notificationViewModel.IsRead && notificationViewModel.isMarkable)
                {
                    await _notificationService.ReadAsync(notificationID);
                    notificationViewModel.IsRead = true;
                }

                return View(notificationViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving notification details for ID: {NotificationId}", id);
                return BadRequest();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsUnread(string? id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return NotFound();
                }

                if (!Guid.TryParse(id, out Guid notificationId))
                {
                    _logger.LogWarning("Invalid Notification ID format.");
                    ModelState.AddModelError(nameof(notificationId), "Invalid Notification ID format.");
                    return BadRequest();
                }

                if (!Guid.TryParse(this.GetUserId(), out Guid userId))
                {
                    _logger.LogWarning("Invalid User ID format.");
                    ModelState.AddModelError(nameof(userId), "Invalid User ID format.");
                    return BadRequest();
                }

                if (!await _notificationService.GetAll().AnyAsync(n => n.NotificationID.Equals(notificationId) && n.ReceiverID.Equals(userId)))
                {
                    _logger.LogWarning("Notification not found or does not belong to the user.");
                    ModelState.AddModelError(nameof(userId), "Notification not found or does not belong to the user.");
                    return NotFound();
                }

                await _notificationService.UnreadAsync(notificationId);

                return RedirectToAction(nameof(Index), "Notification");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while marking notification as unread for ID: {NotificationId}", id);
                ModelState.AddModelError(string.Empty, string.Join("An error occurred while marking notification as unread for ID: {0}", id));
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(string? id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return NotFound();
                }


                if (!Guid.TryParse(id, out Guid notificationId))
                {
                    _logger.LogWarning("Invalid Notification ID format.");
                    ModelState.AddModelError(nameof(notificationId), "Invalid Notification ID format.");
                    return BadRequest();
                }

                if (!Guid.TryParse(this.GetUserId(), out Guid userId))
                {
                    _logger.LogWarning("Invalid User ID format.");
                    ModelState.AddModelError(nameof(userId), "Invalid User ID format.");
                    return BadRequest();
                }

                if (!await _notificationService.GetAll().AnyAsync(n => n.NotificationID.Equals(notificationId) && n.ReceiverID.Equals(userId)))
                {

                    _logger.LogWarning("Notification not found or does not belong to the user.");
                    ModelState.AddModelError(nameof(userId), "Notification not found or does not belong to the user.");
                    return NotFound();
                }

                await _notificationService.ReadAsync(notificationId);

                return RedirectToAction(nameof(Index), "Notification");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while marking notification as read for ID: {NotificationId}", id);
                ModelState.AddModelError(string.Empty, string.Join("An error occurred while marking notification as read for ID: {0}", id));
                return BadRequest();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string? id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return NotFound();
                }

                if (!Guid.TryParse(id, out Guid notificationId))
                {
                    _logger.LogWarning("Invalid Notification ID format.");
                    ModelState.AddModelError(nameof(notificationId), "Invalid Notification ID format.");
                    return BadRequest();
                }

                if (!await _notificationService.GetAll().AnyAsync(n => n.NotificationID.Equals(notificationId)))
                {
                    _logger.LogError("Notification not found.");
                    ModelState.AddModelError(string.Empty, "Notification not found.");
                    return NotFound();
                }

                await _notificationService.SoftDelete(notificationId);

                return RedirectToAction(nameof(Index), "Notification");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting notification with ID: {NotificationId}", id);
                ModelState.AddModelError(string.Empty, string.Join("An error occurred while deleting notification with ID: {0}", id));
                return BadRequest();
            }
        }

        private async Task<Dictionary<Guid, string?>> GetUsers()
        {
            return await _userManager.Users.ToDictionaryAsync(
                                                                user => user.Id,
                                                                user => user.UserName
                                                             );
        }

        private async Task<Dictionary<Guid, string>> GetOrders()
        {
            return await _orderService.GetAll()
                                      .Include(order => order.User)
                                      .Where(order => !(new[] { OrderStatus.Completed,
                                                               OrderStatus.Cancelled,
                                                               OrderStatus.Failed
                                                               }.Contains(order.Status)))
                                      .ToDictionaryAsync(
                                                          order => order.OrderID,
                                                          order => order.OrderID.ToString() + " - " + order.User.UserName
                                                         );
        }

        private async Task<Dictionary<Guid, string>> GetTrucks()
        {
            return await _truckService.GetAll()
                                      .Include(t => t.Driver)
                                      .ToDictionaryAsync(
                                                           t => t.TruckID,
                                                           t => t.TruckID.ToString() + " - " + t.Driver.UserName
                                                         );
        }
    }
}