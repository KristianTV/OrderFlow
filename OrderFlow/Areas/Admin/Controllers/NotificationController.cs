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
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationController(ILogger<NotificationController> logger, INotificationService notificationService, IOrderService orderService, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _notificationService = notificationService;
            _orderService = orderService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? sortBy = null)
        {

            if (!Guid.TryParse(this.GetUserId(), out Guid userId))
            {
                _logger.LogWarning("User ID is not valid.");
                return BadRequest("Invalid user ID.");
            }

            var notifications = await _notificationService.GetAllNotificationsAsync(userId);

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
        public async Task<IActionResult> Create(string? orderId)
        {
            Dictionary<Guid, string> orders = await _orderService.All<Order>()
                                                                 .Include(order => order.User)
                                                                 .Where(order => !(new[] { OrderStatus.Completed,
                                                                                           OrderStatus.Cancelled,
                                                                                           OrderStatus.Failed
                                                                                          }.Contains(order.Status)))
                                                                  .ToDictionaryAsync(
                                                                                order => order.OrderID,
                                                                                order => order.OrderID.ToString() + " - " + order.User.UserName
                                                                            );

            Dictionary<Guid, string?> receivers = await _userManager.Users.ToDictionaryAsync(
                                                                                            user => user.Id,
                                                                                            user => user.UserName
                                                                                           );

            CreateNotificationViewModel createNotification = new CreateNotificationViewModel
            {
                Receivers = receivers,
                Orders = orders,
            };

            if (!string.IsNullOrEmpty(orderId))
            {
                if (!Guid.TryParse(orderId, out Guid orderGuid))
                {
                    _logger.LogWarning("Invalid Order ID format: {OrderId}", orderId);
                    return BadRequest("Invalid Order ID format.");
                }

                var receiverId = await _orderService.All<Order>()
                                              .Where(o => o.OrderID.Equals(orderGuid))
                                              .Select(o => o.UserID)
                                              .SingleOrDefaultAsync();

                createNotification.OrderId = orderGuid;
                createNotification.ReceiverId = receiverId;
            }


            return View(createNotification);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateNotificationViewModel createPayment)
        {
            if (!ModelState.IsValid)
            {
                return View(createPayment);
            }

            if (!Guid.TryParse(this.GetUserId(), out var senderId))
            {
                _logger.LogWarning("Sender ID is not valid.");
                return BadRequest("Invalid sender ID.");
            }

            await _notificationService.CreateNotificationAsync(createPayment, senderId);

            return RedirectToAction(nameof(Index), "Notification");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!Guid.TryParse(id, out Guid notification))
            {
                return BadRequest("Invalid Notification ID format.");
            }

            if (!Guid.TryParse(this.GetUserId(), out Guid userId))
            {
                return BadRequest("Invalid User ID format.");
            }

            Dictionary<Guid, string> orders = await _orderService.All<Order>()
                                                            .ToDictionaryAsync(
                                                                              order => order.OrderID,
                                                                              order => order.OrderID.ToString()
                                                                          );

            Dictionary<Guid, string> recivers = await _userManager.Users.ToDictionaryAsync(
                                                                                            user => user.Id,
                                                                                            user => user.UserName
                                                                                           );

            CreateNotificationViewModel? createNotification = await _notificationService.All<Notification>()
                                                                                        .Where(n => n.Id.Equals(notification) &&
                                                                                                     n.SenderId.Equals(userId))
                                                                                        .Select(o => new CreateNotificationViewModel
                                                                                        {
                                                                                            Title = o.Title,
                                                                                            Message = o.Message,
                                                                                            ReceiverId = o.ReceiverId,
                                                                                            OrderId = o.OrderId,
                                                                                            Receivers = recivers,
                                                                                            Orders = orders
                                                                                        }).SingleOrDefaultAsync();

            if (createNotification == null)
            {
                return NotFound();
            }

            return View(createNotification);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CreateNotificationViewModel createNotification, string? id)
        {
            if (!ModelState.IsValid)
                return View(createNotification);

            if (id == null)
            {
                return NotFound();
            }

            if (!Guid.TryParse(id, out Guid notification))
            {
                return BadRequest("Invalid Notification ID format.");
            }

            if (!Guid.TryParse(this.GetUserId(), out Guid userId))
            {
                return BadRequest("Invalid User ID format.");
            }

            if (!await _notificationService.UpdateNotificationAsync(createNotification, notification, userId))
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Detail), "Notification", new { id = id });
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

            DetailsNotificationViewModel? notificationViewModel = await _notificationService.All<Notification>()
                                                                                     .AsNoTracking()
                                                                                     .Include(n => n.Sender)
                                                                                     .Where(n => n.Id.Equals(notificationID))
                                                                                     .Select(n => new DetailsNotificationViewModel
                                                                                     {
                                                                                         Title = n.Title,
                                                                                         Message = n.Message,
                                                                                         CreatedAt = n.CreatedAt,
                                                                                         IsRead = n.IsRead,
                                                                                         OrderId = n.OrderId,
                                                                                         SenderName = n.Sender!.UserName,
                                                                                         isMarkable = n.ReceiverId.Equals(userId)
                                                                                     }).SingleOrDefaultAsync();


            if (notificationViewModel == null)
            {
                return NotFound();
            }

            if (_notificationService.All<Notification>().Any(n => n.Id.Equals(notificationID) && n.ReceiverId.Equals(userId)))
            {
                await _notificationService.ReadAsync(notificationID);

                if (!notificationViewModel.IsRead)
                {
                    notificationViewModel.IsRead = true;
                }
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

            if (!await _notificationService.All<Notification>()
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

            if (!await _notificationService.All<Notification>()
                                           .AnyAsync(n => n.Id.Equals(notificationId) && n.ReceiverId.Equals(userId)))
            {
                return NotFound("Notification not found or does not belong to the user.");
            }

            await _notificationService.ReadAsync(notificationId);

            return RedirectToAction(nameof(Index), "Notification");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (!Guid.TryParse(id, out Guid notificationId))
            {
                return BadRequest("Invalid Notification ID format.");
            }
            if (!await _notificationService.All<Notification>()
                                           .AnyAsync(n => n.Id.Equals(notificationId)))
            {
                return NotFound("Notification not found or does not belong to the user.");
            }
            await _notificationService.SoftDelete(notificationId);
            return RedirectToAction(nameof(Index), "Notification");
        }
    }
}
