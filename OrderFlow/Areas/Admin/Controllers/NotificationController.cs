using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services;
using OrderFlow.Services.Contracts;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Message;
using OrderFlow.ViewModels.Notification;

namespace OrderFlow.Areas.Admin.Controllers
{
    public class NotificationController : BaseAdminController
    {
        private const int IndexPageSize = 12;
        private static readonly OrderStatus[] ExcludedOrderStatuses =
        {
            OrderStatus.Completed,
            OrderStatus.Cancelled,
            OrderStatus.Failed
        };

        private readonly ILogger<NotificationController> _logger;
        private readonly INotificationService _notificationService;
        private readonly IOrderService _orderService;
        private readonly ITruckService _truckService;
        private readonly ITruckCourseService _truckCourseService;
        private readonly ITruckSpendingService _truckSpendingService;
        private readonly IPaymentService _paymentService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRealtimeNotifier _realtimeNotifier;
        private readonly IMessageService _messageService;

        public NotificationController(ILogger<NotificationController> logger,
                                        INotificationService notificationService,
                                        IOrderService orderService,
                                        ITruckService truckService,
                                        ITruckCourseService truckCourseService,
                                        ITruckSpendingService truckSpendingService,
                                        IPaymentService paymentService,
                                        UserManager<ApplicationUser> userManager,
                                        IMessageService messageService,
                                        IRealtimeNotifier? realtimeNotifier = null)
        {
            _logger = logger;
            _notificationService = notificationService;
            _orderService = orderService;
            _userManager = userManager;
            _truckService = truckService;
            _truckCourseService = truckCourseService;
            _truckSpendingService = truckSpendingService;
            _paymentService = paymentService;
            _messageService = messageService;
            _realtimeNotifier = realtimeNotifier ?? NullRealtimeNotifier.Instance;
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

                ViewBag.hideSystemNotifications = hideSystemNotifications;
                ViewBag.CurrentSort = string.IsNullOrEmpty(sortBy) ? "All" : char.ToUpper(sortBy[0]) + sortBy.Substring(1).ToLower();
                ViewBag.NotificationArea = "Admin";

                if (IsAjaxRequest())
                {
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
        public async Task<IActionResult> NotificationOptions(Guid? receiverId, Guid? orderId, Guid? truckId, Guid? courseId)
        {
            if (!receiverId.HasValue)
            {
                _logger.LogWarning("Receiver ID is missing when fetching notification options.");
                return Json(new { error = "Receiver ID is required." });
            }

            var receiverRoles = await GetUserRoles(receiverId.Value);
            if (receiverRoles == null)
            {
                _logger.LogWarning("Receiver with ID {ReceiverId} not found when fetching notification options.", receiverId);
                return Json(new { error = "Receiver not found." });
            }

            bool isStaff = receiverRoles.Contains(UserRoles.Admin.ToString()) || receiverRoles.Contains(UserRoles.Speditor.ToString());
            bool isDriver = receiverRoles.Contains(UserRoles.Driver.ToString()) && !isStaff;
            bool isRegularUser = receiverRoles.Contains(UserRoles.User.ToString()) && !isStaff && !isDriver;

            if (!isStaff && !isDriver && !isRegularUser)
            {
                return Json(new { error = "Receiver must have a valid role to fetch notification options." });
            }

            List<NotificationSelectOption>? orders = null;
            List<NotificationSelectOption>? payments = null;
            List<NotificationSelectOption>? trucks = null;
            List<NotificationSelectOption>? courses = null;
            List<NotificationSelectOption>? truckSpendings = null;


            if (isRegularUser || isStaff)
            {
                Guid? filterUserId = isRegularUser ? receiverId : null;
                orders = await FetchOrdersAsync(filterUserId);
            }


            if (orderId.HasValue && orders != null && orders.Any(o => o.Id == orderId.Value))
            {
                payments = await FetchPaymentsAsync(orderId.Value);
            }


            if (isDriver || isStaff)
            {
                Guid? filterDriverId = isDriver ? receiverId : null;
                trucks = await FetchTrucksAsync(filterDriverId);
            }


            if (truckId.HasValue && trucks != null && trucks.Any(t => t.Id == truckId.Value))
            {
                courses = await FetchCoursesAsync(truckId.Value);

                Guid? filterCourseId = (courseId.HasValue && courses.Any(c => c.Id == courseId.Value)) ? courseId : null;
                truckSpendings = await FetchTruckSpendingsAsync(truckId.Value, filterCourseId);
            }


            if (isRegularUser)
            {
                return Json(new
                {
                    orders = BuildOptions(orders),
                    payments = BuildOptions(payments)
                });
            }

            if (isDriver)
            {
                return Json(new
                {
                    trucks = BuildOptions(trucks),
                    courses = BuildOptions(courses),
                    truckSpendings = BuildOptions(truckSpendings)
                });
            }


            return Json(new
            {
                orders = BuildOptions(orders),
                payments = BuildOptions(payments),
                trucks = BuildOptions(trucks),
                courses = BuildOptions(courses),
                truckSpendings = BuildOptions(truckSpendings)
            });
        }


        [HttpGet]
        public async Task<IActionResult> Create(string? orderId)
        {
            try
            {
                var createNotification = new AdminCreateNotificationViewModel
                {
                    Receivers = await GetUsers()
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
                    await RepopulateViewModelListsAsync(createNotification);
                    return View(createNotification);
                }

                if (!Guid.TryParse(this.GetUserId(), out var senderId))
                {
                    _logger.LogWarning("Sender ID is not valid.");
                    ModelState.AddModelError(nameof(senderId), "Sender ID is not valid.");
                    return BadRequest();
                }

                await _notificationService.CreateNotificationAsync(createNotification, senderId);

                await _realtimeNotifier.EntityChangedAsync(new RealtimeEntityChanged
                {
                    Entity = "Notification",
                    Action = "Created",
                    UserIds = new[] { createNotification.ReceiverId },
                    Roles = new[] { UserRoles.Admin.ToString(), UserRoles.Speditor.ToString() }
                });

                await _realtimeNotifier.NotificationCountChangedAsync(createNotification.ReceiverId);

                return RedirectToAction(nameof(Index), "Notification");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a notification.");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");

                await RepopulateViewModelListsAsync(createNotification);
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

                await RepopulateViewModelListsAsync(createNotification);

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
                    await RepopulateViewModelListsAsync(createNotification);
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

                await _realtimeNotifier.EntityChangedAsync(new RealtimeEntityChanged
                {
                    Entity = "Notification",
                    Action = "Updated",
                    Id = notificationId,
                    UserIds = new[] { createNotification.ReceiverId },
                    Roles = new[] { UserRoles.Admin.ToString(), UserRoles.Speditor.ToString() }
                });

                await _realtimeNotifier.NotificationCountChangedAsync(createNotification.ReceiverId);

                return RedirectToAction(nameof(Detail), "Notification", new { id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while editing notification with ID: {NotificationId}", id);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");

                await RepopulateViewModelListsAsync(createNotification);
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

                await _messageService.MarkMessagesAsReadAsync(notificationID, userId);

                if (!notificationViewModel.IsRead && notificationViewModel.isMarkable)
                {
                    await _notificationService.ReadAsync(notificationID);
                    notificationViewModel.IsRead = true;
                    await _realtimeNotifier.NotificationCountChangedAsync(userId);
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
                await _realtimeNotifier.NotificationCountChangedAsync(userId);

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
                await _realtimeNotifier.NotificationCountChangedAsync(userId);

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
                await _realtimeNotifier.EntityChangedAsync(new RealtimeEntityChanged
                {
                    Entity = "Notification",
                    Action = "Deleted",
                    Id = notificationId,
                    Roles = new[] { UserRoles.Admin.ToString(), UserRoles.Speditor.ToString() }
                });

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

        private async Task<List<NotificationSelectOption>> FetchOrdersAsync(Guid? userId)
        {
            var query = _orderService.GetAll().AsNoTracking()
                .Where(o => !ExcludedOrderStatuses.Contains(o.Status));

            if (userId.HasValue)
                query = query.Where(o => o.UserID == userId.Value);

            return await query
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new NotificationSelectOption(o.OrderID, $"{o.OrderID} - {o.User.UserName}"))
                .ToListAsync();
        }

        private async Task<List<NotificationSelectOption>> FetchPaymentsAsync(Guid orderId)
        {
            return await _paymentService.GetAll().AsNoTracking()
                .Where(p => p.OrderID == orderId)
                .OrderByDescending(p => p.CreatedOn)
                .Select(p => new NotificationSelectOption(p.PaymentID, $"{p.PaymentID} - {p.Amount.ToString("C")}"))
                .ToListAsync();
        }

        private async Task<List<NotificationSelectOption>> FetchTrucksAsync(Guid? driverId)
        {
            var query = _truckService.GetAll().AsNoTracking();

            if (driverId.HasValue)
                query = query.Where(t => t.DriverID == driverId.Value);

            return await query
                .OrderByDescending(t => t.LicensePlate)
                .Select(t => new NotificationSelectOption(t.TruckID, $"{t.TruckID} - {t.LicensePlate}"))
                .ToListAsync();
        }

        private async Task<List<NotificationSelectOption>> FetchCoursesAsync(Guid truckId)
        {
            return await _truckCourseService.GetAll().AsNoTracking()
                .Where(c => c.TruckID == truckId)
                .OrderByDescending(c => c.AssignedDate)
                .Select(c => new NotificationSelectOption(c.TruckCourseID, $"{c.TruckCourseID} - {c.AssignedDate.ToString("g")}"))
                .ToListAsync();
        }

        private async Task<List<NotificationSelectOption>> FetchTruckSpendingsAsync(Guid truckId, Guid? courseId)
        {
            var query = _truckSpendingService.GetAll().AsNoTracking()
                .Where(s => s.TruckID == truckId);

            if (courseId.HasValue)
                query = query.Where(s => s.TruckCourseID == courseId.Value);

            return await query
                .OrderByDescending(s => s.PaymentDate)
                .Select(s => new NotificationSelectOption(s.TruckSpendingID, $"{s.TruckSpendingID} - {s.Amount.ToString("C")}"))
                .ToListAsync();
        }

        private async Task<HashSet<string>?> GetUserRoles(Guid? userId)
        {
            if (!userId.HasValue)
            {
                return null;
            }

            ApplicationUser? user = await _userManager.FindByIdAsync(userId.Value.ToString());
            if (user == null)
            {
                return null;
            }

            return (await _userManager.GetRolesAsync(user)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        private static object? BuildOptions(IEnumerable<NotificationSelectOption>? options)
        {
            if (options == null)
                return null;

            return new
            {
                all = options
                    .OrderBy(o => o.Text)
                    .Select(o => new { id = o.Id, text = o.Text })
            };
        }

        private sealed record NotificationSelectOption(Guid Id, string Text);

        private async Task RepopulateViewModelListsAsync(AdminCreateNotificationViewModel model)
        {

            model.Receivers = await GetUsers();


            model.Orders = (await FetchOrdersAsync(userId: null)).ToDictionary(o => o.Id, o => o.Text);
            model.Trucks = (await FetchTrucksAsync(driverId: null)).ToDictionary(t => t.Id, t => t.Text);

            if (model.OrderId.HasValue)
            {
                model.Payments = (await FetchPaymentsAsync(model.OrderId.Value)).ToDictionary(p => p.Id, p => p.Text);
            }
            else
            {
                model.Payments = new Dictionary<Guid, string>();
            }

            if (model.TruckId.HasValue)
            {
                model.Courses = (await FetchCoursesAsync(model.TruckId.Value)).ToDictionary(c => c.Id, c => c.Text);

                model.TruckSpendings = (await FetchTruckSpendingsAsync(model.TruckId.Value, model.CourseId)).ToDictionary(s => s.Id, s => s.Text);
            }
            else
            {
                model.Courses = new Dictionary<Guid, string>();
                model.TruckSpendings = new Dictionary<Guid, string>();
            }
        }

    }
}
