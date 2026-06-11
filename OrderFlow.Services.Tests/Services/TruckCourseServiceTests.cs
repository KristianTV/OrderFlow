using Microsoft.EntityFrameworkCore;
using Moq;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services.Contracts;
using OrderFlow.Services.Core;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Course;
using OrderFlow.ViewModels.CourseOrder;

namespace OrderFlow.Tests.Services
{
    [TestFixture]
    public class TruckCourseServiceTests
    {
        private OrderFlowDbContext _context = null!;
        private TruckCourseService _truckCourseService = null!;
        private Mock<INotificationService> _notificationServiceMock = null!;
        private Mock<IPaymentService> _paymentServiceMock = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<OrderFlowDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new OrderFlowDbContext(options);
            _notificationServiceMock = new Mock<INotificationService>();
            _notificationServiceMock
                .Setup(service => service.SendSystemNotificationAsync(It.IsAny<OrderFlow.Services.Core.Commands.NotificationCommand>(), It.IsAny<bool>()))
                .ReturnsAsync(true);
            _paymentServiceMock = new Mock<IPaymentService>();
            _paymentServiceMock
                .Setup(service => service.CreateCoursePayoutAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<TruckCourse>(), It.IsAny<bool>()))
                .Returns(Task.CompletedTask);

            var truckService = new TruckService(_context);
            var orderService = new OrderService(_context, Mock.Of<IMailService>(), _notificationServiceMock.Object);
            var courseOrderService = new CourseOrderService(_context, orderService);

            _truckCourseService = new TruckCourseService(
                _context,
                orderService,
                _notificationServiceMock.Object,
                truckService,
                courseOrderService,
                _paymentServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAllGetByIdAndGetQueryableById_ReturnCourses()
        {
            var course = await AddCourseAsync();

            Assert.That(_truckCourseService.GetAll().Count(), Is.EqualTo(1));
            Assert.That((await _truckCourseService.GetByIdAsync(course.TruckCourseID))!.TruckCourseID, Is.EqualTo(course.TruckCourseID));
            Assert.That(_truckCourseService.GetQueryableByIdAsync(course.TruckCourseID).Single().TruckCourseID, Is.EqualTo(course.TruckCourseID));
        }

        [Test]
        public async Task GetCourseForEditAsync_ReturnsEditableModel()
        {
            var truck = await AddTruckAsync();
            var course = await AddCourseAsync(truck.TruckID, pickup: "Old pickup", deliver: "Old delivery", income: 123m);

            var result = await _truckCourseService.GetCourseForEditAsync(course.TruckCourseID);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.SelectedTruckID, Is.EqualTo(truck.TruckID));
            Assert.That(result.PickupAddress, Is.EqualTo("Old pickup"));
            Assert.That(result.DeliverAddress, Is.EqualTo("Old delivery"));
            Assert.That(result.Income, Is.EqualTo(123m));
        }

        [Test]
        public async Task CreateCourseAsync_CreatesAssignedOrPendingCourse()
        {
            var truck = await AddTruckAsync();

            bool assignedResult = await _truckCourseService.CreateCourseAsync(new CreateCourseViewModel
            {
                SelectedTruckID = truck.TruckID,
                PickupAddress = "Sofia",
                DeliverAddress = "Varna",
                Income = 300m
            });
            bool pendingResult = await _truckCourseService.CreateCourseAsync(new CreateCourseViewModel
            {
                SelectedTruckID = null,
                PickupAddress = "Plovdiv",
                DeliverAddress = "Burgas",
                Income = null
            });

            var courses = await _context.TrucksCourses.OrderBy(c => c.PickupAddress).ToListAsync();
            Assert.That(assignedResult, Is.True);
            Assert.That(pendingResult, Is.True);
            Assert.That(courses.Single(c => c.PickupAddress == "Sofia").Status, Is.EqualTo(CourseStatus.Assigned));
            Assert.That(courses.Single(c => c.PickupAddress == "Plovdiv").Status, Is.EqualTo(CourseStatus.Pending));
        }

        [Test]
        public void CreateCourseAsync_Throws_WhenModelIsNull()
        {
            Assert.ThrowsAsync<NullReferenceException>(() => _truckCourseService.CreateCourseAsync(null!));
        }

        [Test]
        public async Task UpdateCourseAsync_UpdatesExistingCourseAndStatus()
        {
            var truck = await AddTruckAsync();
            var course = await AddCourseAsync();

            bool result = await _truckCourseService.UpdateCourseAsync(new CreateCourseViewModel
            {
                SelectedTruckID = truck.TruckID,
                PickupAddress = "Updated pickup",
                DeliverAddress = "Updated delivery",
                Income = 555m
            }, course.TruckCourseID);

            var updated = await _context.TrucksCourses.FindAsync(course.TruckCourseID);
            Assert.That(result, Is.True);
            Assert.That(updated!.TruckID, Is.EqualTo(truck.TruckID));
            Assert.That(updated.Status, Is.EqualTo(CourseStatus.Assigned));
            Assert.That(updated.Income, Is.EqualTo(555m));
        }

        [Test]
        public async Task UpdateCourseAsync_ReturnsFalse_WhenInputIsInvalidOrCourseMissing()
        {
            Assert.That(await _truckCourseService.UpdateCourseAsync(null!, Guid.NewGuid()), Is.False);
            Assert.That(await _truckCourseService.UpdateCourseAsync(new CreateCourseViewModel(), Guid.Empty), Is.False);
            Assert.That(await _truckCourseService.UpdateCourseAsync(new CreateCourseViewModel(), Guid.NewGuid()), Is.False);
        }

        [Test]
        public async Task DeleteCourseAsync_DeletesCourse()
        {
            var course = await AddCourseAsync();

            bool result = await _truckCourseService.DeleteCourseAsync(course.TruckCourseID);

            Assert.That(result, Is.True);
            Assert.That(await _context.TrucksCourses.FindAsync(course.TruckCourseID), Is.Null);
        }

        [Test]
        public void DeleteCourseAsync_Throws_WhenIdIsEmptyOrCourseMissing()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _truckCourseService.DeleteCourseAsync(Guid.Empty));
            Assert.ThrowsAsync<KeyNotFoundException>(() => _truckCourseService.DeleteCourseAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task AssignOrdersToCourseAsync_AssignsOrdersWithinCapacityAndChangesStatus()
        {
            var truck = await AddTruckAsync(capacity: 100);
            var course = await AddCourseAsync(truck.TruckID);
            var order = await AddOrderAsync(loadCapacity: 40);

            int changes = await _truckCourseService.AssignOrdersToCourseAsync(new[]
            {
                new OrderViewModel { OrderID = order.OrderID, LoadCapacity = 40 }
            }, course.TruckCourseID);

            var assigned = await _context.CoursesOrders.SingleAsync();
            var updatedOrder = await _context.Orders.FindAsync(order.OrderID);
            Assert.That(changes, Is.GreaterThan(0));
            Assert.That(assigned.OrderID, Is.EqualTo(order.OrderID));
            Assert.That(updatedOrder!.Status, Is.EqualTo(OrderStatus.InProgress));
            _notificationServiceMock.Verify(service =>
                service.SendSystemNotificationAsync(It.IsAny<OrderFlow.Services.Core.Commands.NotificationCommand>(), false),
                Times.AtLeastOnce);
        }

        [Test]
        public async Task AssignOrdersToCourseAsync_ThrowsForInvalidInputMissingOrdersAndCapacityOverflow()
        {
            var truck = await AddTruckAsync(capacity: 10);
            var course = await AddCourseAsync(truck.TruckID);
            var order = await AddOrderAsync(loadCapacity: 20);

            Assert.ThrowsAsync<ArgumentException>(() => _truckCourseService.AssignOrdersToCourseAsync(Array.Empty<OrderViewModel>(), course.TruckCourseID));
            Assert.ThrowsAsync<InvalidOperationException>(() => _truckCourseService.AssignOrdersToCourseAsync(new[] { new OrderViewModel { OrderID = Guid.NewGuid(), LoadCapacity = 1 } }, course.TruckCourseID));
            Assert.ThrowsAsync<InvalidOperationException>(() => _truckCourseService.AssignOrdersToCourseAsync(new[] { new OrderViewModel { OrderID = order.OrderID, LoadCapacity = 20 } }, course.TruckCourseID));
        }

        [Test]
        public async Task CompleteCourseAsync_CompletesCourseAndCreatesPayouts()
        {
            var driverId = Guid.NewGuid();
            var truck = await AddTruckAsync(driverId: driverId);
            var course = await AddCourseAsync(truck.TruckID, deliver: "Delivery A", status: CourseStatus.Assigned, income: 200m);
            var order = await AddOrderAsync(status: OrderStatus.InProgress, deliveryAddress: "Delivery A");
            await _context.CoursesOrders.AddAsync(new CourseOrder
            {
                OrderID = order.OrderID,
                TruckCourseID = course.TruckCourseID
            });
            await _context.SaveChangesAsync();

            bool result = await _truckCourseService.CompleteCourseAsync(course.TruckCourseID);

            var completedCourse = await _context.TrucksCourses.FindAsync(course.TruckCourseID);
            var completedOrder = await _context.Orders.FindAsync(order.OrderID);
            Assert.That(result, Is.True);
            Assert.That(completedCourse!.Status, Is.EqualTo(CourseStatus.Delivered));
            Assert.That(completedCourse.DeliveryDate, Is.Not.Null);
            Assert.That(completedOrder!.Status, Is.EqualTo(OrderStatus.Completed));
            _paymentServiceMock.Verify(service =>
                service.CreateCoursePayoutAsync(It.Is<IEnumerable<Guid>>(ids => ids.Contains(order.OrderID)), It.IsAny<TruckCourse>(), false),
                Times.Once);
        }

        [Test]
        public async Task CompleteCourseAsync_ReturnsFalse_WhenCourseCannotBeCompleted()
        {
            var deliveredCourse = await AddCourseAsync(status: CourseStatus.Delivered);
            var noTruckCourse = await AddCourseAsync(truckId: null, status: CourseStatus.Assigned);
            var truck = await AddTruckAsync(driverId: Guid.Empty);
            var noDriverCourse = await AddCourseAsync(truck.TruckID, status: CourseStatus.Assigned);

            Assert.That(await _truckCourseService.CompleteCourseAsync(Guid.NewGuid()), Is.False);
            Assert.That(await _truckCourseService.CompleteCourseAsync(deliveredCourse.TruckCourseID), Is.False);
            Assert.That(await _truckCourseService.CompleteCourseAsync(noTruckCourse.TruckCourseID), Is.False);
            Assert.That(await _truckCourseService.CompleteCourseAsync(noDriverCourse.TruckCourseID), Is.False);
        }

        private async Task<Truck> AddTruckAsync(Guid? driverId = null, double capacity = 100)
        {
            var truck = new Truck
            {
                TruckID = Guid.NewGuid(),
                DriverID = driverId ?? Guid.NewGuid(),
                LicensePlate = "TRK-" + Guid.NewGuid().ToString("N")[..4],
                Capacity = capacity,
                Status = TruckStatus.Available
            };
            await _context.Trucks.AddAsync(truck);
            await _context.SaveChangesAsync();
            return truck;
        }

        private async Task<TruckCourse> AddCourseAsync(Guid? truckId = null, string pickup = "Pickup", string deliver = "Delivery", CourseStatus status = CourseStatus.Pending, decimal income = 100m)
        {
            var course = new TruckCourse
            {
                TruckCourseID = Guid.NewGuid(),
                TruckID = truckId,
                PickupAddress = pickup,
                DeliverAddress = deliver,
                AssignedDate = DateTime.UtcNow,
                Status = status,
                Income = income
            };
            await _context.TrucksCourses.AddAsync(course);
            await _context.SaveChangesAsync();
            return course;
        }

        private async Task<Order> AddOrderAsync(double loadCapacity = 10, OrderStatus status = OrderStatus.Pending, string deliveryAddress = "Delivery")
        {
            var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "Customer" + Guid.NewGuid().ToString("N")[..4] };
            var order = new Order
            {
                OrderID = Guid.NewGuid(),
                UserID = user.Id,
                User = user,
                OrderDate = DateTime.UtcNow,
                PickupAddress = "Pickup",
                DeliveryAddress = deliveryAddress,
                LoadCapacity = loadCapacity,
                Status = status
            };
            await _context.Users.AddAsync(user);
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }
    }
}
