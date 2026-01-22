using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Notification;

namespace OrderFlow.Areas.Driver.Controllers
{
    public class NotificationController : BaseDriverController
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly INotificationService _notificationService;

        public NotificationController(ILogger<NotificationController> logger, INotificationService notificationService)
        {
            _logger = logger;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? sortBy = null, bool hideSystemNotifications = false)
        {
            if (!Guid.TryParse(this.GetUserId(), out var userId))
            {
                _logger.LogWarning("Invalid user ID format from session.");
                return BadRequest("Invalid user ID.");
            }

            try
            {
                var notifications = await _notificationService.GetAllNotificationsForDriverAsync(userId);

                if (notifications == null || !notifications.Any())
                {
                    _logger.LogInformation("No notifications found for user with ID: {UserId}", userId);
                    return View(new List<DriverIndexNotificationViewModel>());
                }

                ViewData["hideSystemNotifications"] = hideSystemNotifications;
                if (hideSystemNotifications)
                {
                    notifications = notifications.Where(n => !string.IsNullOrEmpty(n.SenderName)).ToList();
                }

                if (!string.IsNullOrEmpty(sortBy))
                {
                    switch (sortBy.ToLower())
                    {
                        case "all":
                            notifications = notifications.OrderBy(n => n.IsRead).ToList();
                            ViewData["CurrentSort"] = "All";
                            break;
                        case "unread":
                            notifications = notifications.Where(n => !n.IsRead).ToList();
                            ViewData["CurrentSort"] = "Unread";
                            break;
                        case "read":
                            notifications = notifications.Where(n => n.IsRead).ToList();
                            ViewData["CurrentSort"] = "Read";
                            break;
                        default:
                            _logger.LogWarning("Invalid status filter provided: {Status}", sortBy);
                            ModelState.AddModelError(string.Empty, "Invalid status filter.");
                            return BadRequest("Invalid status filter.");
                    }
                }

                return View(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving notifications for user {UserId}.", userId);
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid notificationId) || !Guid.TryParse(this.GetUserId(), out Guid userId))
            {
                _logger.LogWarning("Invalid ID or User ID format. Notification ID: {NotificationId}, User ID: {UserId}", id, this.GetUserId());
                return BadRequest();
            }

            try
            {
                var notificationViewModel = await _notificationService.GetAll()
                    .AsNoTracking()
                    .Include(n => n.Sender)
                    .Where(n => n.Id.Equals(notificationId) && n.ReceiverId.Equals(userId))
                    .Select(n => new DriverDetailsNotificationViewModel
                    {
                        Title = n.Title,
                        Message = n.Message,
                        CreatedAt = n.CreatedAt,
                        IsRead = n.IsRead,
                        OrderId = n.OrderId,
                        TruckId = n.TruckId,
                        SenderName = n.Sender!.UserName,
                    })
                    .SingleOrDefaultAsync();

                if (notificationViewModel == null)
                {
                    _logger.LogWarning("Notification with ID {NotificationId} not found for user {UserId}.", notificationId, userId);
                    return NotFound();
                }


                if (!notificationViewModel.IsRead)
                {
                    await _notificationService.ReadAsync(notificationId);
                    notificationViewModel.IsRead = true;
                }

                return View(notificationViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving or reading notification {NotificationId} for user {UserId}.", notificationId, userId);
                return BadRequest();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsUnread(string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid notificationId) || !Guid.TryParse(this.GetUserId(), out Guid userId))
            {
                _logger.LogWarning("Invalid ID or User ID format. Notification ID: {NotificationId}, User ID: {UserId}", id, this.GetUserId());
                ModelState.AddModelError(string.Empty, "Invalid request parameters.");
                return BadRequest();
            }

            try
            {
                bool notificationExists = await _notificationService.GetAll()
                    .AnyAsync(n => n.Id.Equals(notificationId) && n.ReceiverId.Equals(userId));

                if (!notificationExists)
                {
                    _logger.LogWarning("Notification with ID {NotificationId} not found for user {UserId}.", notificationId, userId);
                    ModelState.AddModelError(string.Empty, "Notification not found or does not belong to the user.");
                    return NotFound();
                }

                await _notificationService.UnreadAsync(notificationId);

                return RedirectToAction(nameof(Index), "Notification");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while marking notification {NotificationId} as unread for user {UserId}.", notificationId, userId);
                ModelState.AddModelError(string.Empty, "An error occurred while marking the notification as unread. Please try again later.");
                return BadRequest();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid notificationId) || !Guid.TryParse(this.GetUserId(), out Guid userId))
            {
                _logger.LogWarning("Invalid ID or User ID format. Notification ID: {NotificationId}, User ID: {UserId}", id, this.GetUserId());
                ModelState.AddModelError(string.Empty, "Invalid request parameters.");
                return BadRequest();
            }

            try
            {
                bool notificationExists = await _notificationService.GetAll()
                    .AnyAsync(n => n.Id.Equals(notificationId) && n.ReceiverId.Equals(userId));

                if (!notificationExists)
                {
                    _logger.LogWarning("Notification with ID {NotificationId} not found for user {UserId}.", notificationId, userId);
                    ModelState.AddModelError(string.Empty, "Notification not found or does not belong to the user.");
                    return NotFound();
                }

                await _notificationService.ReadAsync(notificationId);

                return RedirectToAction(nameof(Index), "Notification");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while marking notification {NotificationId} as read for user {UserId}.", notificationId, userId);
                ModelState.AddModelError(string.Empty, "An error occurred while marking the notification as read. Please try again later.");
                return BadRequest();
            }
        }
    }
}