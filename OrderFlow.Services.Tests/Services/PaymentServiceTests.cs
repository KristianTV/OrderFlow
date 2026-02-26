using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Services.Core;
using OrderFlow.ViewModels.Payment;

namespace OrderFlow.Tests.Services
{
    [TestFixture]
    public class PaymentServiceTests
    {
        private OrderFlowDbContext _context;
        private PaymentService _paymentService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<OrderFlowDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new OrderFlowDbContext(options);
            _paymentService = new PaymentService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void GetAll_ReturnsAllPayments()
        {
            _context.Payments.Add(new Payment { PaymentID = Guid.NewGuid(), Amount = 10.00m, OrderID = Guid.NewGuid(), PaymentDate = DateTime.UtcNow });
            _context.Payments.Add(new Payment { PaymentID = Guid.NewGuid(), Amount = 20.00m, OrderID = Guid.NewGuid(), PaymentDate = DateTime.UtcNow });
            _context.SaveChanges();

            var result = _paymentService.GetAll();

            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetAll_ReturnsEmptyCollection_WhenNoPaymentsExist()
        {
            var result = _paymentService.GetAll();

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task CreatePaymentAsync_AddsNewPayment()
        {
            var createPaymentViewModel = new CreatePaymentViewModel
            {
                Amount = 50.00m,
                PaymentDescription = "Test Payment"
            };
            var orderId = Guid.NewGuid();

            bool success = await _paymentService.CreatePaymentAsync(createPaymentViewModel, orderId);

            var payment = await _context.Payments.SingleOrDefaultAsync(p => p.OrderID == orderId);
            Assert.IsTrue(success);
            Assert.IsNotNull(payment);
            Assert.That(payment.Amount, Is.EqualTo(50.00m));
            Assert.That(payment.PaymentDescription, Is.EqualTo("Test Payment"));
            Assert.That(payment.OrderID, Is.EqualTo(orderId));
            Assert.That(payment.PaymentDate, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(5)));
        }

        [Test]
        public async Task DeletePaymentAsync_DeletesExistingPayment()
        {
            var paymentId = Guid.NewGuid();
            _context.Payments.Add(new Payment { PaymentID = paymentId, Amount = 100.00m, OrderID = Guid.NewGuid(), PaymentDate = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            bool success = await _paymentService.DeletePaymentAsync(paymentId);

            var payment = await _context.Payments.FindAsync(paymentId);
            Assert.IsTrue(success);
            Assert.IsNull(payment);
        }

        [Test]
        public void DeletePaymentAsync_ThrowsKeyNotFoundException_WhenPaymentDoesNotExist()
        {
            var nonExistentId = Guid.NewGuid();

            Assert.ThrowsAsync<KeyNotFoundException>(() => _paymentService.DeletePaymentAsync(nonExistentId));
        }

        [Test]
        public void DeletePaymentAsync_ThrowsArgumentException_WhenPaymentIdIsEmpty()
        {
            var emptyId = Guid.Empty;

            Assert.ThrowsAsync<ArgumentException>(() => _paymentService.DeletePaymentAsync(emptyId));
        }

        [Test]
        public async Task UpdatePaymentAsync_UpdatesExistingPayment()
        {
            var paymentId = Guid.NewGuid();
            _context.Payments.Add(new Payment { PaymentID = paymentId, Amount = 150.00m, PaymentDescription = "Old Description", OrderID = Guid.NewGuid(), PaymentDate = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            var updatedPaymentViewModel = new CreatePaymentViewModel
            {
                Amount = 200.00m,
                PaymentDescription = "New Description"
            };

            bool success = await _paymentService.UpdatePaymentAsync(paymentId, updatedPaymentViewModel);

            var updatedPayment = await _context.Payments.FindAsync(paymentId);
            Assert.IsTrue(success);
            Assert.IsNotNull(updatedPayment);
            Assert.AreEqual(200.00m, updatedPayment.Amount);
            Assert.AreEqual("New Description", updatedPayment.PaymentDescription);
        }

        [Test]
        public void UpdatePaymentAsync_ThrowsKeyNotFoundException_WhenPaymentDoesNotExist()
        {
            var nonExistentId = Guid.NewGuid();
            var updatedPaymentViewModel = new CreatePaymentViewModel { Amount = 10.00m, PaymentDescription = "Test" };

            Assert.ThrowsAsync<KeyNotFoundException>(() => _paymentService.UpdatePaymentAsync(nonExistentId, updatedPaymentViewModel));
        }

        [Test]
        public void UpdatePaymentAsync_ThrowsArgumentException_WhenPaymentIdIsEmpty()
        {
            var emptyId = Guid.Empty;
            var updatedPaymentViewModel = new CreatePaymentViewModel { Amount = 10.00m, PaymentDescription = "Test" };

            Assert.ThrowsAsync<ArgumentException>(() => _paymentService.UpdatePaymentAsync(emptyId, updatedPaymentViewModel));
        }

        [Test]
        public void UpdatePaymentAsync_ThrowsArgumentNullException_WhenViewModelIsNull()
        {
            var paymentId = Guid.NewGuid();

            Assert.ThrowsAsync<ArgumentNullException>(() => _paymentService.UpdatePaymentAsync(paymentId, null));
        }
    }
}