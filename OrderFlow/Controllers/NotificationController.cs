using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Message;
using OrderFlow.ViewModels.Notification;

namespace OrderFlow.Controllers
{
    public class NotificationController : BaseController
    {
        private const int IndexPageSize = 12;
        private readonly ILogger<NotificationController> _logger;
        private readonly INotificationService _notificationService;

        public NotificationController(ILogger<NotificationController> logger, INotificationService notificationService)
        {
            _logger = logger;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? sortBy = "all", bool hideSystemNotifications = false, int page = 1)
        {
            if (!Guid.TryParse(this.GetUserId(), out var userId))
            {
                _logger.LogWarning("Invalid user ID format received in Index action.");
                return BadRequest();
            }

            try
            {
                var queryModel = new NotificationQueryModel
                {
                    SortBy = sortBy,
                    HideSystemNotifications = hideSystemNotifications,
                    Page = page,
                    PageSize = IndexPageSize + 1
                };

                var notificationsList = (await _notificationService.GetAllNotificationsForUserAsync(userId, queryModel))?.ToList()
                    ?? new List<IndexNotificationViewModel>();

                bool hasMore = notificationsList.Count > IndexPageSize;
                HttpContext?.Response?.Headers.TryAdd("X-Has-More", hasMore.ToString().ToLowerInvariant());
                notificationsList = notificationsList.Take(IndexPageSize).ToList();

                ViewData["hideSystemNotifications"] = hideSystemNotifications;
                ViewData["CurrentSort"] = string.IsNullOrEmpty(sortBy) ? "All" : char.ToUpper(sortBy[0]) + sortBy.Substring(1).ToLower();

                if (IsAjaxRequest())
                {
                    ViewData["NotificationArea"] = string.Empty;
                    return PartialView("~/Views/Shared/_NotificationCards.cshtml", notificationsList);
                }

                ViewBag.HasMore = hasMore;

                return View(notificationsList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving notifications for user ID: {UserId}.", userId);
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
                                                                       .Where(n => n.NotificationID.Equals(notificationID) && n.ReceiverID.Equals(userId))
                                                                       .Select(n => new DetailsNotificationViewModel
                                                                       {
                                                                           Title = n.Title,
                                                                           Message = n.Message,
                                                                           CreatedAt = n.CreatedAt,
                                                                           IsRead = n.IsRead,
                                                                           OrderId = n.OrderID,
                                                                           PaymentId = n.PaymentID,
                                                                           SenderName = n.Sender!.UserName,
                                                                           IsResponseEnabled = n.CanRespond,
                                                                           Messages = n.Messages
                                                                                                .Where(m => m.IsDeleted == false)
                                                                                                .Select(m => new DetailsNotificationMessageViewModel
                                                                                                {
                                                                                                    MessageID = m.MessageID,
                                                                                                    SenderID = m.SenderID,
                                                                                                    SenderName = m.Sender.UserName ?? string.Empty,
                                                                                                    Content = m.Content,
                                                                                                    SentAt = m.SentAt,
                                                                                                    IsRead = m.IsRead
                                                                                                })
                                                                                                .OrderBy(m => m.SentAt).ToList()
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
                ViewBag.NotificationId = notificationID;
                ViewBag.CurrentUserId = userId;
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
                                                                     .AnyAsync(n => n.NotificationID.Equals(notificationId) && n.ReceiverID.Equals(userId));

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
                                                                     .AnyAsync(n => n.NotificationID.Equals(notificationId) && n.ReceiverID.Equals(userId));

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
