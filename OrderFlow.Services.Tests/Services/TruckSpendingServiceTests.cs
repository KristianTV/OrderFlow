using Microsoft.EntityFrameworkCore;
using Moq;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services.Contracts;
using OrderFlow.Services.Core;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.TruckSpending;

namespace OrderFlow.Tests.Services
{
    [TestFixture]
    public class TruckSpendingServiceTests
    {
        private OrderFlowDbContext _context = null!;
        private TruckSpendingService _truckSpendingService = null!;
        private Guid _driverId;
        private Guid _otherDriverId;
        private Guid _truckId;
        private Guid _otherTruckId;
        private Guid _courseId;
        private Guid _otherCourseId;

        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<OrderFlowDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new OrderFlowDbContext(options);
            var truckService = new TruckService(_context);
            var truckCourseService = new TruckCourseService(
                _context,
                new Mock<IOrderService>().Object,
                new Mock<INotificationService>().Object,
                truckService,
                new Mock<ICourseOrderService>().Object,
                new Mock<IPaymentService>().Object);
            _truckSpendingService = new TruckSpendingService(_context, truckService, truckCourseService);

            await SeedDataAsync();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void GetAll_ReturnsAllTruckSpendings()
        {
            var result = _truckSpendingService.GetAll().ToList();

            Assert.That(result, Has.Count.EqualTo(3));
        }

        [Test]
        public async Task GetTruckSpendingsAsync_FiltersByDriverTruckCourseSearchAndProjectsDisplayFields()
        {
            var result = (await _truckSpendingService.GetTruckSpendingsAsync(_driverId, new TruckSpendingQueryModel
            {
                TruckID = _truckId,
                TruckCourseID = _courseId,
                Search = "Fuel",
                SortOrder = "amount_desc"
            })).ToList();

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].TruckID, Is.EqualTo(_truckId));
            Assert.That(result[0].TruckCourseID, Is.EqualTo(_courseId));
            Assert.That(result[0].TruckLicensePlate, Is.EqualTo("DRV-001"));
            Assert.That(result[0].CourseDescription, Does.Contain("Depot -> Client"));
            Assert.That(result[0].PaymentMethod, Is.EqualTo(PaymentMethods.Cash.ToString()));
        }

        [Test]
        public async Task GetTruckSpendingsAsync_UsesSafePagingDefaults()
        {
            var result = (await _truckSpendingService.GetTruckSpendingsAsync(null, new TruckSpendingQueryModel
            {
                Page = -5,
                PageSize = 0
            })).ToList();

            Assert.That(result, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task GetTruckSpendingDetailsAsync_ReturnsNullForEmptyOrUnauthorizedId()
        {
            var otherSpending = await _context.TrucksSpendings.SingleAsync(spending => spending.TruckID == _otherTruckId);

            Assert.That(await _truckSpendingService.GetTruckSpendingDetailsAsync(Guid.Empty, _driverId), Is.Null);
            Assert.That(await _truckSpendingService.GetTruckSpendingDetailsAsync(otherSpending.TruckSpendingID, _driverId), Is.Null);
        }

        [Test]
        public async Task GetTruckSpendingDetailsAsync_ReturnsProjectedDetailsForAuthorizedDriver()
        {
            var spending = await _context.TrucksSpendings.SingleAsync(s => s.TruckID == _truckId && s.PaymentDescription.Contains("Fuel"));

            var result = await _truckSpendingService.GetTruckSpendingDetailsAsync(spending.TruckSpendingID, _driverId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.TruckSpendingID, Is.EqualTo(spending.TruckSpendingID));
            Assert.That(result.TruckLicensePlate, Is.EqualTo("DRV-001"));
            Assert.That(result.CourseDescription, Does.Contain("Depot -> Client"));
            Assert.That(result.PaymentDescription, Is.EqualTo("Fuel stop"));
        }

        [Test]
        public async Task GetTruckSpendingForEditAsync_ReturnsModelOnlyWhenAuthorized()
        {
            var spending = await _context.TrucksSpendings.SingleAsync(s => s.TruckID == _truckId && s.PaymentDescription.Contains("Fuel"));

            var result = await _truckSpendingService.GetTruckSpendingForEditAsync(spending.TruckSpendingID, _driverId);
            var unauthorized = await _truckSpendingService.GetTruckSpendingForEditAsync(spending.TruckSpendingID, _otherDriverId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.TruckID, Is.EqualTo(_truckId));
            Assert.That(result.Amount, Is.EqualTo(100m));
            Assert.That(unauthorized, Is.Null);
        }

        [Test]
        public async Task CreateTruckSpendingAsync_AddsSpending_WhenTruckAndCourseAreAllowed()
        {
            var result = await _truckSpendingService.CreateTruckSpendingAsync(new CreateTruckSpendingViewModel
            {
                TruckID = _truckId,
                TruckCourseID = _courseId,
                Amount = 75m,
                PaymentDate = DateTime.UtcNow.Date,
                SpendingType = TruckSpendingsType.Tolls,
                PaymentDescription = "Road tax",
                PaymentMethod = PaymentMethods.Card
            }, _driverId);

            var created = await _context.TrucksSpendings.SingleAsync(s => s.PaymentDescription == "Road tax");
            Assert.That(result, Is.True);
            Assert.That(created.TruckID, Is.EqualTo(_truckId));
            Assert.That(created.TruckCourseID, Is.EqualTo(_courseId));
            Assert.That(created.PaymentMethod, Is.EqualTo(PaymentMethods.Card));
        }

        [Test]
        public async Task CreateTruckSpendingAsync_ReturnsFalseForInvalidModelOrUnauthorizedRelations()
        {
            Assert.That(await _truckSpendingService.CreateTruckSpendingAsync(null!, _driverId), Is.False);
            Assert.That(await _truckSpendingService.CreateTruckSpendingAsync(new CreateTruckSpendingViewModel { TruckID = Guid.Empty, Amount = 10m }, _driverId), Is.False);
            Assert.That(await _truckSpendingService.CreateTruckSpendingAsync(new CreateTruckSpendingViewModel { TruckID = _truckId, Amount = 0m }, _driverId), Is.False);
            Assert.That(await _truckSpendingService.CreateTruckSpendingAsync(new CreateTruckSpendingViewModel { TruckID = _otherTruckId, Amount = 10m }, _driverId), Is.False);
            Assert.That(await _truckSpendingService.CreateTruckSpendingAsync(new CreateTruckSpendingViewModel { TruckID = _truckId, TruckCourseID = _otherCourseId, Amount = 10m }, _driverId), Is.False);
        }

        [Test]
        public async Task UpdateTruckSpendingAsync_UpdatesAuthorizedSpending()
        {
            var spending = await _context.TrucksSpendings.SingleAsync(s => s.TruckID == _truckId && s.PaymentDescription.Contains("Fuel"));

            var result = await _truckSpendingService.UpdateTruckSpendingAsync(spending.TruckSpendingID, new CreateTruckSpendingViewModel
            {
                TruckID = _truckId,
                TruckCourseID = null,
                Amount = 125m,
                PaymentDate = DateTime.UtcNow.Date.AddDays(1),
                SpendingType = TruckSpendingsType.Maintenance,
                PaymentDescription = "Updated service",
                PaymentMethod = PaymentMethods.Card
            }, _driverId);

            var updated = await _context.TrucksSpendings.FindAsync(spending.TruckSpendingID);
            Assert.That(result, Is.True);
            Assert.That(updated!.Amount, Is.EqualTo(125m));
            Assert.That(updated.TruckCourseID, Is.Null);
            Assert.That(updated.SpendingType, Is.EqualTo(TruckSpendingsType.Maintenance));
        }

        [Test]
        public async Task UpdateTruckSpendingAsync_ReturnsFalseForInvalidOrUnauthorizedUpdate()
        {
            var spending = await _context.TrucksSpendings.SingleAsync(s => s.TruckID == _truckId && s.PaymentDescription.Contains("Fuel"));

            Assert.That(await _truckSpendingService.UpdateTruckSpendingAsync(Guid.Empty, new CreateTruckSpendingViewModel(), _driverId), Is.False);
            Assert.That(await _truckSpendingService.UpdateTruckSpendingAsync(spending.TruckSpendingID, null!, _driverId), Is.False);
            Assert.That(await _truckSpendingService.UpdateTruckSpendingAsync(spending.TruckSpendingID, new CreateTruckSpendingViewModel { TruckID = _truckId, Amount = -1m }, _driverId), Is.False);
            Assert.That(await _truckSpendingService.UpdateTruckSpendingAsync(spending.TruckSpendingID, new CreateTruckSpendingViewModel { TruckID = _otherTruckId, Amount = 10m }, _driverId), Is.False);
            Assert.That(await _truckSpendingService.UpdateTruckSpendingAsync(spending.TruckSpendingID, new CreateTruckSpendingViewModel { TruckID = _truckId, TruckCourseID = _otherCourseId, Amount = 10m }, _driverId), Is.False);
        }

        [Test]
        public async Task DeleteTruckSpendingAsync_ReturnsTrueOnlyForExistingNonEmptyId()
        {
            var spending = await _context.TrucksSpendings.FirstAsync();

            Assert.That(await _truckSpendingService.DeleteTruckSpendingAsync(Guid.Empty), Is.False);
            Assert.That(await _truckSpendingService.DeleteTruckSpendingAsync(Guid.NewGuid()), Is.False);
            Assert.That(await _truckSpendingService.DeleteTruckSpendingAsync(spending.TruckSpendingID), Is.True);
            Assert.That(await _context.TrucksSpendings.FindAsync(spending.TruckSpendingID), Is.Null);
        }

        [Test]
        public async Task GetTruckOptionsAsync_FiltersDeletedAndDriverOwnedTrucks()
        {
            var allOptions = await _truckSpendingService.GetTruckOptionsAsync();
            var driverOptions = await _truckSpendingService.GetTruckOptionsAsync(_driverId);

            Assert.That(allOptions.Values, Is.EquivalentTo(new[] { "DRV-001", "DRV-002" }));
            Assert.That(driverOptions.Values, Is.EquivalentTo(new[] { "DRV-001" }));
        }

        [Test]
        public async Task GetCourseOptionsAsync_FiltersByTruckAndDriver()
        {
            var truckOptions = await _truckSpendingService.GetCourseOptionsAsync(_truckId);
            var driverOptions = await _truckSpendingService.GetCourseOptionsAsync(null, _driverId);

            Assert.That(truckOptions.Keys, Is.EquivalentTo(new[] { _courseId }));
            Assert.That(driverOptions.Keys, Is.EquivalentTo(new[] { _courseId }));
            Assert.That(truckOptions[_courseId], Does.Contain("Depot -> Client"));
            Assert.That(truckOptions[_courseId], Does.Contain("DRV-001"));
        }

        private async Task SeedDataAsync()
        {
            _driverId = Guid.NewGuid();
            _otherDriverId = Guid.NewGuid();
            _truckId = Guid.NewGuid();
            _otherTruckId = Guid.NewGuid();
            _courseId = Guid.NewGuid();
            _otherCourseId = Guid.NewGuid();

            var truck = new Truck
            {
                TruckID = _truckId,
                DriverID = _driverId,
                LicensePlate = "DRV-001",
                Capacity = 100,
                Status = TruckStatus.Available
            };
            var otherTruck = new Truck
            {
                TruckID = _otherTruckId,
                DriverID = _otherDriverId,
                LicensePlate = "DRV-002",
                Capacity = 100,
                Status = TruckStatus.Available
            };
            var deletedTruck = new Truck
            {
                TruckID = Guid.NewGuid(),
                DriverID = _driverId,
                LicensePlate = "DEL-001",
                Capacity = 100,
                Status = TruckStatus.Available,
                IsDeleted = true
            };
            var course = new TruckCourse
            {
                TruckCourseID = _courseId,
                TruckID = _truckId,
                Truck = truck,
                PickupAddress = "Depot",
                DeliverAddress = "Client",
                AssignedDate = DateTime.UtcNow.AddDays(-1)
            };
            var otherCourse = new TruckCourse
            {
                TruckCourseID = _otherCourseId,
                TruckID = _otherTruckId,
                Truck = otherTruck,
                PickupAddress = "Other Depot",
                DeliverAddress = "Other Client",
                AssignedDate = DateTime.UtcNow
            };

            await _context.Trucks.AddRangeAsync(truck, otherTruck, deletedTruck);
            await _context.TrucksCourses.AddRangeAsync(course, otherCourse);
            await _context.TrucksSpendings.AddRangeAsync(
                new TruckSpending
                {
                    TruckSpendingID = Guid.NewGuid(),
                    TruckID = _truckId,
                    Truck = truck,
                    TruckCourseID = _courseId,
                    TruckCourse = course,
                    Amount = 100m,
                    PaymentDate = DateTime.UtcNow.Date,
                    SpendingType = TruckSpendingsType.Fuel,
                    PaymentDescription = "Fuel stop",
                    PaymentMethod = PaymentMethods.Cash
                },
                new TruckSpending
                {
                    TruckSpendingID = Guid.NewGuid(),
                    TruckID = _truckId,
                    Truck = truck,
                    Amount = 50m,
                    PaymentDate = DateTime.UtcNow.Date.AddDays(-2),
                    SpendingType = TruckSpendingsType.Parking,
                    PaymentDescription = "Parking",
                    PaymentMethod = PaymentMethods.Card
                },
                new TruckSpending
                {
                    TruckSpendingID = Guid.NewGuid(),
                    TruckID = _otherTruckId,
                    Truck = otherTruck,
                    TruckCourseID = _otherCourseId,
                    TruckCourse = otherCourse,
                    Amount = 200m,
                    PaymentDate = DateTime.UtcNow.Date.AddDays(-3),
                    SpendingType = TruckSpendingsType.Maintenance,
                    PaymentDescription = "Other maintenance",
                    PaymentMethod = PaymentMethods.Cash
                });
            await _context.SaveChangesAsync();
        }
    }
}
