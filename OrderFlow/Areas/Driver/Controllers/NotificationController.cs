using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Models;
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
        public async Task<IActionResult> Index(string? sortBy = null)
        {
            if (!Guid.TryParse(this.GetUserId(), out var userId))
            {
                _logger.LogWarning("User ID is not valid.");
                return BadRequest("Invalid user ID.");
            }

            var notifications = await _notificationService.GetAllNotificationsForUserAsync(userId);

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
                        _logger.LogWarning("Invalid status filter provided: {Status}", sortBy);
                        return BadRequest("Invalid status filter.");
                }
            }

            if (notifications == null)
            {
                _logger.LogWarning("No notifications found for user with ID: {UserId}", userId);
                return NotFound("No notifications found.");
            }

            return View(notifications);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            if (!Guid.TryParse(id, out Guid notificationID))
            {
                return BadRequest();
            }

            if (!Guid.TryParse(this.GetUserId(), out Guid userId))
            {
                return BadRequest("Invalid User ID format.");
            }

            DetailsNotificationViewModel? notificationViewModel = await _notificationService.GetAll()
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
                return NotFound();
            }

            await _notificationService.ReadAsync(notificationID);

            if (!notificationViewModel.IsRead)
            {
                notificationViewModel.IsRead = true;
            }

            return View(notificationViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsUnread(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!Guid.TryParse(id, out Guid notificationId))
            {
                return BadRequest("Invalid Notification ID format.");
            }

            if (!Guid.TryParse(this.GetUserId(), out Guid userId))
            {
                return BadRequest("Invalid User ID format.");
            }

            if (!await _notificationService.GetAll()
                                           .AnyAsync(n => n.Id.Equals(notificationId) && n.ReceiverId.Equals(userId)))
            {
                return NotFound("Notification not found or does not belong to the user.");
            }

            await _notificationService.UnreadAsync(notificationId);

            return RedirectToAction(nameof(Index), "Notification"); ;
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!Guid.TryParse(id, out Guid notificationId))
            {
                return BadRequest("Invalid Notification ID format.");
            }

            if (!Guid.TryParse(this.GetUserId(), out Guid userId))
            {
                return BadRequest("Invalid User ID format.");
            }

            if (!await _notificationService.GetAll()
                                           .AnyAsync(n => n.Id.Equals(notificationId) && n.ReceiverId.Equals(userId)))
            {
                return NotFound("Notification not found or does not belong to the user.");
            }

            await _notificationService.ReadAsync(notificationId);

            return RedirectToAction(nameof(Index), "Notification");
        }

    }
}
