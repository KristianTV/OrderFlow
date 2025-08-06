using Microsoft.EntityFrameworkCore;
using MockQueryable;
using Moq;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services.Core;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Truck;

namespace OrderFlow.Tests.Services
{
    [TestFixture]
    public class TruckOrderServiceTests
    {
        private OrderFlowDbContext _context;
        private TruckOrderService _truckOrderService;
        private Mock<IOrderService> _mockOrderService;
        private Mock<INotificationService> _mockNotificationService;
        private Mock<ITruckService> _mockTruckService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<OrderFlowDbContext>()
              .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
              .Options;
            _context = new OrderFlowDbContext(options);

            _mockOrderService = new Mock<IOrderService>();
            _mockNotificationService = new Mock<INotificationService>();
            _mockTruckService = new Mock<ITruckService>();

            _truckOrderService = new TruckOrderService(
              _context,
              _mockOrderService.Object,
              _mockNotificationService.Object,
              _mockTruckService.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void GetAll_ReturnsAllTruckOrders()
        {
            _context.TrucksOrders.Add(new TruckOrder { TruckOrderId = Guid.NewGuid(), TruckID = Guid.NewGuid(), OrderID = Guid.NewGuid(), Status = TruckOrderStatus.Assigned });
            _context.TrucksOrders.Add(new TruckOrder { TruckOrderId = Guid.NewGuid(), TruckID = Guid.NewGuid(), OrderID = Guid.NewGuid(), Status = TruckOrderStatus.Delivered });
            _context.SaveChanges();

            var result = _truckOrderService.GetAll().IgnoreQueryFilters().ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void GetAll_ReturnsEmptyCollection_WhenNoTruckOrdersExist()
        {
            var result = _truckOrderService.GetAll();

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task AssignOrdersToTruckAsync_SuccessfullyAssignsOrders()
        {
            var truckId = Guid.NewGuid();
            var driverId = Guid.NewGuid();
            var orderId1 = Guid.NewGuid();
            var orderId2 = Guid.NewGuid();

            var ordersToAssign = new List<OrderViewModel>
            {
                new OrderViewModel { OrderID = orderId1, DeliveryAddress = "Address 1" },
                new OrderViewModel { OrderID = orderId2, DeliveryAddress = "Address 2" }
            };

            var trucks = new List<Truck> { new Truck { TruckID = truckId, Capacity = 100, DriverID = driverId, Driver = new ApplicationUser { Id = driverId }, isDeleted = false } };

            _context.Trucks.AddRange(trucks);
            _context.SaveChanges();

            _mockTruckService.Setup(s => s.GetAll())
                .Returns(trucks.BuildMock());
            _mockTruckService.Setup(s => s.GetTruckStatus(truckId)).Returns(TruckStatus.Available.ToString());
            _mockTruckService.Setup(s => s.ChangeTruckStatus(truckId, TruckStatus.Unavailable.ToString()));

            var orders = new List<Order>
            {
                new Order { OrderID = orderId1, LoadCapacity = 30 },
                new Order { OrderID = orderId2, LoadCapacity = 40 }
            };
            _context.Orders.AddRange(orders);
            _context.SaveChanges();

            _mockOrderService.Setup(s => s.GetAll())
                .Returns(orders.BuildMock());
            _mockOrderService.Setup(s => s.ChangeOrderStatusAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            _mockNotificationService.Setup(s => s.AddAsync(It.IsAny<Notification>()))
                .Returns(Task.CompletedTask);

            var result = await _truckOrderService.AssignOrdersToTruckAsync(ordersToAssign, truckId);

            Assert.Greater(result, 0);
            Assert.That(_context.TrucksOrders.ToList().Count(), Is.EqualTo(2));
            Assert.IsTrue(_context.TrucksOrders.Any(to => to.OrderID == orderId1 && to.TruckID == truckId && to.Status == TruckOrderStatus.Assigned));
            Assert.IsTrue(_context.TrucksOrders.Any(to => to.OrderID == orderId2 && to.TruckID == truckId && to.Status == TruckOrderStatus.Assigned));

            _mockOrderService.Verify(s => s.ChangeOrderStatusAsync(orderId1, OrderStatus.InProgress.ToString()), Times.Once);
            _mockOrderService.Verify(s => s.ChangeOrderStatusAsync(orderId2, OrderStatus.InProgress.ToString()), Times.Once);
            _mockNotificationService.Verify(s => s.AddAsync(It.Is<Notification>(n =>
                n.Title == "Orders Assigned" &&
                n.Message == $"Orders have been assigned to truck {truckId}." &&
                n.TruckId == truckId
            )), Times.Once);
            _mockTruckService.Verify(s => s.ChangeTruckStatus(truckId, TruckStatus.Unavailable.ToString()), Times.Once);
        }

        [Test]
        public void AssignOrdersToTruckAsync_ThrowsArgumentNullException_WhenAssignOrdersIsNull()
        {
            IEnumerable<OrderViewModel> assignOrders = null;
            var truckId = Guid.NewGuid();

            Assert.ThrowsAsync<ArgumentNullException>(() => _truckOrderService.AssignOrdersToTruckAsync(assignOrders, truckId));
        }

        [Test]
        public void AssignOrdersToTruckAsync_ThrowsArgumentNullException_WhenAssignOrdersIsEmpty()
        {
            var assignOrders = new List<OrderViewModel>();
            var truckId = Guid.NewGuid();

            Assert.ThrowsAsync<ArgumentNullException>(() => _truckOrderService.AssignOrdersToTruckAsync(assignOrders, truckId));
        }

        [Test]
        public async Task AssignOrdersToTruckAsync_ThrowsInvalidOperationException_WhenTruckCapacityIsFull()
        {
            var truckId = Guid.NewGuid();
            var orderId = Guid.NewGuid();

            var ordersToAssign = new List<OrderViewModel>
            {
                new OrderViewModel { OrderID = orderId, DeliveryAddress = "Address 1" }
            };

            var trucks = new List<Truck> { new Truck { TruckID = truckId, Capacity = 10 } };

            _mockTruckService.Setup(s => s.GetAll())
                .Returns(trucks.BuildMock());

            var orders = new List<Order> { new Order { OrderID = orderId, LoadCapacity = 20 } };
            _mockOrderService.Setup(s => s.GetAll())
                .Returns(orders.BuildMock());

            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _truckOrderService.AssignOrdersToTruckAsync(ordersToAssign, truckId));
            Assert.That(ex.Message, Is.EqualTo("Truck capacity is full."));
        }

        [Test]
        public async Task AssignOrdersToTruckAsync_SkipsAlreadyAssignedOrders()
        {
            var truckId = Guid.NewGuid();
            var driverId = Guid.NewGuid();
            var orderId1 = Guid.NewGuid();
            var orderId2 = Guid.NewGuid();

            _context.TrucksOrders.Add(new TruckOrder { TruckOrderId = Guid.NewGuid(), OrderID = orderId1, TruckID = truckId, DeliverAddress = "Address 1", Status = TruckOrderStatus.Assigned });
            await _context.SaveChangesAsync();

            var ordersToAssign = new List<OrderViewModel>
            {
                new OrderViewModel { OrderID = orderId1, DeliveryAddress = "Address 1" },
                new OrderViewModel { OrderID = orderId2, DeliveryAddress = "Address 2" }
            };

            var trucks = new List<Truck> { new Truck { TruckID = truckId, Capacity = 100, DriverID = driverId, Driver = new ApplicationUser { Id = driverId } } };
            _context.Trucks.AddRange(trucks);
            _context.SaveChanges();

            _mockTruckService.Setup(s => s.GetAll())
                .Returns(trucks.BuildMock());
            _mockTruckService.Setup(s => s.GetTruckStatus(truckId)).Returns(TruckStatus.Available.ToString());
            _mockTruckService.Setup(s => s.ChangeTruckStatus(truckId, TruckStatus.Unavailable.ToString()));

            var orders = new List<Order>
            {
                new Order { OrderID = orderId1, LoadCapacity = 30 },
                new Order { OrderID = orderId2, LoadCapacity = 40 }
            };
            _context.Orders.AddRange(orders);
            _context.SaveChanges();
            _mockOrderService.Setup(s => s.GetAll())
                .Returns(orders.BuildMock());
            _mockOrderService.Setup(s => s.ChangeOrderStatusAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            _mockNotificationService.Setup(s => s.AddAsync(It.IsAny<Notification>()))
                .Returns(Task.CompletedTask);

            var result = await _truckOrderService.AssignOrdersToTruckAsync(ordersToAssign, truckId);

            Assert.Greater(result, 0);
            Assert.That(_context.TrucksOrders.Count(), Is.EqualTo(2));
            Assert.IsTrue(_context.TrucksOrders.Any(to => to.OrderID == orderId1 && to.TruckID == truckId && to.DeliverAddress == "Address 1"));
            Assert.IsTrue(_context.TrucksOrders.Any(to => to.OrderID == orderId2 && to.TruckID == truckId && to.DeliverAddress == "Address 2"));

            _mockOrderService.Verify(s => s.ChangeOrderStatusAsync(orderId1, It.IsAny<string>()), Times.Never);
            _mockOrderService.Verify(s => s.ChangeOrderStatusAsync(orderId2, OrderStatus.InProgress.ToString()), Times.Once);
            _mockNotificationService.Verify(s => s.AddAsync(It.IsAny<Notification>()), Times.Once);
        }

        [Test]
        public async Task AssignOrdersToTruckAsync_DoesNotChangeTruckStatus_IfAlreadyUnavailable()
        {
            var truckId = Guid.NewGuid();
            var driverId = Guid.NewGuid();
            var orderId = Guid.NewGuid();

            var ordersToAssign = new List<OrderViewModel>
            {
                new OrderViewModel { OrderID = orderId, DeliveryAddress = "Address 1" }
            };

            var trucks = new List<Truck> { new Truck { TruckID = truckId, Capacity = 100, DriverID = driverId, Driver = new ApplicationUser { Id = driverId } } };
            var orders = new List<Order> { new Order { OrderID = orderId, LoadCapacity = 30 } };

            _mockTruckService.Setup(s => s.GetAll()).Returns(trucks.BuildMock());
            _mockOrderService.Setup(s => s.GetAll()).Returns(orders.BuildMock());

            _mockTruckService.Setup(s => s.GetTruckStatus(truckId)).Returns(TruckStatus.Unavailable.ToString());
            _mockOrderService.Setup(s => s.ChangeOrderStatusAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));
            _mockNotificationService.Setup(s => s.AddAsync(It.IsAny<Notification>()))
                .Returns(Task.CompletedTask);

            await _truckOrderService.AssignOrdersToTruckAsync(ordersToAssign, truckId);

            _mockTruckService.Verify(s => s.ChangeTruckStatus(truckId, TruckStatus.Unavailable.ToString()), Times.Never);
        }

        [Test]
        public async Task AssignOrdersToTruckAsync_NoNotificationOrStatusChange_IfNoOrdersAdded()
        {
            var truckId = Guid.NewGuid();
            var orderId1 = Guid.NewGuid();

            _context.TrucksOrders.Add(new TruckOrder { TruckOrderId = Guid.NewGuid(), OrderID = orderId1, TruckID = truckId, DeliverAddress = "Address 1", Status = TruckOrderStatus.Assigned });
            await _context.SaveChangesAsync();

            var ordersToAssign = new List<OrderViewModel>
            {
                new OrderViewModel { OrderID = orderId1, DeliveryAddress = "Address 1" }
            };

            var trucks = new List<Truck> { new Truck { TruckID = truckId, Capacity = 100 } };
            _context.Trucks.AddRange(trucks);
            _context.SaveChanges();
            _mockTruckService.Setup(s => s.GetAll())
                .Returns(trucks.BuildMock());
            _mockTruckService.Setup(s => s.GetTruckStatus(truckId)).Returns(TruckStatus.Available.ToString());

            var orders = new List<Order> { new Order { OrderID = orderId1, LoadCapacity = 30 } };
            _context.Orders.AddRange(orders);
            _context.SaveChanges();
            _mockOrderService.Setup(s => s.GetAll())
                .Returns(orders.BuildMock());

            var result = await _truckOrderService.AssignOrdersToTruckAsync(ordersToAssign, truckId);

            Assert.That(result, Is.EqualTo(0));
            _mockNotificationService.Verify(s => s.AddAsync(It.IsAny<Notification>()), Times.Never);
            _mockTruckService.Verify(s => s.ChangeTruckStatus(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task RemoveOrderFromTruckAsync_SuccessfullyRemovesOrder()
        {
            var truckId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var driverId = Guid.NewGuid();

            _context.TrucksOrders.Add(new TruckOrder
            {
                TruckOrderId = Guid.NewGuid(),
                OrderID = orderId,
                TruckID = truckId,
                DeliverAddress = "Address 1",
                Status = TruckOrderStatus.Assigned,
                Truck = new Truck { TruckID = truckId, DriverID = driverId, Driver = new ApplicationUser { Id = driverId } }
            });
            await _context.SaveChangesAsync();

            _mockOrderService.Setup(s => s.ChangeOrderStatusAsync(orderId, OrderStatus.Pending.ToString()))
                .Returns(Task.FromResult(true));
            _mockNotificationService.Setup(s => s.AddAsync(It.IsAny<Notification>()))
                .Returns(Task.CompletedTask);
            _mockTruckService.Setup(s => s.GetTruckStatus(truckId)).Returns(TruckStatus.Unavailable.ToString());
            _mockTruckService.Setup(s => s.ChangeTruckStatus(truckId, TruckStatus.Available.ToString()));

            await _truckOrderService.RemoveOrderFromTruckAsync(truckId, orderId);

            Assert.IsFalse(_context.TrucksOrders.Any(to => to.OrderID == orderId && to.TruckID == truckId && to.Status == TruckOrderStatus.Assigned));
            _mockOrderService.Verify(s => s.ChangeOrderStatusAsync(orderId, OrderStatus.Pending.ToString()), Times.Once);
            _mockNotificationService.Verify(s => s.AddAsync(It.Is<Notification>(n =>
                n.Title == "Order Remove" &&
                n.Message == $"Order {orderId} have been remove from truck {truckId}." &&
                n.OrderId == orderId &&
                n.ReceiverId == driverId &&
                n.TruckId == truckId
            )), Times.Once);
            _mockTruckService.Verify(s => s.ChangeTruckStatus(truckId, TruckStatus.Available.ToString()), Times.Once);
        }

        [Test]
        public void RemoveOrderFromTruckAsync_ThrowsArgumentException_WhenTruckIdIsEmpty()
        {
            var emptyTruckId = Guid.Empty;
            var orderId = Guid.NewGuid();

            Assert.ThrowsAsync<ArgumentException>(() => _truckOrderService.RemoveOrderFromTruckAsync(emptyTruckId, orderId));
        }

        [Test]
        public void RemoveOrderFromTruckAsync_ThrowsArgumentException_WhenOrderIdIsEmpty()
        {
            var truckId = Guid.NewGuid();
            var emptyOrderId = Guid.Empty;

            Assert.ThrowsAsync<ArgumentException>(() => _truckOrderService.RemoveOrderFromTruckAsync(truckId, emptyOrderId));
        }

        [Test]
        public async Task RemoveOrderFromTruckAsync_DoesNothing_WhenTruckOrderDoesNotExist()
        {
            var truckId = Guid.NewGuid();
            var orderId = Guid.NewGuid();

            await _truckOrderService.RemoveOrderFromTruckAsync(truckId, orderId);

            _mockOrderService.Verify(s => s.ChangeOrderStatusAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
            _mockNotificationService.Verify(s => s.AddAsync(It.IsAny<Notification>()), Times.Never);
            _mockTruckService.Verify(s => s.ChangeTruckStatus(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task RemoveOrderFromTruckAsync_DoesNotChangeTruckStatus_IfOtherOrdersAreAssigned()
        {
            var truckId = Guid.NewGuid();
            var orderIdToRemove = Guid.NewGuid();
            var otherOrderId = Guid.NewGuid();
            var driverId = Guid.NewGuid();

            _context.TrucksOrders.Add(new TruckOrder
            {
                TruckOrderId = Guid.NewGuid(),
                OrderID = orderIdToRemove,
                TruckID = truckId,
                DeliverAddress = "Address 1",
                Status = TruckOrderStatus.Assigned,
                Truck = new Truck { TruckID = truckId, DriverID = driverId, Driver = new ApplicationUser { Id = driverId } }
            });
            _context.TrucksOrders.Add(new TruckOrder
            {
                TruckOrderId = Guid.NewGuid(),
                OrderID = otherOrderId,
                TruckID = truckId,
                DeliverAddress = "Address 2",
                Status = TruckOrderStatus.Assigned
            });
            await _context.SaveChangesAsync();

            _mockOrderService.Setup(s => s.ChangeOrderStatusAsync(orderIdToRemove, OrderStatus.Pending.ToString()))
                .Returns(Task.FromResult(true));
            _mockNotificationService.Setup(s => s.AddAsync(It.IsAny<Notification>()))
                .Returns(Task.CompletedTask);
            _mockTruckService.Setup(s => s.GetTruckStatus(truckId)).Returns(TruckStatus.Unavailable.ToString());
            _mockTruckService.Setup(s => s.ChangeTruckStatus(truckId, TruckStatus.Available.ToString()));

            await _truckOrderService.RemoveOrderFromTruckAsync(truckId, orderIdToRemove);

            Assert.IsFalse(_context.TrucksOrders.Any(to => to.OrderID == orderIdToRemove && to.TruckID == truckId && to.Status == TruckOrderStatus.Assigned));
            Assert.IsTrue(_context.TrucksOrders.Any(to => to.OrderID == otherOrderId && to.TruckID == truckId && to.Status == TruckOrderStatus.Assigned));
            _mockTruckService.Verify(s => s.ChangeTruckStatus(truckId, TruckStatus.Available.ToString()), Times.Never);
        }

        [Test]
        public async Task CompleteTruckOrderAsync_SuccessfullyCompletesOrder()
        {
            var orderId = Guid.NewGuid();
            var truckId = Guid.NewGuid();
            var driverId = Guid.NewGuid();

            _context.TrucksOrders.Add(new TruckOrder
            {
                TruckOrderId = Guid.NewGuid(),
                OrderID = orderId,
                TruckID = truckId,
                Status = TruckOrderStatus.Assigned,
                Truck = new Truck { TruckID = truckId, DriverID = driverId, Driver = new ApplicationUser { Id = driverId } }
            });
            await _context.SaveChangesAsync();

            _mockNotificationService.Setup(s => s.AddAsync(It.IsAny<Notification>()))
                .Returns(Task.CompletedTask);

            await _truckOrderService.CompleteTruckOrderAsync(orderId);

            var completedOrder = await _context.TrucksOrders.SingleOrDefaultAsync(to => to.OrderID == orderId);
            Assert.IsNotNull(completedOrder);
            Assert.That(completedOrder.Status, Is.EqualTo(TruckOrderStatus.Delivered));
            Assert.That(completedOrder.DeliveryDate, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(5)));

            _mockNotificationService.Verify(s => s.AddAsync(It.Is<Notification>(n =>
                n.Title == $"Order {orderId} has been delivered" &&
                n.OrderId == orderId &&
                n.Message == $"Order {orderId} has been delivered" &&
                n.ReceiverId == driverId
            )), Times.Once);
        }

        [Test]
        public async Task CompleteTruckOrderAsync_DoesNothing_IfOrderIsAlreadyDelivered()
        {
            var orderId = Guid.NewGuid();
            var truckId = Guid.NewGuid();
            var driverId = Guid.NewGuid();

            _context.TrucksOrders.Add(new TruckOrder
            {
                TruckOrderId = Guid.NewGuid(),
                OrderID = orderId,
                TruckID = truckId,
                Status = TruckOrderStatus.Delivered,
                Truck = new Truck { TruckID = truckId, DriverID = driverId, Driver = new ApplicationUser { Id = driverId } }
            });
            await _context.SaveChangesAsync();

            await _truckOrderService.CompleteTruckOrderAsync(orderId);

            var order = await _context.TrucksOrders.SingleOrDefaultAsync(to => to.OrderID == orderId);
            Assert.IsNotNull(order);
            Assert.AreEqual(TruckOrderStatus.Delivered, order.Status);
            _mockNotificationService.Verify(s => s.AddAsync(It.IsAny<Notification>()), Times.Never);
        }

        [Test]
        public async Task CompleteTruckOrderAsync_DoesNothing_WhenTruckOrderDoesNotExist()
        {
            var nonExistentOrderId = Guid.NewGuid();

            await _truckOrderService.CompleteTruckOrderAsync(nonExistentOrderId);

            _mockNotificationService.Verify(s => s.AddAsync(It.IsAny<Notification>()), Times.Never);
        }

        [Test]
        public async Task AssignOrdersToTruckAsync_HandlesZeroCapacityTruck()
        {
            var truckId = Guid.NewGuid();
            var orderId = Guid.NewGuid();

            var ordersToAssign = new List<OrderViewModel>
            {
                new OrderViewModel { OrderID = orderId, DeliveryAddress = "Address 1" }
            };

            var trucks = new List<Truck> { new Truck { TruckID = truckId, Capacity = 0 } };
            _mockTruckService.Setup(s => s.GetAll())
                .Returns(trucks.BuildMock());

            var orders = new List<Order> { new Order { OrderID = orderId, LoadCapacity = 10 } };
            _mockOrderService.Setup(s => s.GetAll())
                .Returns(orders.BuildMock());

            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _truckOrderService.AssignOrdersToTruckAsync(ordersToAssign, truckId));
            Assert.AreEqual("Truck capacity is full.", ex.Message);
        }

        [Test]
        public async Task AssignOrdersToTruckAsync_HandlesTruckWithNoDriver()
        {
            var truckId = Guid.NewGuid();
            var orderId = Guid.NewGuid();

            var ordersToAssign = new List<OrderViewModel>
            {
                new OrderViewModel { OrderID = orderId, DeliveryAddress = "Address 1" }
            };

            var trucks = new List<Truck> { new Truck { TruckID = truckId, Capacity = 100, DriverID = Guid.Empty } };
            _mockTruckService.Setup(s => s.GetAll())
                .Returns(trucks.BuildMock());
            _mockTruckService.Setup(s => s.GetTruckStatus(truckId)).Returns(TruckStatus.Available.ToString());
            _mockTruckService.Setup(s => s.ChangeTruckStatus(truckId, TruckStatus.Unavailable.ToString()));

            var orders = new List<Order> { new Order { OrderID = orderId, LoadCapacity = 30 } };
            _mockOrderService.Setup(s => s.GetAll())
                .Returns(orders.BuildMock());
            _mockOrderService.Setup(s => s.ChangeOrderStatusAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            _mockNotificationService.Setup(s => s.AddAsync(It.IsAny<Notification>()))
                .Returns(Task.CompletedTask);

            var result = await _truckOrderService.AssignOrdersToTruckAsync(ordersToAssign, truckId);

            Assert.Greater(result, 0);
            Assert.AreEqual(1, _context.TrucksOrders.IgnoreQueryFilters().Count());
            _mockNotificationService.Verify(s => s.AddAsync(It.Is<Notification>(n =>
                n.SenderId == null
            )), Times.Once);
        }

        [Test]
        public async Task RemoveOrderFromTruckAsync_HandlesTruckWithNoDriver()
        {
            var truckId = Guid.NewGuid();
            var orderId = Guid.NewGuid();

            _context.TrucksOrders.Add(new TruckOrder
            {
                TruckOrderId = Guid.NewGuid(),
                OrderID = orderId,
                TruckID = truckId,
                DeliverAddress = "Address 1",
                Status = TruckOrderStatus.Assigned,
                Truck = new Truck { TruckID = truckId, DriverID = Guid.Empty }
            });
            await _context.SaveChangesAsync();

            _mockOrderService.Setup(s => s.ChangeOrderStatusAsync(orderId, OrderStatus.Pending.ToString()))
                .Returns(Task.FromResult(true));
            _mockNotificationService.Setup(s => s.AddAsync(It.IsAny<Notification>()))
                .Returns(Task.CompletedTask);
            _mockTruckService.Setup(s => s.GetTruckStatus(truckId)).Returns(TruckStatus.Unavailable.ToString());
            _mockTruckService.Setup(s => s.ChangeTruckStatus(truckId, TruckStatus.Available.ToString()));

            await _truckOrderService.RemoveOrderFromTruckAsync(truckId, orderId);

            _mockNotificationService.Verify(s => s.AddAsync(It.Is<Notification>(n =>
                n.SenderId == null
            )), Times.Once);
        }

        [Test]
        public async Task CompleteTruckOrderAsync_HandlesTruckWithNoDriver()
        {
            var orderId = Guid.NewGuid();
            var truckId = Guid.NewGuid();

            _context.TrucksOrders.Add(new TruckOrder
            {
                TruckOrderId = Guid.NewGuid(),
                OrderID = orderId,
                TruckID = truckId,
                Status = TruckOrderStatus.Assigned,
                Truck = new Truck { TruckID = truckId, DriverID = Guid.Empty }
            });
            await _context.SaveChangesAsync();

            _mockNotificationService.Setup(s => s.AddAsync(It.IsAny<Notification>()))
                .Returns(Task.CompletedTask);

            await _truckOrderService.CompleteTruckOrderAsync(orderId);

            _mockNotificationService.Verify(s => s.AddAsync(It.Is<Notification>(n =>
                n.SenderId == null
            )), Times.Never);
        }
    }
}