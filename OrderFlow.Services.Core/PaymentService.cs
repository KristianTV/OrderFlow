using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Data.Repository;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Payment;

namespace OrderFlow.Services.Core
{
    public class PaymentService : BaseRepository, IPaymentService
    {
        public PaymentService(OrderFlowDbContext _context) : base(_context)
        {
        }

        public IQueryable<Payment> GetAll()
        {
            return All<Payment>().AsQueryable();
        }

        public async Task<bool> CreatePaymentAsync(CreatePaymentViewModel createPayment, Guid orderId)
        {
            if (createPayment == null || orderId == Guid.Empty)
            {
                return false;
            }

            await AddAsync(new Payment
            {
                Amount = createPayment.Amount,
                PaymentDescription = createPayment.PaymentDescription ?? string.Empty,
                OrderID = orderId,
                CreatedOn = DateTime.UtcNow,
            });

            return await SaveChangesAsync() > 0;
        }

        public async Task<CardPaymentViewModel?> GetCardPaymentAsync(Guid orderId, Guid userId)
        {
            if (orderId == Guid.Empty || userId == Guid.Empty)
            {
                return null;
            }

            List<Payment> payments = await this.GetAll()
                                               .AsNoTracking()
                                               .Include(p => p.Order)
                                               .Where(p => p.OrderID == orderId && p.Order.UserID == userId && p.PaymentDate == null)
                                               .ToListAsync();

            if (payments == null || !payments.Any())
            {
                return null;
            }

            return new CardPaymentViewModel
            {
                OrderId = payments.First().OrderID,
                Amount = payments.Sum(p => p.Amount),
                PaymentSuccess = true
            };
        }

        public async Task<bool> PayOrderByCardAsync(CardPaymentViewModel cardPayment, Guid userId)
        {
            if (cardPayment == null || cardPayment.OrderId == Guid.Empty || userId == Guid.Empty)
            {
                return false;
            }

            if (!cardPayment.PaymentSuccess ||
                !ValidateCreditCardNumber(cardPayment.CardNumber) ||
                !IsValidExpiryDate(cardPayment.ExpiryDate))
            {
                return false;
            }

            List<Payment> payments = await this.GetAll()
                                               .AsNoTracking()
                                               .Include(p => p.Order)
                                               .Where(p => p.OrderID == cardPayment.OrderId && p.Order.UserID == userId && p.PaymentDate == null)
                                               .ToListAsync();

            if (payments == null || payments.First().Order.Status != OrderStatus.Completed || !payments.Any())
            {
                return false;
            }

            decimal dbAmount = payments.Sum(p => p.Amount);

            if (cardPayment.Amount != dbAmount)
            {
                return false;
            }

            DateTime paidOn = DateTime.UtcNow;

            foreach (Payment payment in payments)
            {
                payment.PaymentMethod = PaymentMethods.Card;
                payment.PaymentDate = paidOn;
            }

            return await SaveChangesAsync() > 0;
        }

        public async Task<bool> MarkPaymentAsCashAsync(Guid paymentId)
        {
            if (paymentId == Guid.Empty)
            {
                throw new ArgumentException("Payment ID cannot be empty.", nameof(paymentId));
            }

            Payment? payment = await this.GetAll().Where(p => p.PaymentID == paymentId && p.PaymentDate == null).SingleOrDefaultAsync();

            if (payment == null)
            {
                throw new KeyNotFoundException($"Payment with ID {paymentId} was not found.");
            }


            payment.PaymentMethod = PaymentMethods.Cash;
            payment.PaymentDate = DateTime.UtcNow;

            return await SaveChangesAsync() > 0;
        }

        public async Task<bool> MarkOrderPaymentsAsCashAsync(Guid orderId)
        {
            if (orderId == Guid.Empty)
            {
                throw new ArgumentException("Order ID cannot be empty.", nameof(orderId));
            }

            List<Payment> payments = await this.GetAll().Where(p => p.OrderID == orderId && p.PaymentDate == null).ToListAsync();

            if (!payments.Any())
            {
                throw new KeyNotFoundException($"Payments for order ID {orderId} were not found.");
            }

            DateTime paidOn = DateTime.UtcNow;
            foreach (Payment payment in payments)
            {
                payment.PaymentMethod = PaymentMethods.Cash;
                payment.PaymentDate = paidOn;
            }

            return await SaveChangesAsync() > 0;
        }

        public async Task<bool> DeletePaymentAsync(Guid paymentId)
        {
            if (paymentId == Guid.Empty)
            {
                throw new ArgumentException("Payment ID cannot be empty.", nameof(paymentId));
            }

            var payment = await this.GetAll().SingleOrDefaultAsync(p => p.PaymentID == paymentId);

            if (payment == null)
            {
                throw new KeyNotFoundException($"Payment with ID {paymentId} not found.");
            }

            Delete(payment);

            return await SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdatePaymentAsync(Guid paymentId, CreatePaymentViewModel createPayment)
        {
            if (paymentId == Guid.Empty)
            {
                throw new ArgumentException("Payment ID cannot be empty.", nameof(paymentId));
            }
            if (createPayment == null)
            {
                throw new ArgumentNullException(nameof(createPayment), "CreatePaymentViewModel cannot be null.");
            }

            var payment = await this.GetAll().SingleOrDefaultAsync(p => p.PaymentID == paymentId);

            if (payment == null)
            {
                throw new KeyNotFoundException($"Payment with ID {paymentId} not found.");
            }
            payment.Amount = createPayment.Amount;
            payment.PaymentDescription = createPayment.PaymentDescription ?? string.Empty;

            return await SaveChangesAsync() > 0;

        }

        private static bool ValidateCreditCardNumber(string? cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
            {
                return false;
            }

            string normalizedCardNumber = cardNumber.Replace(" ", string.Empty).Replace("-", string.Empty);
            if (!normalizedCardNumber.All(char.IsDigit))
            {
                return false;
            }

            string[] cardPatterns =
            {
                @"^4[0-9]{12}(?:[0-9]{3})?$",
                @"^5[1-5][0-9]{14}$",
                @"^3[47][0-9]{13}$",
                @"^6(?:011|5[0-9]{2})[0-9]{12}$"
            };

            return cardPatterns.Any(pattern => System.Text.RegularExpressions.Regex.IsMatch(normalizedCardNumber, pattern));
        }

        private static bool IsValidExpiryDate(string? expiryDate)
        {
            if (string.IsNullOrWhiteSpace(expiryDate) ||
                !System.Text.RegularExpressions.Regex.IsMatch(expiryDate, @"^(0[1-9]|1[0-2])\/\d{2}$"))
            {
                return false;
            }

            int month = int.Parse(expiryDate[..2]);
            int year = 2000 + int.Parse(expiryDate[^2..]);
            DateTime expiresAt = new DateTime(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59, DateTimeKind.Utc);

            return expiresAt >= DateTime.UtcNow;
        }
    }
}
