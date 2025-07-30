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
        public IActionResult Create(string? orderId)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePaymentViewModel createPayment, string? orderId)
        {
            if (!ModelState.IsValid)
            {
                return View(createPayment);
            }

            if(string.IsNullOrEmpty(orderId))
            {
                ModelState.AddModelError(string.Empty, "Order ID cannot be null or empty.");
                return View(createPayment);
            }

            if(!Guid.TryParse(orderId, out var parsedOrderId))
            {
                ModelState.AddModelError(string.Empty, "Invalid Order ID format.");
                return View(createPayment);
            }
            await _paymentService.CreatePaymentAsync(createPayment, parsedOrderId);

            return RedirectToAction("Detail", "Order", new { id = orderId });
        }
    }
}
