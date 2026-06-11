using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services.Core;
using OrderFlow.ViewModels.Payment;

namespace OrderFlow.Tests.Services
{
    [TestFixture]
    public class PaymentServiceTests
    {
        private OrderFlowDbContext _context = null!;
        private PaymentService _paymentService = null!;

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
            Assert.That(payment!.Amount, Is.EqualTo(50.00m));
            Assert.That(payment.PaymentDescription, Is.EqualTo("Test Payment"));
            Assert.That(payment.OrderID, Is.EqualTo(orderId));
            Assert.That(payment.CreatedOn, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(5)));
        }

        [Test]
        public async Task CreatePaymentAsync_ReturnsFalse_WhenViewModelIsNullOrOrderIdIsEmpty()
        {
            bool nullModelResult = await _paymentService.CreatePaymentAsync(null!, Guid.NewGuid());
            bool emptyOrderResult = await _paymentService.CreatePaymentAsync(new CreatePaymentViewModel { Amount = 25m }, Guid.Empty);

            Assert.That(nullModelResult, Is.False);
            Assert.That(emptyOrderResult, Is.False);
            Assert.That(_context.Payments, Is.Empty);
        }

        [Test]
        public async Task CreatePaymentAsync_StoresEmptyDescription_WhenDescriptionIsNull()
        {
            var orderId = Guid.NewGuid();

            bool result = await _paymentService.CreatePaymentAsync(new CreatePaymentViewModel { Amount = 15m, PaymentDescription = null }, orderId);

            var payment = await _context.Payments.SingleAsync();
            Assert.That(result, Is.True);
            Assert.That(payment.PaymentDescription, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task GetCardPaymentAsync_ReturnsOnlyUnpaidPaymentsForOrderOwner()
        {
            var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "Customer" };
            var otherUser = new ApplicationUser { Id = Guid.NewGuid(), UserName = "Other" };
            var order = new Order { OrderID = Guid.NewGuid(), UserID = user.Id, User = user, Status = OrderStatus.Completed };
            var otherOrder = new Order { OrderID = Guid.NewGuid(), UserID = otherUser.Id, User = otherUser, Status = OrderStatus.Completed };
            await _context.Users.AddRangeAsync(user, otherUser);
            await _context.Orders.AddRangeAsync(order, otherOrder);
            await _context.Payments.AddRangeAsync(
                new Payment { PaymentID = Guid.NewGuid(), OrderID = order.OrderID, Order = order, Amount = 10m, CreatedOn = DateTime.UtcNow },
                new Payment { PaymentID = Guid.NewGuid(), OrderID = order.OrderID, Order = order, Amount = 20m, CreatedOn = DateTime.UtcNow, PaymentDate = DateTime.UtcNow },
                new Payment { PaymentID = Guid.NewGuid(), OrderID = otherOrder.OrderID, Order = otherOrder, Amount = 30m, CreatedOn = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            var result = await _paymentService.GetCardPaymentAsync(order.OrderID, user.Id);
            var unauthorizedResult = await _paymentService.GetCardPaymentAsync(order.OrderID, otherUser.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.OrderId, Is.EqualTo(order.OrderID));
            Assert.That(result.Amount, Is.EqualTo(10m));
            Assert.That(result.PaymentSuccess, Is.True);
            Assert.That(unauthorizedResult, Is.Null);
        }

        [Test]
        public async Task PayOrderByCardAsync_MarksAllUnpaidPayments_WhenCardDataAndAmountAreValid()
        {
            var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "Customer" };
            var order = new Order { OrderID = Guid.NewGuid(), UserID = user.Id, User = user, Status = OrderStatus.Completed };
            await _context.Users.AddAsync(user);
            await _context.Orders.AddAsync(order);
            await _context.Payments.AddRangeAsync(
                new Payment { PaymentID = Guid.NewGuid(), OrderID = order.OrderID, Order = order, Amount = 10m, CreatedOn = DateTime.UtcNow },
                new Payment { PaymentID = Guid.NewGuid(), OrderID = order.OrderID, Order = order, Amount = 15m, CreatedOn = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            bool result = await _paymentService.PayOrderByCardAsync(new CardPaymentViewModel
            {
                OrderId = order.OrderID,
                Amount = 25m,
                PaymentSuccess = true,
                CardNumber = "4111 1111 1111 1111",
                ExpiryDate = DateTime.UtcNow.AddYears(1).ToString("MM'/'yy", System.Globalization.CultureInfo.InvariantCulture)
            }, user.Id);

            var payments = await _context.Payments.Where(p => p.OrderID == order.OrderID).ToListAsync();
            Assert.That(result, Is.True);
            Assert.That(payments, Has.All.Matches<Payment>(p => p.PaymentMethod == PaymentMethods.Card));
            Assert.That(payments, Has.All.Matches<Payment>(p => p.PaymentDate != null));
        }

        [Test]
        public async Task PayOrderByCardAsync_ReturnsFalse_ForValidationFailuresAndNoUnpaidPayments()
        {
            var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "Customer" };
            var order = new Order { OrderID = Guid.NewGuid(), UserID = user.Id, User = user, Status = OrderStatus.Completed };
            await _context.Users.AddAsync(user);
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            Assert.That(await _paymentService.PayOrderByCardAsync(null!, user.Id), Is.False);
            Assert.That(await _paymentService.PayOrderByCardAsync(new CardPaymentViewModel { OrderId = Guid.Empty, PaymentSuccess = true }, user.Id), Is.False);
            Assert.That(await _paymentService.PayOrderByCardAsync(new CardPaymentViewModel { OrderId = order.OrderID, PaymentSuccess = false }, user.Id), Is.False);
            Assert.That(await _paymentService.PayOrderByCardAsync(new CardPaymentViewModel
            {
                OrderId = order.OrderID,
                Amount = 1m,
                PaymentSuccess = true,
                CardNumber = "not-a-card",
                ExpiryDate = DateTime.UtcNow.AddYears(1).ToString("MM'/'yy", System.Globalization.CultureInfo.InvariantCulture)
            }, user.Id), Is.False);
            Assert.That(await _paymentService.PayOrderByCardAsync(new CardPaymentViewModel
            {
                OrderId = order.OrderID,
                Amount = 1m,
                PaymentSuccess = true,
                CardNumber = "4111111111111111",
                ExpiryDate = "01/20"
            }, user.Id), Is.False);
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
        public async Task MarkPaymentAsCashAsync_SetsMethodAndDate_ForUnpaidPayment()
        {
            var paymentId = Guid.NewGuid();
            _context.Payments.Add(new Payment { PaymentID = paymentId, Amount = 100.00m, OrderID = Guid.NewGuid(), CreatedOn = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            bool result = await _paymentService.MarkPaymentAsCashAsync(paymentId);

            var payment = await _context.Payments.FindAsync(paymentId);
            Assert.That(result, Is.True);
            Assert.That(payment!.PaymentMethod, Is.EqualTo(PaymentMethods.Cash));
            Assert.That(payment.PaymentDate, Is.Not.Null);
        }

        [Test]
        public void MarkPaymentAsCashAsync_Throws_WhenPaymentIdIsEmptyOrAlreadyPaid()
        {
            var paidPaymentId = Guid.NewGuid();
            _context.Payments.Add(new Payment { PaymentID = paidPaymentId, Amount = 100.00m, OrderID = Guid.NewGuid(), CreatedOn = DateTime.UtcNow, PaymentDate = DateTime.UtcNow });
            _context.SaveChanges();

            Assert.ThrowsAsync<ArgumentException>(() => _paymentService.MarkPaymentAsCashAsync(Guid.Empty));
            Assert.ThrowsAsync<KeyNotFoundException>(() => _paymentService.MarkPaymentAsCashAsync(paidPaymentId));
        }

        [Test]
        public async Task MarkOrderPaymentsAsCashAsync_MarksOnlyUnpaidPaymentsForOrder()
        {
            var orderId = Guid.NewGuid();
            var paidPaymentId = Guid.NewGuid();
            await _context.Payments.AddRangeAsync(
                new Payment { PaymentID = Guid.NewGuid(), Amount = 10m, OrderID = orderId, CreatedOn = DateTime.UtcNow },
                new Payment { PaymentID = paidPaymentId, Amount = 20m, OrderID = orderId, CreatedOn = DateTime.UtcNow, PaymentDate = DateTime.UtcNow, PaymentMethod = PaymentMethods.Card });
            await _context.SaveChangesAsync();

            bool result = await _paymentService.MarkOrderPaymentsAsCashAsync(orderId);

            var payments = await _context.Payments.Where(p => p.OrderID == orderId).ToListAsync();
            Assert.That(result, Is.True);
            Assert.That(payments.Single(p => p.PaymentID != paidPaymentId).PaymentMethod, Is.EqualTo(PaymentMethods.Cash));
            Assert.That(payments.Single(p => p.PaymentID == paidPaymentId).PaymentMethod, Is.EqualTo(PaymentMethods.Card));
        }

        [Test]
        public void MarkOrderPaymentsAsCashAsync_Throws_WhenOrderIdIsEmptyOrNoUnpaidPaymentsExist()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _paymentService.MarkOrderPaymentsAsCashAsync(Guid.Empty));
            Assert.ThrowsAsync<KeyNotFoundException>(() => _paymentService.MarkOrderPaymentsAsCashAsync(Guid.NewGuid()));
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
            Assert.AreEqual(200.00m, updatedPayment!.Amount);
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

            Assert.ThrowsAsync<ArgumentNullException>(() => _paymentService.UpdatePaymentAsync(paymentId, null!));
        }

        [Test]
        public async Task GetOrderIdByPaymentIdAsync_ReturnsOrderIdOrNull()
        {
            var orderId = Guid.NewGuid();
            var paymentId = Guid.NewGuid();
            await _context.Payments.AddAsync(new Payment { PaymentID = paymentId, Amount = 10m, OrderID = orderId, CreatedOn = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            Assert.That(await _paymentService.GetOrderIdByPaymentIdAsync(paymentId), Is.EqualTo(orderId));
            Assert.That(await _paymentService.GetOrderIdByPaymentIdAsync(Guid.NewGuid()), Is.Null);
        }

        [Test]
        public async Task CreateCoursePayoutAsync_CreatesEqualPayouts_WhenSaveIsTrue()
        {
            var orderIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
            var course = new TruckCourse
            {
                Income = 100m,
                PickupAddress = "Sofia",
                DeliverAddress = "Plovdiv",
                Truck = new Truck { LicensePlate = "PB-1234" }
            };

            await _paymentService.CreateCoursePayoutAsync(orderIds, course, true);

            var payments = await _context.Payments.OrderBy(p => p.OrderID).ToListAsync();
            Assert.That(payments, Has.Count.EqualTo(2));
            Assert.That(payments, Has.All.Matches<Payment>(p => p.Amount == 50m));
            Assert.That(payments, Has.All.Matches<Payment>(p => p.PaymentDescription.Contains("Sofia to Plovdiv")));
        }
    }
}
