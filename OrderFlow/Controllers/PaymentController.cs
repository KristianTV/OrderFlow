using Microsoft.AspNetCore.Mvc;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Helpers;
using OrderFlow.Services;
using OrderFlow.Services.Contracts;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Payment;

namespace OrderFlow.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IPaymentService _paymentService;
        private readonly IRealtimeNotifier _realtimeNotifier;
        private readonly IMailService _mailService;
        private readonly INotificationService _notificationService;


        public PaymentController(ILogger<PaymentController> logger, IPaymentService paymentService, IMailService mailService, INotificationService notificationService, IRealtimeNotifier? realtimeNotifier = null)
        {
            _logger = logger;
            _paymentService = paymentService;
            _mailService = mailService;
            _notificationService = notificationService;
            _realtimeNotifier = realtimeNotifier ?? NullRealtimeNotifier.Instance;
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string? id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var paymentId))
            {
                TempData["Error"] = "Invalid or missing Payment ID for detail.";
                _logger.LogWarning("Detail Payment GET: Invalid or missing Payment ID '{PaymentId}'", id);
                return NotFound();
            }

            Guid? orderId = await _paymentService.GetOrderIdByPaymentIdAsync(paymentId);

            if (orderId == null || orderId.Equals(Guid.Empty))
            {
                TempData["Error"] = "Invalid or missing Order ID for detail.";
                _logger.LogWarning("Detail Payment GET: Invalid or missing Order ID");
                return NotFound();
            }

            return RedirectToAction("Detail", "Order", new { id = orderId });
        }

        [HttpGet]
        public async Task<IActionResult> Card(string? orderId)
        {
            if (string.IsNullOrEmpty(orderId) || !Guid.TryParse(orderId, out Guid parsedOrderId))
            {
                TempData["Error"] = "Invalid or missing order ID for card payment.";
                return RedirectToAction("Index", "Order");
            }

            if (!Guid.TryParse(this.GetUserId(), out Guid userId))
            {
                _logger.LogWarning("Invalid user ID format received during card payment.");
                return BadRequest();
            }

            CardPaymentViewModel? cardPayment = await _paymentService.GetCardPaymentAsync(parsedOrderId, userId);

            if (cardPayment == null)
            {
                TempData["Error"] = "Card payment is available only for delivered orders with payments.";
                return RedirectToAction("Detail", "Order", new { id = parsedOrderId });
            }

            return View(cardPayment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Card(CardPaymentViewModel cardPayment)
        {
            if (!Guid.TryParse(this.GetUserId(), out Guid userId))
            {
                _logger.LogWarning("Invalid user ID format received during card payment.");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(cardPayment);
            }

            try
            {
                bool success = await _paymentService.PayOrderByCardAsync(cardPayment, userId);

                if (!success)
                {
                    CardPaymentViewModel? paymentFromDb = await _paymentService.GetCardPaymentAsync(cardPayment.OrderId, userId);
                    if (paymentFromDb != null)
                    {
                        cardPayment.Amount = paymentFromDb.Amount;
                    }


                    ModelState.AddModelError(string.Empty, "Payment could not be completed. Check the card details, expiry date, order status, amount, and demo result.");

                    string? userEmail = this.GetUserEmail();
                    if (!string.IsNullOrEmpty(userEmail))
                    {
                        var privacyPolicyUrl = Url.Action("Privacy", "", new { }, protocol: HttpContext.Request.Scheme) ?? "#";

                        await _mailService.SendMailAsync(userEmail,
                                                         "Card Payment Failed",
                                                         EmailTemplates.PaymentFailed(this.GetUserName() ?? "User",
                                                                                      cardPayment.OrderId.ToString(),
                                                                                      cardPayment.Amount.ToString("C"),
                                                                                      "Payment could not be completed. Check the card details, expiry date, order status, amount, and demo result.",
                                                                                      privacyPolicyUrl));
                    }

                    await _notificationService.SendSystemNotificationAsync(new Services.Core.Commands.NotificationCommand
                    {
                        Title = "Card Payment Failed",
                        Message = $"Your card payment attempt for order {cardPayment.OrderId} was unsuccessful. Please check your card details and try again.",
                        ReceiverID = userId,
                        OrderID = cardPayment.OrderId,
                        CanRespond = false
                    });

                    return View(cardPayment);
                }

                TempData["Success"] = "Card payment completed successfully.";
                await _realtimeNotifier.EntityChangedAsync(new RealtimeEntityChanged
                {
                    Entity = "Payment",
                    Action = "Paid",
                    RelatedId = cardPayment.OrderId,
                    UserIds = new[] { userId },
                    Roles = new[] { UserRoles.Admin.ToString(), UserRoles.Speditor.ToString() }
                });

                await _notificationService.SendSystemNotificationAsync(new Services.Core.Commands.NotificationCommand
                {
                    Title = "Card Payment Successful",
                    Message = $"Your card payment for order {cardPayment.OrderId} was successful. Thank you for your payment!",
                    ReceiverID = userId,
                    OrderID = cardPayment.OrderId,
                    CanRespond = false
                });

                await _mailService.SendMailAsync(this.GetUserEmail() ?? string.Empty,
                                                 "Card Payment Successful",
                                                 EmailTemplates.PaymentSucceeded(this.GetUserName() ?? "User",
                                                                                 cardPayment.OrderId.ToString(),
                                                                                 DateTime.UtcNow.ToString("f"),
                                                                                 cardPayment.Amount.ToString("C"),
                                                                                 "Card",
                                                                                 Url.Action("Detail", "Order", new { id = cardPayment.OrderId }, protocol: HttpContext.Request.Scheme) ?? "#",
                                                                                 Url.Action("Privacy", "", new { }, protocol: HttpContext.Request.Scheme) ?? "#"));

                return RedirectToAction("Detail", "Order", new { id = cardPayment.OrderId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing card payment for order {OrderId}.", cardPayment.OrderId);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while processing the payment.");
                return View(cardPayment);
            }
        }
    }
}
