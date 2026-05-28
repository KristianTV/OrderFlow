using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Message;
using OrderFlow.ViewModels.Notification;

namespace OrderFlow.Areas.Driver.Controllers
{
    public class NotificationController : BaseDriverController
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
                _logger.LogWarning("Invalid user ID format from session.");
                return BadRequest("Invalid user ID.");
            }

            try
            {
                var queryModel = new NotificationQueryModel
                {
                    SortBy = sortBy,
                    HideSystemNotifications = hideSystemNotifications,
                    Page = page,
                    PageSize = IndexPageSize
                };

                var notificationsList = (await _notificationService.GetAllNotificationsForDriverAsync(userId, queryModel))?.ToList()
                    ?? new List<DriverIndexNotificationViewModel>();

                bool hasMore = notificationsList.Count > queryModel.PageSize;
                HttpContext?.Response?.Headers.TryAdd("X-Has-More", hasMore.ToString().ToLowerInvariant());
                notificationsList = notificationsList.Take(queryModel.PageSize).ToList();

                ViewData["hideSystemNotifications"] = hideSystemNotifications;
                ViewData["CurrentSort"] = string.IsNullOrEmpty(sortBy) ? "All" : char.ToUpper(sortBy[0]) + sortBy.Substring(1).ToLower();

                if (IsAjaxRequest())
                {
                    ViewData["NotificationArea"] = "Driver";
                    return PartialView("~/Views/Shared/_NotificationCards.cshtml", notificationsList);
                }

                ViewBag.HasMore = hasMore;

                return View(notificationsList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving notifications for user {UserId}.", userId);
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
                    .Include(n => n.Messages)
                    .ThenInclude(m => m.Sender)
                    .Where(n => n.NotificationID.Equals(notificationId) && n.ReceiverID.Equals(userId))
                    .Select(n => new DriverDetailsNotificationViewModel
                    {
                        Title = n.Title,
                        Message = n.Message,
                        CreatedAt = n.CreatedAt,
                        IsRead = n.IsRead,
                        TruckId = n.TruckID,
                        TruckSpendingId = n.TruckSpendingID,
                        CourseId = n.CourseID,
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
                ViewBag.NotificationId = notificationId;
                ViewBag.CurrentUserId = userId;
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
                    .AnyAsync(n => n.NotificationID.Equals(notificationId) && n.ReceiverID.Equals(userId));

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
                    .AnyAsync(n => n.NotificationID.Equals(notificationId) && n.ReceiverID.Equals(userId));

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
