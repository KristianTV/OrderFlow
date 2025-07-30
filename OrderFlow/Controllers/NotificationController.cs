using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Models;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Notification;
using System.Threading.Tasks;

namespace OrderFlow.Controllers
{
    public class NotificationController : BaseController
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
        public async Task<IActionResult> Index()
        {
            if (!Guid.TryParse(this.GetUserId(), out var userId))
            {
                _logger.LogWarning("User ID is not valid.");
                return BadRequest("Invalid user ID.");
            }

            var notifications = await _notificationService.GetAllNotificationsForUserAsync(userId);

            if (notifications == null)
            {
                _logger.LogWarning("No notifications found for user with ID: {UserId}", userId);
                return NotFound("No notifications found.");
            }

            return View(notifications);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            Dictionary<Guid, string> orders = await _orderService.All<Order>()
                                                              .ToDictionaryAsync(
                                                                                order => order.OrderID,
                                                                                order => order.OrderID.ToString()
                                                                            );

            Dictionary<Guid, string> recivers = await _userManager.Users.ToDictionaryAsync(
                                                                                            user => user.Id,
                                                                                            user => user.UserName
                                                                                           );

            CreateNotificationViewModel createNotification = new CreateNotificationViewModel
            {
                Receivers = recivers,
                Orders = orders,
            };

            return View(createNotification);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateNotificationViewModel createPayment)
        {
            if (!ModelState.IsValid)
            {
                return View(createPayment);
            }

            if(!Guid.TryParse(this.GetUserId(), out var senderId))
            {
                _logger.LogWarning("Sender ID is not valid.");
                return BadRequest("Invalid sender ID.");
            }

            await _notificationService.CreateNotificationAsync(createPayment, senderId);

            return RedirectToAction(nameof(Index), "Notification");
        }
    }
}
