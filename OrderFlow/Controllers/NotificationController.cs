using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Notification;

namespace OrderFlow.Controllers
{
    public class NotificationController : BaseController
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
                _logger.LogWarning("Invalid user ID format received in Index action.");
                return BadRequest();
            }

            try
            {
                var notifications = await _notificationService.GetAllNotificationsForUserAsync(userId);

                if (notifications == null || !notifications.Any())
                {
                    _logger.LogInformation("No notifications found for user with ID: {UserId}", userId);
                    return View(new List<IndexNotificationViewModel>()); 
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
                            _logger.LogWarning("Invalid status filter provided: {Status} for user: {UserId}", sortBy, userId);
                            return BadRequest();
                    }
                }
                else
                {
                    ViewData["CurrentSort"] = "All";
                }

                return View(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving notifications for user ID: {UserId}.", userId);
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid notificationID))
            {
                _logger.LogWarning("Invalid or missing notification ID received in Detail action.");
                return BadRequest();
            }

            if (!Guid.TryParse(this.GetUserId(), out Guid userId))
            {
                _logger.LogWarning("Invalid user ID format received in Detail action for notification ID: {NotificationId}.", notificationID);
                return BadRequest();
            }

            try
            {
                var notificationViewModel = await _notificationService.GetAll()
                                                                       .AsNoTracking()
                                                                       .Include(n => n.Sender)
                                                                       .Where(n => n.Id.Equals(notificationID) && n.ReceiverId.Equals(userId))
                                                                       .Select(n => new DetailsNotificationViewModel
                                                                       {
                                                                           Title = n.Title,
                                                                           Message = n.Message,
                                                                           CreatedAt = n.CreatedAt,
                                                                           IsRead = n.IsRead,
                                                                           OrderId = n.OrderId,
                                                                           SenderName = n.Sender!.UserName,
                                                                       }).SingleOrDefaultAsync();

                if (notificationViewModel == null)
                {
                    _logger.LogInformation("Notification with ID {NotificationId} not found or does not belong to user {UserId}.", notificationID, userId);
                    return NotFound();
                }

                if (!notificationViewModel.IsRead)
                {
                    await _notificationService.ReadAsync(notificationID);
                    notificationViewModel.IsRead = true;
                }

                return View(notificationViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving details for notification ID: {NotificationId} and user ID: {UserId}.", notificationID, userId);
                return BadRequest();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsUnread(string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid notificationId))
            {
                _logger.LogWarning("Invalid or missing notification ID received in MarkAsUnread action.");
                return BadRequest();
            }

            if (!Guid.TryParse(this.GetUserId(), out Guid userId))
            {
                _logger.LogWarning("Invalid user ID format received in MarkAsUnread action for notification ID: {NotificationId}.", notificationId);
                return BadRequest();
            }

            try
            {
                bool notificationExists = await _notificationService.GetAll()
                                                                     .AnyAsync(n => n.Id.Equals(notificationId) && n.ReceiverId.Equals(userId));

                if (!notificationExists)
                {
                    _logger.LogInformation("Attempt to mark non-existent or unauthorized notification {NotificationId} as unread by user {UserId}.", notificationId, userId);
                    return NotFound();
                }

                await _notificationService.UnreadAsync(notificationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while marking notification {NotificationId} as unread for user {UserId}.", notificationId, userId);
                return BadRequest();
            }

            return RedirectToAction(nameof(Index), "Notification");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid notificationId))
            {
                _logger.LogWarning("Invalid or missing notification ID received in MarkAsRead action.");
                return BadRequest();
            }

            if (!Guid.TryParse(this.GetUserId(), out Guid userId))
            {
                _logger.LogWarning("Invalid user ID format received in MarkAsRead action for notification ID: {NotificationId}.", notificationId);
                return BadRequest();
            }

            try
            {
                bool notificationExists = await _notificationService.GetAll()
                                                                     .AnyAsync(n => n.Id.Equals(notificationId) && n.ReceiverId.Equals(userId));

                if (!notificationExists)
                {
                    _logger.LogInformation("Attempt to mark non-existent or unauthorized notification {NotificationId} as read by user {UserId}.", notificationId, userId);
                    return NotFound();
                }

                await _notificationService.ReadAsync(notificationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while marking notification {NotificationId} as read for user {UserId}.", notificationId, userId);
                return BadRequest();
            }

            return RedirectToAction(nameof(Index), "Notification");
        }
    }
}