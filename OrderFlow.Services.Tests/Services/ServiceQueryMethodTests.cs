using Microsoft.EntityFrameworkCore;
using Moq;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services.Core;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Course;
using OrderFlow.ViewModels.Order;
using OrderFlow.ViewModels.Truck;

namespace OrderFlow.Tests.Services
{
    [TestFixture]
    public class ServiceQueryMethodTests
    {
        private OrderFlowDbContext _context = null!;
        private OrderService _orderService = null!;
        private TruckService _truckService = null!;
        private TruckCourseService _truckCourseService = null!;

        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<OrderFlowDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new OrderFlowDbContext(options);
            _truckService = new TruckService(_context);
            _orderService = new OrderService(_context, new Mock<INotificationService>().Object);
            _truckCourseService = new TruckCourseService(
                _context,
                _orderService,
                new Mock<INotificationService>().Object,
                _truckService,
                new Mock<ICourseOrderService>().Object);

            await SeedDataAsync();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetUserOrdersAsync_FiltersByUserStatusAndHideCompleted()
        {
            var user = await _context.Users.SingleAsync(u => u.UserName == "Customer");

            var result = (await _orderService.GetUserOrdersAsync(user.Id, new OrderQueryModel
            {
                HideCompleted = true,
                StatusFilter = OrderStatus.Pending.ToString(),
                SortOrder = "date_desc"
            })).ToList();

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].PickupAddress, Is.EqualTo("Warehouse A"));
            Assert.That(result[0].Status, Is.EqualTo(OrderStatus.Pending.ToString()));
        }

        [Test]
        public async Task GetAdminOrdersAsync_ReturnsMatchingSearchAcrossAllUsers()
        {
            var targetOrder = await _context.Orders.FirstAsync(o => o.PickupAddress == "Warehouse B");

            var result = (await _orderService.GetAdminOrdersAsync(new OrderQueryModel
            {
                SearchId = targetOrder.OrderID.ToString()[..8]
            })).ToList();

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].OrderID, Is.EqualTo(targetOrder.OrderID));
        }

        [Test]
        public async Task GetOrderDetailsAsync_RestrictsToOwnerWhenUserIdIsProvided()
        {
            var otherUser = await _context.Users.SingleAsync(u => u.UserName == "OtherCustomer");
            var customerOrder = await _context.Orders.FirstAsync(o => o.PickupAddress == "Warehouse A");

            var result = await _orderService.GetOrderDetailsAsync(customerOrder.OrderID, otherUser.Id);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetTrucksAsync_FiltersByDriverAndStatus()
        {
            var driver = await _context.Users.SingleAsync(u => u.UserName == "Driver");

            var result = (await _truckService.GetTrucksAsync(driver.Id, new TruckQueryModel
            {
                Status = TruckStatus.Available.ToString()
            })).ToList();

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].LicensePlate, Is.EqualTo("DRV-001"));
        }

        [Test]
        public async Task GetTruckDetailsAsync_ReturnsNullWhenTruckDoesNotBelongToDriver()
        {
            var otherDriver = await _context.Users.SingleAsync(u => u.UserName == "OtherDriver");
            var truck = await _context.Trucks.SingleAsync(t => t.LicensePlate == "DRV-001");

            var result = await _truckService.GetTruckDetailsAsync(truck.TruckID, otherDriver.Id);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetAvailableTruckOptionsAsync_ReturnsOnlyAvailableTrucks()
        {
            var result = await _truckService.GetAvailableTruckOptionsAsync();

            Assert.That(result.Values, Is.EquivalentTo(new[] { "DRV-001" }));
        }

        [Test]
        public async Task GetCoursesAsync_FiltersByDriverAndHidesDelivered()
        {
            var driver = await _context.Users.SingleAsync(u => u.UserName == "Driver");

            var result = (await _truckCourseService.GetCoursesAsync(driver.Id, new CourseQueryModel
            {
                HideCompleted = true
            })).ToList();

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].DeliverAddress, Is.EqualTo("Customer A"));
            Assert.That(result[0].Status, Is.EqualTo(CourseStatus.Assigned));
        }

        [Test]
        public async Task GetCourseDetailsAsync_IncludesAssignedOrders()
        {
            var course = await _context.TrucksCourses.SingleAsync(c => c.DeliverAddress == "Customer A");

            var result = await _truckCourseService.GetCourseDetailsAsync(course.TruckCourseID);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.AssinedOrders, Has.Count.EqualTo(1));
            Assert.That(result.AssinedOrders.Single().DeliveryAddress, Is.EqualTo("Customer A"));
        }

        private async Task SeedDataAsync()
        {
            var customer = new ApplicationUser { Id = Guid.NewGuid(), UserName = "Customer", Email = "customer@test.com" };
            var otherCustomer = new ApplicationUser { Id = Guid.NewGuid(), UserName = "OtherCustomer", Email = "other@test.com" };
            var driver = new ApplicationUser { Id = Guid.NewGuid(), UserName = "Driver", Email = "driver@test.com" };
            var otherDriver = new ApplicationUser { Id = Guid.NewGuid(), UserName = "OtherDriver", Email = "other-driver@test.com" };

            var order = new Order
            {
                OrderID = Guid.NewGuid(),
                UserID = customer.Id,
                User = customer,
                PickupAddress = "Warehouse A",
                DeliveryAddress = "Customer A",
                OrderDate = DateTime.UtcNow.AddDays(-1),
                Status = OrderStatus.Pending,
                LoadCapacity = 10
            };
            var completedOrder = new Order
            {
                OrderID = Guid.NewGuid(),
                UserID = customer.Id,
                User = customer,
                PickupAddress = "Warehouse Completed",
                DeliveryAddress = "Customer Completed",
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Completed,
                LoadCapacity = 20
            };
            var otherOrder = new Order
            {
                OrderID = Guid.NewGuid(),
                UserID = otherCustomer.Id,
                User = otherCustomer,
                PickupAddress = "Warehouse B",
                DeliveryAddress = "Customer B",
                OrderDate = DateTime.UtcNow.AddHours(-1),
                Status = OrderStatus.Pending,
                LoadCapacity = 30
            };

            var availableTruck = new Truck
            {
                TruckID = Guid.NewGuid(),
                DriverID = driver.Id,
                Driver = driver,
                LicensePlate = "DRV-001",
                Capacity = 100,
                Status = TruckStatus.Available
            };
            var unavailableTruck = new Truck
            {
                TruckID = Guid.NewGuid(),
                DriverID = driver.Id,
                Driver = driver,
                LicensePlate = "DRV-002",
                Capacity = 200,
                Status = TruckStatus.Unavailable
            };

            var assignedCourse = new TruckCourse
            {
                TruckCourseID = Guid.NewGuid(),
                TruckID = availableTruck.TruckID,
                Truck = availableTruck,
                PickupAddress = "Warehouse A",
                DeliverAddress = "Customer A",
                AssignedDate = DateTime.UtcNow.AddHours(-3),
                Status = CourseStatus.Assigned,
                Income = 120
            };
            var deliveredCourse = new TruckCourse
            {
                TruckCourseID = Guid.NewGuid(),
                TruckID = availableTruck.TruckID,
                Truck = availableTruck,
                PickupAddress = "Warehouse Old",
                DeliverAddress = "Customer Old",
                AssignedDate = DateTime.UtcNow.AddDays(-2),
                Status = CourseStatus.Delivered,
                Income = 80
            };

            var courseOrder = new CourseOrder
            {
                OrderID = order.OrderID,
                Order = order,
                TruckCourseID = assignedCourse.TruckCourseID,
                TruckCourse = assignedCourse
            };

            await _context.Users.AddRangeAsync(customer, otherCustomer, driver, otherDriver);
            await _context.Orders.AddRangeAsync(order, completedOrder, otherOrder);
            await _context.Trucks.AddRangeAsync(availableTruck, unavailableTruck);
            await _context.TrucksCourses.AddRangeAsync(assignedCourse, deliveredCourse);
            await _context.CoursesOrders.AddAsync(courseOrder);
            await _context.SaveChangesAsync();
        }
    }
}
