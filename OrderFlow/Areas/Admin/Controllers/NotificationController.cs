using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Message;
using OrderFlow.ViewModels.Notification;

namespace OrderFlow.Areas.Admin.Controllers
{
    public class NotificationController : BaseAdminController
    {
        private const int IndexPageSize = 12;
        private readonly ILogger<NotificationController> _logger;
        private readonly INotificationService _notificationService;
        private readonly IOrderService _orderService;
        private readonly ITruckService _truckService;
        private readonly ITruckCourseService _truckCourseService;
        private readonly IPaymentService _paymentService;
        // private readonly itru _truckService;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationController(ILogger<NotificationController> logger,
                                        INotificationService notificationService,
                                        IOrderService orderService,
                                        ITruckService truckService,
                                        ITruckCourseService truckCourseService,
                                        IPaymentService paymentService,
                                        UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _notificationService = notificationService;
            _orderService = orderService;
            _userManager = userManager;
            _truckService = truckService;
            _truckCourseService = truckCourseService;
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? sortBy = "all", bool hideSystemNotifications = false, int page = 1)
        {
            try
            {
                if (!Guid.TryParse(this.GetUserId(), out Guid userId))
                {
                    _logger.LogWarning("User ID is not valid.");
                    return BadRequest();
                }

                var queryModel = new NotificationQueryModel
                {
                    SortBy = sortBy,
                    HideSystemNotifications = hideSystemNotifications,
                    Page = page,
                    PageSize = IndexPageSize + 1
                };

                List<DriverIndexNotificationViewModel> notificationsList = (await _notificationService.GetAllNotificationsAsync(userId, queryModel))?.ToList()
                    ?? new List<DriverIndexNotificationViewModel>();

                bool hasMore = notificationsList.Count > IndexPageSize;
                HttpContext?.Response?.Headers.TryAdd("X-Has-More", hasMore.ToString().ToLowerInvariant());
                notificationsList = notificationsList.Take(IndexPageSize).ToList();

                ViewData["hideSystemNotifications"] = hideSystemNotifications;
                ViewData["CurrentSort"] = string.IsNullOrEmpty(sortBy) ? "All" : char.ToUpper(sortBy[0]) + sortBy.Substring(1).ToLower();

                if (IsAjaxRequest())
                {
                    ViewData["NotificationArea"] = "Admin";
                    return PartialView("~/Views/Shared/_NotificationCards.cshtml", notificationsList);
                }

                ViewBag.HasMore = hasMore;

                return View(notificationsList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving notifications.");
                return BadRequest();
            }
        }

        private bool IsAjaxRequest()
        {
            return HttpContext?.Request?.Headers.XRequestedWith.ToString() == "XMLHttpRequest";
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
                    Courses = await GetCourses(),
                    Payments = await GetPayments(),
                    TruckSpendings = await GetTruckSpendings()
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
                    createNotification.Courses = await GetCourses();
                    createNotification.Payments = await GetPayments();
                    createNotification.TruckSpendings = await GetTruckSpendings();
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
                                                                                                .Select(n => new AdminCreateNotificationViewModel
                                                                                                {
                                                                                                    Title = n.Title,
                                                                                                    Message = n.Message,
                                                                                                    ReceiverId = n.ReceiverID,
                                                                                                    OrderId = n.OrderID,
                                                                                                    TruckId = n.TruckID,
                                                                                                    CourseId = n.CourseID,
                                                                                                    PaymentId = n.PaymentID,
                                                                                                    TruckSpendingId = n.TruckSpendingID,
                                                                                                }).SingleOrDefaultAsync();

                if (createNotification == null)
                {
                    return NotFound();
                }

                createNotification.Receivers = await GetUsers();
                createNotification.Orders = await GetOrders();
                createNotification.Trucks = await GetTrucks();
                createNotification.Courses = await GetCourses();
                createNotification.Payments = await GetPayments();
                createNotification.TruckSpendings = await GetTruckSpendings();

                ViewBag.NotificationId = notificationId;

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
            try
            {
                if (!ModelState.IsValid)
                {
                    createNotification.Receivers = await GetUsers();
                    createNotification.Orders = await GetOrders();
                    createNotification.Trucks = await GetTrucks();
                    createNotification.Courses = await GetCourses();
                    createNotification.Payments = await GetPayments();
                    createNotification.TruckSpendings = await GetTruckSpendings();
                    ViewBag.NotificationId = notificationId;
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
                createNotification.Courses = await GetCourses();
                createNotification.Payments = await GetPayments();
                createNotification.TruckSpendings = await GetTruckSpendings();
                ViewBag.NotificationId = notificationId;
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
                                                                                                    .Include(n => n.Messages)
                                                                                                    .ThenInclude(m => m.Sender)
                                                                                                    .Where(n => n.NotificationID.Equals(notificationID))
                                                                                                    .Select(n => new DriverDetailsNotificationViewModel
                                                                                                    {
                                                                                                        Title = n.Title,
                                                                                                        Message = n.Message,
                                                                                                        CreatedAt = n.CreatedAt,
                                                                                                        IsRead = n.IsRead,
                                                                                                        OrderId = n.OrderID,
                                                                                                        TruckId = n.TruckID,
                                                                                                        CourseId = n.CourseID,
                                                                                                        PaymentId = n.PaymentID,
                                                                                                        TruckSpendingId = n.TruckSpendingID,
                                                                                                        SenderName = n.Sender!.UserName ?? string.Empty,
                                                                                                        isMarkable = n.ReceiverID.Equals(userId),
                                                                                                        IsResponseEnabled = n.CanRespond,
                                                                                                        Messages = n.Messages.Where(m => m.IsDeleted == false)
                                                                                                        .Select(m => new DetailsNotificationMessageViewModel
                                                                                                        {
                                                                                                            MessageID = m.MessageID,
                                                                                                            SenderID = m.SenderID,
                                                                                                            SenderName = m.Sender.UserName ?? string.Empty,
                                                                                                            SentAt = m.SentAt,
                                                                                                            Content = m.Content,
                                                                                                            IsRead = m.IsRead,
                                                                                                        }).OrderBy(m => m.SentAt).ToList()
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

                ViewBag.NotificationID = notificationID;
                ViewBag.CurrentUserId = userId;

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

        private async Task<IDictionary<Guid, string>> GetTruckSpendings()
        {

            return new Dictionary<Guid, string>();
        }

        private async Task<IDictionary<Guid, string>> GetPayments()
        {
            return await _paymentService.GetAll()
                                        .Include(t => t.Order)
                                        .ToDictionaryAsync(
                                                          t => t.PaymentID,
                                                          t => t.Order.OrderID.ToString() + " - " + t.OrderID.ToString()
                                                        );
        }

        private async Task<IDictionary<Guid, string>> GetCourses()
        {
            return await _truckCourseService.GetAll()
                                            .Include(t => t.Truck)
                                            .ToDictionaryAsync(
                                                           t => t.TruckCourseID,
                                                           t => t.TruckCourseID.ToString() + " - " + (t.Truck?.LicensePlate ?? "Not Assigned")
                                                         );
        }

    }
}