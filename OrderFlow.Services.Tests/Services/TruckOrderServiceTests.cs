using Microsoft.EntityFrameworkCore;
using Moq;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services.Core;
using OrderFlow.Services.Core.Contracts;

namespace OrderFlow.Tests.Services
{
    [TestFixture]
    public class TruckOrderServiceTests
    {
        private OrderFlowDbContext _context = null!;
        private Mock<IOrderService> _orderServiceMock = null!;
        private CourseOrderService _courseOrderService = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<OrderFlowDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new OrderFlowDbContext(options);
            _orderServiceMock = new Mock<IOrderService>();
            _orderServiceMock
                .Setup(service => service.ChangeOrderStatusAsync(It.IsAny<Guid?>(), It.IsAny<string?>(), It.IsAny<bool>()))
                .ReturnsAsync(true);

            _courseOrderService = new CourseOrderService(_context, _orderServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void GetAll_ReturnsAllCourseOrders()
        {
            _context.CoursesOrders.AddRange(
                new CourseOrder { OrderID = Guid.NewGuid(), TruckCourseID = Guid.NewGuid() },
                new CourseOrder { OrderID = Guid.NewGuid(), TruckCourseID = Guid.NewGuid() });
            _context.SaveChanges();

            var result = _courseOrderService.GetAll().ToList();

            Assert.That(result, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task AddOrderToCourseAsync_AddsRelationAndChangesOrderStatus_WhenRelationDoesNotExist()
        {
            var orderId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            bool result = await _courseOrderService.AddOrderToCourseAsync(orderId, courseId);

            Assert.That(result, Is.True);
            Assert.That(await _context.CoursesOrders.AnyAsync(co => co.OrderID == orderId && co.TruckCourseID == courseId), Is.True);
            _orderServiceMock.Verify(service =>
                service.ChangeOrderStatusAsync(orderId, OrderStatus.InProgress.ToString(), false),
                Times.Once);
        }

        [Test]
        public async Task AddOrderToCourseAsync_ReturnsFalseAndDoesNotChangeStatus_WhenRelationAlreadyExists()
        {
            var orderId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            await _context.CoursesOrders.AddAsync(new CourseOrder { OrderID = orderId, TruckCourseID = courseId });
            await _context.SaveChangesAsync();

            bool result = await _courseOrderService.AddOrderToCourseAsync(orderId, courseId);

            Assert.That(result, Is.False);
            Assert.That(await _context.CoursesOrders.CountAsync(), Is.EqualTo(1));
            _orderServiceMock.Verify(service =>
                service.ChangeOrderStatusAsync(It.IsAny<Guid?>(), It.IsAny<string?>(), It.IsAny<bool>()),
                Times.Never);
        }

        [Test]
        public async Task AddOrderToCourseAsync_ReturnsTrueWithoutSaving_WhenSaveIsFalse()
        {
            var orderId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            bool result = await _courseOrderService.AddOrderToCourseAsync(orderId, courseId, false);

            Assert.That(result, Is.True);
            Assert.That(_context.ChangeTracker.Entries<CourseOrder>().Any(e => e.Entity.OrderID == orderId), Is.True);
            _orderServiceMock.Verify(service =>
                service.ChangeOrderStatusAsync(orderId, OrderStatus.InProgress.ToString(), false),
                Times.Once);
        }

        [Test]
        public async Task GetByCourseIdAsync_ReturnsOnlyMatchingCourseOrders()
        {
            var courseId = Guid.NewGuid();
            var expectedOrderId = Guid.NewGuid();
            await _context.CoursesOrders.AddRangeAsync(
                new CourseOrder { OrderID = expectedOrderId, TruckCourseID = courseId },
                new CourseOrder { OrderID = Guid.NewGuid(), TruckCourseID = Guid.NewGuid() });
            await _context.SaveChangesAsync();

            var result = (await _courseOrderService.GetByCourseIdAsync(courseId)).ToList();

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].OrderID, Is.EqualTo(expectedOrderId));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsMatchingRelationOrNull()
        {
            var orderId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            await _context.CoursesOrders.AddAsync(new CourseOrder { OrderID = orderId, TruckCourseID = courseId });
            await _context.SaveChangesAsync();

            var result = await _courseOrderService.GetByIdAsync(orderId, courseId);
            var missing = await _courseOrderService.GetByIdAsync(Guid.NewGuid(), courseId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.OrderID, Is.EqualTo(orderId));
            Assert.That(missing, Is.Null);
        }

        [Test]
        public async Task RemoveOrderFromCourseAsync_RemovesRelationAndChangesOrderStatus()
        {
            var orderId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            await _context.CoursesOrders.AddAsync(new CourseOrder { OrderID = orderId, TruckCourseID = courseId });
            await _context.SaveChangesAsync();

            bool result = await _courseOrderService.RemoveOrderFromCourseAsync(orderId, courseId);

            Assert.That(result, Is.True);
            Assert.That(await _context.CoursesOrders.AnyAsync(), Is.False);
            _orderServiceMock.Verify(service =>
                service.ChangeOrderStatusAsync(orderId, OrderStatus.Pending.ToString(), false),
                Times.Once);
        }

        [Test]
        public async Task RemoveOrderFromCourseAsync_ReturnsFalse_WhenRelationDoesNotExist()
        {
            bool result = await _courseOrderService.RemoveOrderFromCourseAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.That(result, Is.False);
            _orderServiceMock.Verify(service =>
                service.ChangeOrderStatusAsync(It.IsAny<Guid?>(), It.IsAny<string?>(), It.IsAny<bool>()),
                Times.Never);
        }
    }
}
