using Microsoft.EntityFrameworkCore;
using Moq;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services.Core;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Order;

namespace OrderFlow.Tests.Services
{
    [TestFixture]
    public class OrderServiceTests
    {
        private OrderFlowDbContext _context;
        private Mock<INotificationService> _mockNotificationService;
        private Mock<ICourseOrderService> _mockTruckOrderService;
        private Mock<ITruckService> _mockTruckService;
        private OrderService _orderService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<OrderFlowDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new OrderFlowDbContext(options);

            _mockNotificationService = new Mock<INotificationService>();
            _mockTruckOrderService = new Mock<ICourseOrderService>();
            _mockTruckService = new Mock<ITruckService>();

            _orderService = new OrderService(_context, _mockNotificationService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private async Task<ApplicationUser> AddUser(string username)
        {
            var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = username, Email = $"{username}@test.com" };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        private async Task<Order> AddOrder(Guid userId, OrderStatus status = OrderStatus.Pending, bool isCancelled = false, string deliveryAddress = "Delivery A", string pickupAddress = "Pickup A", int loadCapacity = 100)
        {
            var order = new Order
            {
                OrderID = Guid.NewGuid(),
                UserID = userId,
                OrderDate = DateTime.UtcNow,
                DeliveryAddress = deliveryAddress,
                PickupAddress = pickupAddress,
                Status = status,
                IsCanceled = isCancelled,
                LoadCapacity = loadCapacity
            };
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        private async Task<Truck> AddTruck(Guid driverId, TruckStatus status = TruckStatus.Available, int capacity = 1000)
        {
            var truck = new Truck
            {
                TruckID = Guid.NewGuid(),
                DriverID = driverId,
                Capacity = capacity,
                Status = status
            };
            await _context.Trucks.AddAsync(truck);
            await _context.SaveChangesAsync();
            return truck;
        }

        private async Task<CourseOrder> AddOrderToCourse(Guid orderId, Guid truckId, CourseStatus status = CourseStatus.Assigned, string deliverAddress = "Delivery A")
        {
            var course = new TruckCourse
            {
                TruckCourseID = Guid.NewGuid(),
                TruckID = truckId,
                Status = status,
                DeliverAddress = deliverAddress
            };

            var courseOrder = new CourseOrder
            {
                TruckCourseID = course.TruckCourseID,
                OrderID = orderId,

            };

            await _context.TrucksCourses.AddAsync(course);
            await _context.CoursesOrders.AddAsync(courseOrder);
            await _context.SaveChangesAsync();
            return courseOrder;
        }

        [Test]
        public async Task CancelOrderAsync_ShouldReturnFalse_WhenOrderIdIsNull()
        {
            var userId = Guid.NewGuid();

            var result = await _orderService.CancelOrderAsync(null, userId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CancelOrderAsync_ShouldReturnFalse_WhenUserIdIsNull()
        {
            var orderId = Guid.NewGuid();

            var result = await _orderService.CancelOrderAsync(orderId, null);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CancelOrderAsync_ShouldReturnFalse_WhenOrderNotFound()
        {
            var userId = Guid.NewGuid();
            var orderId = Guid.NewGuid();

            var result = await _orderService.CancelOrderAsync(orderId, userId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CancelOrderAsync_ShouldReturnTrue_WhenOrderAlreadyCancelled()
        {
            var user = await AddUser("TestUser");
            var order = await AddOrder(user.Id, OrderStatus.Cancelled, true);

            var result = await _orderService.CancelOrderAsync(order.OrderID, user.Id);

            Assert.That(result, Is.True);
            var updatedOrder = await _context.Orders.FindAsync(order.OrderID);
            Assert.That(updatedOrder.IsCanceled, Is.True);
            Assert.That(updatedOrder.Status, Is.EqualTo(OrderStatus.Cancelled));
            _mockNotificationService.Verify(s => s.AddAsync(It.IsAny<Notification>()), Times.Never);
        }

        [Test]
        public async Task CancelOrderAsync_ShouldCancelOrderSuccessfully_WithoutAssignedTrucks()
        {
            var user = await AddUser("TestUser");
            var order = await AddOrder(user.Id, OrderStatus.Pending, false);

            var result = await _orderService.CancelOrderAsync(order.OrderID, user.Id);

            Assert.That(result, Is.True);
            var updatedOrder = await _context.Orders.FindAsync(order.OrderID);
            Assert.That(updatedOrder.IsCanceled, Is.True);
            Assert.That(updatedOrder.Status, Is.EqualTo(OrderStatus.Cancelled));
            _mockNotificationService.Verify(s => s.AddAsync(It.Is<Notification>(n =>
                n.Title == $"Order {order.OrderID} has been Cancelled" &&
                n.ReceiverID == user.Id)), Times.Once);
        }

        [Test]
        public async Task CancelOrderAsync_ShouldCancelOrderAndTruckOrders_WhenStatusIsNotDelivered()
        {
            var user = await AddUser("TestUser");
            var driver = await AddUser("DriverUser");
            var order = await AddOrder(user.Id, OrderStatus.InProgress, false);
            var truck = await AddTruck(driver.Id);
            var orderTruck1 = await AddOrderToCourse(order.OrderID, truck.TruckID, CourseStatus.Assigned);
            var orderTruck2 = await AddOrderToCourse(order.OrderID, Guid.NewGuid(), CourseStatus.Delivered);

            var result = await _orderService.CancelOrderAsync(order.OrderID, user.Id);

            Assert.That(result, Is.True);
            var updatedOrder = await _context.Orders.Include(o => o.CourseOrders).SingleAsync(o => o.OrderID == order.OrderID);
            Assert.That(updatedOrder.IsCanceled, Is.True);
            Assert.That(updatedOrder.Status, Is.EqualTo(OrderStatus.Cancelled));

            //var updatedOrderTruck1 = updatedOrder.CourseOrders.Single(ot => ot.TruckCourseID == orderTruck1.TruckCourseID);
            //Assert.That(updatedOrderTruck1.Status, Is.EqualTo(CourseStatus.Cancelled));

            //var updatedOrderTruck2 = updatedOrder.CourseOrders.Single(ot => ot.TruckCourseID == orderTruck2.TruckCourseID);
            //Assert.That(updatedOrderTruck2.Status, Is.EqualTo(CourseStatus.Delivered));

            _mockNotificationService.Verify(s => s.AddAsync(It.IsAny<Notification>()), Times.Once);
        }

        [Test]
        public async Task ChangeOrderStatusAsync_ShouldReturnFalse_WhenOrderIdIsEmpty()
        {
            var result = await _orderService.ChangeOrderStatusAsync(Guid.Empty, "Pending");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task ChangeOrderStatusAsync_ShouldReturnFalse_WhenStatusIsNullOrEmpty()
        {
            var orderId = Guid.NewGuid();

            var result1 = await _orderService.ChangeOrderStatusAsync(orderId, null);
            var result2 = await _orderService.ChangeOrderStatusAsync(orderId, "");

            Assert.That(result1, Is.False);
            Assert.That(result2, Is.False);
        }

        [Test]
        public async Task ChangeOrderStatusAsync_ShouldReturnFalse_WhenStatusIsInvalidEnum()
        {
            var user = await AddUser("User");
            var order = await AddOrder(user.Id);

            var result = await _orderService.ChangeOrderStatusAsync(order.OrderID, "InvalidStatus");

            Assert.That(result, Is.False);
            var originalOrder = await _context.Orders.FindAsync(order.OrderID);
            Assert.That(originalOrder.Status, Is.EqualTo(OrderStatus.Pending));
        }

        [Test]
        public async Task ChangeOrderStatusAsync_ShouldReturnFalse_WhenOrderNotFound()
        {
            var result = await _orderService.ChangeOrderStatusAsync(Guid.NewGuid(), "InProgress");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task ChangeOrderStatusAsync_ShouldReturnTrue_WhenOrderAlreadyHasTargetStatus()
        {
            var user = await AddUser("User");
            var order = await AddOrder(user.Id, OrderStatus.InProgress);

            var result = await _orderService.ChangeOrderStatusAsync(order.OrderID, "InProgress");

            Assert.That(result, Is.True);
            var updatedOrder = await _context.Orders.FindAsync(order.OrderID);
            Assert.That(updatedOrder.Status, Is.EqualTo(OrderStatus.InProgress));
            _mockNotificationService.Verify(s => s.AddAsync(It.IsAny<Notification>()), Times.Never);
        }

        [Test]
        public async Task ChangeOrderStatusAsync_ShouldChangeStatusSuccessfullyAndSendNotification()
        {
            var user = await AddUser("User");
            var order = await AddOrder(user.Id, OrderStatus.Pending);

            var result = await _orderService.ChangeOrderStatusAsync(order.OrderID, "InProgress");

            Assert.That(result, Is.True);
            var updatedOrder = await _context.Orders.FindAsync(order.OrderID);
            Assert.That(updatedOrder.Status, Is.EqualTo(OrderStatus.InProgress));
            _mockNotificationService.Verify(s => s.AddAsync(It.Is<Notification>(n =>
                n.Title == $"Order status changed to InProgress" &&
                n.ReceiverID == user.Id)), Times.Once);
        }

        [Test]
        public async Task ChangeStatusToCompletedAsync_ShouldReturnFalse_WhenOrderIdIsEmpty()
        {
            var result = await _orderService.ChangeStatusToCompletedAsync(Guid.Empty);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task ChangeStatusToCompletedAsync_ShouldReturnFalse_WhenOrderNotFound()
        {
            var result = await _orderService.ChangeStatusToCompletedAsync(Guid.NewGuid());

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task ChangeStatusToCompletedAsync_ShouldReturnTrue_WhenOrderAlreadyCompleted()
        {
            var user = await AddUser("User");
            var order = await AddOrder(user.Id, OrderStatus.Completed);

            var result = await _orderService.ChangeStatusToCompletedAsync(order.OrderID);

            Assert.That(result, Is.True);
            var updatedOrder = await _context.Orders.FindAsync(order.OrderID);
            Assert.That(updatedOrder.Status, Is.EqualTo(OrderStatus.Completed));
            _mockNotificationService.Verify(s => s.AddAsync(It.IsAny<Notification>()), Times.Never);
        }

        [Test]
        public async Task ChangeStatusToCompletedAsync_ShouldChangeStatusToCompletedAndSetDeliveryDate()
        {
            var user = await AddUser("User");
            var order = await AddOrder(user.Id, OrderStatus.InProgress);

            var result = await _orderService.ChangeStatusToCompletedAsync(order.OrderID);

            Assert.That(result, Is.True);
            var updatedOrder = await _context.Orders.FindAsync(order.OrderID);
            Assert.That(updatedOrder.Status, Is.EqualTo(OrderStatus.Completed));
            Assert.That(updatedOrder.DeliveryDate, Is.Not.Null);
            Assert.That(updatedOrder.DeliveryDate.Value.Date, Is.EqualTo(DateTime.UtcNow.Date));
            _mockNotificationService.Verify(s => s.AddAsync(It.Is<Notification>(n =>
                n.Title == $"Order status changed to Completed" &&
                n.ReceiverID == user.Id)), Times.Once);
        }

        [Test]
        public async Task GetAll_ShouldReturnAllOrders()
        {
            var user1 = await AddUser("User1");
            var user2 = await AddUser("User2");
            await AddOrder(user1.Id);
            await AddOrder(user2.Id);

            var orders = _orderService.GetAll();

            Assert.That(orders, Is.Not.Null);
            Assert.That(orders.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetAll_ShouldReturnEmptyQueryable_WhenNoOrdersExist()
        {
            var orders = _orderService.GetAll();

            Assert.That(orders, Is.Not.Null);
            Assert.That(orders, Is.Empty);
        }

        [Test]
        public async Task ReactivateOrderAsync_ShouldReturnFalse_WhenOrderIdIsEmpty()
        {
            var result = await _orderService.ReactivateOrderAsync(Guid.Empty);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task ReactivateOrderAsync_ShouldReturnFalse_WhenOrderNotFound()
        {
            var result = await _orderService.ReactivateOrderAsync(Guid.NewGuid());

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task ReactivateOrderAsync_ShouldReturnTrue_WhenOrderNotCancelled()
        {
            var user = await AddUser("User");
            var order = await AddOrder(user.Id, OrderStatus.Pending, false);

            var result = await _orderService.ReactivateOrderAsync(order.OrderID);

            Assert.That(result, Is.True);
            var updatedOrder = await _context.Orders.FindAsync(order.OrderID);
            Assert.That(updatedOrder.IsCanceled, Is.False);
            Assert.That(updatedOrder.Status, Is.EqualTo(OrderStatus.Pending));
        }

        [Test]
        public async Task ReactivateOrderAsync_ShouldReactivateOrderSuccessfully_WithoutAssignedTrucks()
        {
            var user = await AddUser("TestUser");
            var order = await AddOrder(user.Id, OrderStatus.Cancelled, true);

            var result = await _orderService.ReactivateOrderAsync(order.OrderID);

            Assert.That(result, Is.True);
            var updatedOrder = await _context.Orders.FindAsync(order.OrderID);
            Assert.That(updatedOrder.IsCanceled, Is.False);
            Assert.That(updatedOrder.Status, Is.EqualTo(OrderStatus.Pending));
        }

        [Test]
        public async Task CreateOrderAsync_User_ShouldCreateOrderSuccessfully()
        {
            var user = await AddUser("User");
            var createOrderViewModel = new CreateOrderViewModel
            {
                DeliveryAddress = "Addr1",
                PickupAddress = "Addr2",
                LoadCapacity = 500
            };

            var result = await _orderService.CreateOrderAsync(createOrderViewModel, user.Id);

            Assert.That(result, Is.True);
            var order = await _context.Orders.SingleOrDefaultAsync();
            Assert.That(order, Is.Not.Null);
            Assert.That(order.UserID, Is.EqualTo(user.Id));
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Pending));
            Assert.That(order.LoadCapacity, Is.EqualTo(500));
        }

        [Test]
        public async Task CreateOrderAsync_User_ShouldReturnFalse_WhenViewModelIsNull()
        {
            var userId = Guid.NewGuid();

            var result = await _orderService.CreateOrderAsync(null!, userId);

            Assert.That(result, Is.False);
            Assert.That(_context.Orders.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task CreateOrderAsync_User_ShouldReturnFalse_WhenUserIdIsNull()
        {
            var createOrderViewModel = new CreateOrderViewModel();

            var result = await _orderService.CreateOrderAsync(createOrderViewModel, null);

            Assert.That(result, Is.False);
            Assert.That(_context.Orders.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task CreateOrderAsync_Admin_ShouldCreateOrderSuccessfully()
        {
            var user = await AddUser("AdminUser");
            var createOrderViewModel = new AdminCreateOrderViewModel
            {
                UsersId = user.Id,
                DeliveryAddress = "AdminAddr1",
                PickupAddress = "AdminAddr2",
                LoadCapacity = 700
            };

            var result = await _orderService.CreateOrderAsync(createOrderViewModel, createOrderViewModel.UsersId);

            Assert.That(result, Is.True);
            var order = await _context.Orders.SingleOrDefaultAsync();
            Assert.That(order, Is.Not.Null);
            Assert.That(order.UserID, Is.EqualTo(user.Id));
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Pending));
            Assert.That(order.LoadCapacity, Is.EqualTo(700));
        }

        [Test]
        public async Task CreateOrderAsync_Admin_ShouldReturnFalse_WhenViewModelIsNull()
        {
            var result = await _orderService.CreateOrderAsync(null!, null);

            Assert.That(result, Is.False);
            Assert.That(_context.Orders.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task UpdateOrderAsync_User_ShouldUpdateOrderSuccessfully()
        {
            var user = await AddUser("User");
            var order = await AddOrder(user.Id, OrderStatus.Pending, false, "Old Delivery", "Old Pickup", 100);
            var updateViewModel = new CreateOrderViewModel
            {
                DeliveryAddress = "New Delivery",
                PickupAddress = "New Pickup",
                DeliveryInstructions = "New Instructions",
                LoadCapacity = 200
            };

            var result = await _orderService.UpdateOrderAsync(updateViewModel, order.OrderID, user.Id);

            Assert.That(result, Is.True);
            var updatedOrder = await _context.Orders.FindAsync(order.OrderID);
            Assert.That(updatedOrder.DeliveryAddress, Is.EqualTo("New Delivery"));
            Assert.That(updatedOrder.PickupAddress, Is.EqualTo("New Pickup"));
            Assert.That(updatedOrder.DeliveryInstructions, Is.EqualTo("New Instructions"));
            Assert.That(updatedOrder.LoadCapacity, Is.EqualTo(200));
            Assert.That(updatedOrder.Status, Is.EqualTo(OrderStatus.Pending));
        }

        [Test]
        public async Task UpdateOrderAsync_User_ShouldReturnFalse_WhenViewModelIsNull()
        {
            var user = await AddUser("User");
            var order = await AddOrder(user.Id);

            var result = await _orderService.UpdateOrderAsync(null!, order.OrderID, user.Id);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateOrderAsync_User_ShouldReturnFalse_WhenOrderIdIsEmpty()
        {
            var user = await AddUser("User");
            var updateViewModel = new CreateOrderViewModel();

            var result = await _orderService.UpdateOrderAsync(updateViewModel, Guid.Empty, user.Id);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateOrderAsync_User_ShouldReturnFalse_WhenUserIdIsEmpty()
        {
            var user = await AddUser("User");
            var order = await AddOrder(user.Id);
            var updateViewModel = new CreateOrderViewModel();

            var result = await _orderService.UpdateOrderAsync(updateViewModel, order.OrderID, Guid.Empty);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateOrderAsync_User_ShouldReturnFalse_WhenOrderNotFound()
        {
            var user = await AddUser("User");
            var updateViewModel = new CreateOrderViewModel();

            var result = await _orderService.UpdateOrderAsync(updateViewModel, Guid.NewGuid(), user.Id);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateOrderAsync_User_ShouldReturnFalse_WhenUserIsNotOwner()
        {
            var owner = await AddUser("Owner");
            var unauthorizedUser = await AddUser("Unauthorized");
            var order = await AddOrder(owner.Id);
            var updateViewModel = new CreateOrderViewModel();

            var result = await _orderService.UpdateOrderAsync(updateViewModel, order.OrderID, unauthorizedUser.Id);

            Assert.That(result, Is.False);
            var originalOrder = await _context.Orders.FindAsync(order.OrderID);
            Assert.That(originalOrder.DeliveryAddress, Is.EqualTo(order.DeliveryAddress));
        }

        [Test]
        public async Task UpdateOrderAsync_User_ShouldReturnFalse_WhenOrderStatusIsCompleted()
        {
            var user = await AddUser("User");
            var order = await AddOrder(user.Id, OrderStatus.Completed);
            var updateViewModel = new CreateOrderViewModel { DeliveryAddress = "New Address" };

            var result = await _orderService.UpdateOrderAsync(updateViewModel, order.OrderID, user.Id);

            Assert.That(result, Is.False);
            var updatedOrder = await _context.Orders.FindAsync(order.OrderID);
            Assert.That(updatedOrder.DeliveryAddress, Is.EqualTo(order.DeliveryAddress));
        }

        [Test]
        public async Task UpdateOrderAsync_Admin_ShouldUpdateOrderSuccessfully()
        {
            var user = await AddUser("User");
            var admin = await AddUser("Admin");
            var order = await AddOrder(user.Id, OrderStatus.Pending, false, "Old Delivery", "Old Pickup", 100);
            var updateViewModel = new AdminCreateOrderViewModel
            {
                UsersId = user.Id,
                DeliveryAddress = "New Admin Delivery",
                PickupAddress = "New Admin Pickup",
                DeliveryInstructions = "New Admin Instructions",
                LoadCapacity = 300
            };

            var result = await _orderService.UpdateOrderAsync(updateViewModel, order.OrderID, updateViewModel.UsersId);

            Assert.That(result, Is.True);
            var updatedOrder = await _context.Orders.FindAsync(order.OrderID);
            Assert.That(updatedOrder.UserID, Is.EqualTo(user.Id));
            Assert.That(updatedOrder.DeliveryAddress, Is.EqualTo("New Admin Delivery"));
            Assert.That(updatedOrder.PickupAddress, Is.EqualTo("New Admin Pickup"));
            Assert.That(updatedOrder.DeliveryInstructions, Is.EqualTo("New Admin Instructions"));
            Assert.That(updatedOrder.LoadCapacity, Is.EqualTo(300));
            Assert.That(updatedOrder.Status, Is.EqualTo(OrderStatus.Pending));
        }

        [Test]
        public async Task UpdateOrderAsync_Admin_ShouldReturnFalse_WhenViewModelIsNull()
        {
            var orderId = Guid.NewGuid();

            var result = await _orderService.UpdateOrderAsync(null!, orderId, null);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateOrderAsync_Admin_ShouldReturnFalse_WhenOrderIdIsEmpty()
        {
            var updateViewModel = new AdminCreateOrderViewModel { UsersId = Guid.NewGuid() };

            var result = await _orderService.UpdateOrderAsync(updateViewModel, Guid.Empty, updateViewModel.UsersId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateOrderAsync_Admin_ShouldReturnFalse_WhenOrderNotFound()
        {
            var user = await AddUser("User");
            var updateViewModel = new AdminCreateOrderViewModel { UsersId = user.Id };

            var result = await _orderService.UpdateOrderAsync(updateViewModel, Guid.NewGuid(), updateViewModel.UsersId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateOrderAsync_Admin_ShouldReturnFalse_WhenOrderStatusIsCompleted()
        {
            var user = await AddUser("User");
            var order = await AddOrder(user.Id, OrderStatus.Completed);
            var updateViewModel = new AdminCreateOrderViewModel { UsersId = user.Id, DeliveryAddress = "New Address" };

            var result = await _orderService.UpdateOrderAsync(updateViewModel, order.OrderID, updateViewModel.UsersId);

            Assert.That(result, Is.False);
            var updatedOrder = await _context.Orders.FindAsync(order.OrderID);
            Assert.That(updatedOrder.DeliveryAddress, Is.EqualTo(order.DeliveryAddress));
        }

        [Test]
        public void CompleteOrderAsync_ShouldThrowArgumentNullException_WhenOrderIdIsEmpty()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _orderService.CompleteOrderAsync(Guid.Empty, _mockTruckOrderService.Object, _mockTruckService.Object));
        }

        [Test]
        public async Task CompleteOrderAsync_ShouldThrowArgumentNullException_WhenOrderNotFound()
        {
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await _orderService.CompleteOrderAsync(Guid.NewGuid(), _mockTruckOrderService.Object, _mockTruckService.Object));
            Assert.That(ex.ParamName, Is.EqualTo("order"));
        }


        [Test]
        public async Task CompleteOrderAsync_ShouldThrowArgumentNullException_WhenNoAssignedTruckOrders()
        {
            var user = await AddUser("User");
            var order = await AddOrder(user.Id, OrderStatus.InProgress);

            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _orderService.CompleteOrderAsync(order.OrderID, _mockTruckOrderService.Object, _mockTruckService.Object));
            Assert.That(ex.ParamName, Is.EqualTo("OrderTrucks"));
        }

        //[Test]
        //public async Task CompleteOrderAsync_ShouldSetOrderStatusToCompleted_WhenDeliveryAddressMatches()
        //{
        //    var user = await AddUser("User");
        //    var driver = await AddUser("Driver");
        //    var order = await AddOrder(user.Id, OrderStatus.InProgress, false, "Matching Address");
        //    var truck = await AddTruck(driver.Id);
        //    await AddOrderTruck(order.OrderID, truck.TruckID, CourseStatus.Assigned, "Matching Address");

        //    _mockTruckService.Setup(s => s.GetAll()).Returns(new List<Truck>().AsQueryable());
        //    _mockTruckService.Setup(s => s.GetTruckStatus(It.IsAny<Guid>())).Returns(TruckStatus.Unavailable.ToString());
        //    _mockTruckService.Setup(s => s.ChangeTruckStatus(It.IsAny<Guid>(), It.IsAny<string>()));

        //    await _orderService.CompleteOrderAsync(order.OrderID, _mockTruckOrderService.Object, _mockTruckService.Object);

        //    var updatedOrder = await _context.Orders.FindAsync(order.OrderID);
        //    Assert.That(updatedOrder.Status, Is.EqualTo(OrderStatus.Completed));
        //    Assert.That(updatedOrder.DeliveryDate, Is.Not.Null);

        //    _mockTruckOrderService.Verify(s => s.CompleteTruckOrderAsync(order.OrderID), Times.Once);
        //    _mockNotificationService.Verify(s => s.AddAsync(It.Is<Notification>(n =>
        //        n.Title == $"Order status changed to Completed" &&
        //        n.ReceiverID == user.Id)), Times.Once);
        //    _mockTruckService.Verify(s => s.ChangeTruckStatus(truck.TruckID, TruckStatus.Available.ToString()), Times.Once);
        //}

        //[Test]
        //public async Task CompleteOrderAsync_ShouldSetOrderStatusToOnHold_WhenDeliveryAddressDoesNotMatch()
        //{
        //    var user = await AddUser("User");
        //    var driver = await AddUser("Driver");
        //    var order = await AddOrder(user.Id, OrderStatus.InProgress, false, "Order Delivery Address");
        //    var truck = await AddTruck(driver.Id);
        //    await AddOrderTruck(order.OrderID, truck.TruckID, CourseStatus.Assigned, "Different Delivery Address");

        //    _mockTruckService.Setup(s => s.GetAll()).Returns(new List<Truck>().AsQueryable());
        //    _mockTruckService.Setup(s => s.GetTruckStatus(It.IsAny<Guid>())).Returns(TruckStatus.Unavailable.ToString());
        //    _mockTruckService.Setup(s => s.ChangeTruckStatus(It.IsAny<Guid>(), It.IsAny<string>()));


        //    await _orderService.CompleteOrderAsync(order.OrderID, _mockTruckOrderService.Object, _mockTruckService.Object);

        //    var updatedOrder = await _context.Orders.FindAsync(order.OrderID);
        //    Assert.That(updatedOrder.Status, Is.EqualTo(OrderStatus.OnHold));
        //    Assert.That(updatedOrder.DeliveryDate, Is.Null);

        //    _mockTruckOrderService.Verify(s => s.CompleteTruckOrderAsync(order.OrderID), Times.Once);
        //    _mockNotificationService.Verify(s => s.AddAsync(It.Is<Notification>(n =>
        //        n.Title == $"Order status changed to OnHold" &&
        //        n.ReceiverID == user.Id)), Times.Once);
        //    _mockTruckService.Verify(s => s.ChangeTruckStatus(truck.TruckID, TruckStatus.Available.ToString()), Times.Once);
        //}

        //[Test]
        //public async Task CompleteOrderAsync_ShouldNotChangeTruckStatus_IfOtherAssignOrdersExistForTruck()
        //{
        //    var user = await AddUser("User");
        //    var driver = await AddUser("Driver");
        //    var order1 = await AddOrder(user.Id, OrderStatus.InProgress, false, "Address1");
        //    var order2 = await AddOrder(user.Id, OrderStatus.InProgress, false, "Address2");
        //    var truck = await AddTruck(driver.Id);
        //    await AddOrderTruck(order1.OrderID, truck.TruckID, CourseStatus.Assigned, "Address1");
        //    await AddOrderTruck(order2.OrderID, truck.TruckID, CourseStatus.Assigned, "Address2");

        //    var truckWithOrders = new Truck
        //    {
        //        TruckID = truck.TruckID,
        //        Status = TruckStatus.Unavailable,
        //        TruckOrders = new List<CourseOrder>
        //                    {
        //                        new CourseOrder { TruckID = truck.TruckID, Status = CourseStatus.Assigned, OrderID = order2.OrderID }
        //                    }
        //    };

        //    _mockTruckService.Setup(s => s.GetAll())
        //        .Returns(new List<Truck> { truckWithOrders }.AsQueryable());

        //    _mockTruckService.Setup(s => s.GetTruckStatus(It.IsAny<Guid>())).Returns(TruckStatus.Unavailable.ToString());
        //    _mockTruckService.Setup(s => s.ChangeTruckStatus(It.IsAny<Guid>(), It.IsAny<string>()));

        //    await _orderService.CompleteOrderAsync(order1.OrderID, _mockTruckOrderService.Object, _mockTruckService.Object);

        //    var updatedOrder = await _context.Orders.FindAsync(order1.OrderID);
        //    Assert.That(updatedOrder.Status, Is.EqualTo(OrderStatus.Completed));

        //    _mockTruckOrderService.Verify(s => s.CompleteTruckOrderAsync(order1.OrderID), Times.Once);
        //    _mockNotificationService.Verify(s => s.AddAsync(It.IsAny<Notification>()), Times.Once);
        //    _mockTruckService.Verify(s => s.ChangeTruckStatus(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        //}

        //[Test]
        //public async Task CompleteOrderAsync_ShouldChangeTruckStatusToAvailable_IfNoOtherAssignedOrdersForTruck()
        //{
        //    var user = await AddUser("User");
        //    var driver = await AddUser("Driver");
        //    var order = await AddOrder(user.Id, OrderStatus.InProgress, false, "Address1");
        //    var truck = await AddTruck(driver.Id);
        //    await AddOrderTruck(order.OrderID, truck.TruckID, CourseStatus.Assigned, "Address1");

        //    _mockTruckService.Setup(s => s.GetAll()).Returns(new List<Truck>().AsQueryable());
        //    _mockTruckService.Setup(s => s.GetTruckStatus(It.IsAny<Guid>())).Returns(TruckStatus.Unavailable.ToString());
        //    _mockTruckService.Setup(s => s.ChangeTruckStatus(It.IsAny<Guid>(), It.IsAny<string>()));

        //    await _orderService.CompleteOrderAsync(order.OrderID, _mockTruckOrderService.Object, _mockTruckService.Object);

        //    var updatedOrder = await _context.Orders.FindAsync(order.OrderID);
        //    Assert.That(updatedOrder.Status, Is.EqualTo(OrderStatus.Completed));

        //    _mockTruckOrderService.Verify(s => s.CompleteTruckOrderAsync(order.OrderID), Times.Once);
        //    _mockNotificationService.Verify(s => s.AddAsync(It.IsAny<Notification>()), Times.Once);
        //    _mockTruckService.Verify(s => s.ChangeTruckStatus(truck.TruckID, TruckStatus.Available.ToString()), Times.Once);
        //}
    }
}
