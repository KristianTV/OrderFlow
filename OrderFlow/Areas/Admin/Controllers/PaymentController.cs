﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Models;
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePaymentViewModel createPayment, string? orderId)
        {
            if (!ModelState.IsValid)
            {
                return View(createPayment);
            }

            if (string.IsNullOrEmpty(orderId))
            {
                ModelState.AddModelError(string.Empty, "Order ID cannot be null or empty.");
                return View(createPayment);
            }

            if (!Guid.TryParse(orderId, out var parsedOrderId))
            {
                ModelState.AddModelError(string.Empty, "Invalid Order ID format.");
                return View(createPayment);
            }
            await _paymentService.CreatePaymentAsync(createPayment, parsedOrderId);

            return RedirectToAction("Detail", "Order", new { id = orderId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? Id, string? orderId)
        {
            if (string.IsNullOrEmpty(Id))
            {
                ModelState.AddModelError(string.Empty, "Payment ID cannot be null or empty.");
                return View(new CreatePaymentViewModel());
            }

            if (!Guid.TryParse(Id, out var paymentId))
            {
                ModelState.AddModelError(string.Empty, "Invalid Payment ID format.");
                return View(new CreatePaymentViewModel());
            }

            var editPayment = await _paymentService.All<Payment>()
                                                   .AsNoTracking()
                                                   .Where(p => p.Id.Equals(paymentId))
                                                   .Select(p => new CreatePaymentViewModel
                                                   {
                                                       Amount = p.Amount,
                                                       PaymentDescription = p.PaymentDescription
                                                   }).SingleOrDefaultAsync();

            if (editPayment == null)
            {
                ModelState.AddModelError(string.Empty, "Payment not found.");
                return View(new CreatePaymentViewModel());
            }

            return View(editPayment);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CreatePaymentViewModel createPayment, string? Id, string? orderId)
        {
            if (!ModelState.IsValid)
            {
                return View(createPayment);
            }

            if (string.IsNullOrEmpty(Id))
            {
                ModelState.AddModelError(string.Empty, "Payment ID cannot be null or empty.");
                return View(createPayment);
            }
            if (!Guid.TryParse(Id, out var paymentId))
            {
                ModelState.AddModelError(string.Empty, "Invalid Payment ID format.");
                return View(createPayment);
            }

            await _paymentService.UpdatePaymentAsync(paymentId, createPayment);

            return RedirectToAction("Detail", "Order", new { id = orderId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string? Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                ModelState.AddModelError(string.Empty, "Payment ID cannot be null or empty.");
                return RedirectToAction("Index", "Home");
            }
            if (!Guid.TryParse(Id, out var paymentId))
            {
                ModelState.AddModelError(string.Empty, "Invalid Payment ID format.");
                return RedirectToAction("Index", "Home");
            }
            var orderId = await _paymentService.All<Payment>()
                                               .AsNoTracking()
                                               .Where(p => p.Id.Equals(paymentId))
                                               .Select(p => p.OrderID)
                                               .FirstOrDefaultAsync();
            if (orderId == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "Payment not found or does not belong to any order.");
                return RedirectToAction("Index", "Home");
            }

            await _paymentService.DeletePaymentAsync(paymentId);

            return RedirectToAction("Detail", "Order", new { id = orderId });
        }
    }
}
