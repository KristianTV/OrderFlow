using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Payment;

namespace OrderFlow.Areas.Admin.Controllers
{
    public class PaymentController : BaseAdminController
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IPaymentService _paymentService;

        public PaymentController(ILogger<PaymentController> logger, IPaymentService paymentService)
        {
            _logger = logger;
            _paymentService = paymentService;
        }

        [HttpGet]
        public IActionResult Create(string? orderId)
        {
            if (string.IsNullOrEmpty(orderId) || !Guid.TryParse(orderId, out _))
            {
                TempData["Error"] = "Invalid or missing Order ID for payment creation.";
                _logger.LogWarning("Create Payment GET: Invalid or missing Order ID '{OrderId}'", orderId);
                return RedirectToAction("Index", "Order"); 
            }

            ViewData["OrderId"] = orderId;
            return View(new CreatePaymentViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePaymentViewModel createPayment, string? orderId)
        {
            if (!ModelState.IsValid)
            {
                ViewData["OrderId"] = orderId;
                return View(createPayment);
            }

            if (string.IsNullOrEmpty(orderId))
            {
                ModelState.AddModelError(string.Empty, "Order ID cannot be null or empty.");
                _logger.LogWarning("Create Payment POST: Order ID was null or empty.");
                ViewData["OrderId"] = orderId;
                return View(createPayment);
            }

            if (!Guid.TryParse(orderId, out var parsedOrderId))
            {
                ModelState.AddModelError(string.Empty, "Invalid Order ID format.");
                _logger.LogWarning("Create Payment POST: Invalid Order ID format '{OrderId}'.", orderId);
                ViewData["OrderId"] = orderId;
                return View(createPayment);
            }

            try
            {
                bool success = await _paymentService.CreatePaymentAsync(createPayment, parsedOrderId);

                if (success)
                {
                    TempData["Success"] = "Payment successfully created.";
                    _logger.LogInformation("Payment created for Order ID: {OrderId}", parsedOrderId);
                    return RedirectToAction("Detail", "Order", new { id = orderId });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to create payment. Please check the order details and try again.");
                    _logger.LogError("Create Payment POST: Service failed to create payment for Order ID: {OrderId}", parsedOrderId);
                    ViewData["OrderId"] = orderId;
                    return View(createPayment);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a payment for Order ID: {OrderId}", parsedOrderId);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while creating the payment. Please try again.");
                ViewData["OrderId"] = orderId;
                return View(createPayment);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? Id, string? orderId)
        {
            if (string.IsNullOrEmpty(Id) || !Guid.TryParse(Id, out var paymentId))
            {
                TempData["Error"] = "Invalid or missing Payment ID for editing.";
                _logger.LogWarning("Edit Payment GET: Invalid or missing Payment ID '{PaymentId}'", Id);
                return RedirectToAction("Index", "Order"); 
            }

            if (string.IsNullOrEmpty(orderId) || !Guid.TryParse(orderId, out _))
            {
                TempData["Error"] = "Invalid or missing Order ID for payment editing.";
                _logger.LogWarning("Edit Payment GET: Invalid or missing Order ID '{OrderId}' for Payment ID '{PaymentId}'", orderId, Id);
                return RedirectToAction("Index", "Order");
            }

            CreatePaymentViewModel? editPayment = null;
            try
            {
                editPayment = await _paymentService.GetAll()
                                                   .AsNoTracking()
                                                   .Where(p => p.Id.Equals(paymentId))
                                                   .Select(p => new CreatePaymentViewModel
                                                   {
                                                       Amount = p.Amount,
                                                       PaymentDescription = p.PaymentDescription
                                                   }).SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving payment with ID {PaymentId} for editing.", paymentId);
                TempData["Error"] = "An unexpected error occurred while retrieving payment details. Please try again.";
                return RedirectToAction("Detail", "Order", new { id = orderId });
            }

            if (editPayment == null)
            {
                TempData["Error"] = $"Payment with ID '{paymentId}' not found.";
                _logger.LogWarning("Edit Payment GET: Payment with ID '{PaymentId}' not found.", paymentId);
                return RedirectToAction("Detail", "Order", new { id = orderId });
            }

            ViewData["OrderId"] = orderId; 
            return View(editPayment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreatePaymentViewModel createPayment, string? Id, string? orderId)
        {
            if (!ModelState.IsValid)
            {
                ViewData["OrderId"] = orderId;
                return View(createPayment);
            }

            if (string.IsNullOrEmpty(Id) || !Guid.TryParse(Id, out var paymentId))
            {
                ModelState.AddModelError(string.Empty, "Payment ID cannot be null or empty or invalid.");
                _logger.LogWarning("Edit Payment POST: Invalid or missing Payment ID '{PaymentId}'.", Id);
                ViewData["OrderId"] = orderId;
                return View(createPayment);
            }

            if (string.IsNullOrEmpty(orderId) || !Guid.TryParse(orderId, out _))
            {
                ModelState.AddModelError(string.Empty, "Order ID cannot be null or empty or invalid.");
                _logger.LogWarning("Edit Payment POST: Invalid or missing Order ID '{OrderId}' for Payment ID '{PaymentId}'.", orderId, Id);
                ViewData["OrderId"] = orderId;
                return View(createPayment);
            }

            try
            {
                bool success = await _paymentService.UpdatePaymentAsync(paymentId, createPayment);

                if (success)
                {
                    TempData["Success"] = "Payment successfully updated.";
                    _logger.LogInformation("Payment ID: {PaymentId} updated for Order ID: {OrderId}", paymentId, orderId);
                    return RedirectToAction("Detail", "Order", new { id = orderId });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to update payment. The payment might not exist or the data is invalid.");
                    _logger.LogError("Edit Payment POST: Service failed to update payment ID: {PaymentId} for Order ID: {OrderId}", paymentId, orderId);
                    ViewData["OrderId"] = orderId;
                    return View(createPayment);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating payment with ID {PaymentId} for Order ID {OrderId}.", paymentId, orderId);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while updating the payment. Please try again.");
                ViewData["OrderId"] = orderId;
                return View(createPayment);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string? Id)
        {
            if (string.IsNullOrEmpty(Id) || !Guid.TryParse(Id, out var paymentId))
            {
                TempData["Error"] = "Invalid or missing Payment ID for deletion.";
                _logger.LogWarning("Delete Payment POST: Invalid or missing Payment ID '{PaymentId}'.", Id);
                return RedirectToAction("Index", "Order");
            }

            Guid orderId = Guid.Empty;
            try
            {
                orderId = await _paymentService.GetAll()
                                               .AsNoTracking()
                                               .Where(p => p.Id.Equals(paymentId))
                                               .Select(p => p.OrderID)
                                               .FirstOrDefaultAsync();

                if (orderId == Guid.Empty)
                {
                    TempData["Error"] = $"Payment with ID '{paymentId}' not found or does not belong to any order.";
                    _logger.LogWarning("Delete Payment POST: Payment with ID '{PaymentId}' not found or has no associated order.", paymentId);
                    return RedirectToAction("Index", "Order");
                }

                bool success = await _paymentService.DeletePaymentAsync(paymentId);

                if (success)
                {
                    TempData["Success"] = "Payment successfully deleted.";
                    _logger.LogInformation("Payment ID: {PaymentId} deleted from Order ID: {OrderId}", paymentId, orderId);
                    return RedirectToAction("Detail", "Order", new { id = orderId });
                }
                else
                {
                    TempData["Error"] = "Failed to delete payment. It might not exist or an error occurred in the service.";
                    _logger.LogError("Delete Payment POST: Service failed to delete payment ID: {PaymentId}", paymentId);
                    return RedirectToAction("Detail", "Order", new { id = orderId });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting payment with ID {PaymentId}.", paymentId);
                TempData["Error"] = "An unexpected error occurred while deleting the payment. Please try again.";
                return RedirectToAction("Detail", "Order", new { id = orderId != Guid.Empty ? orderId.ToString() : null });
            }
        }
    }
}