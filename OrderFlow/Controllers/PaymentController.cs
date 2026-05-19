using Microsoft.AspNetCore.Mvc;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Payment;

namespace OrderFlow.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IPaymentService _paymentService;

        public PaymentController(ILogger<PaymentController> logger, IPaymentService paymentService)
        {
            _logger = logger;
            _paymentService = paymentService;
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
                    return View(cardPayment);
                }

                TempData["Success"] = "Card payment completed successfully.";
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
