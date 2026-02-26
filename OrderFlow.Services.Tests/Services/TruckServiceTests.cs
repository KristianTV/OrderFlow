using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services.Core;
using OrderFlow.ViewModels.Truck;
namespace OrderFlow.Tests.Services
{
    [TestFixture]
    public class TruckServiceTests
    {
        private TruckService _truckService;
        private OrderFlowDbContext _context;
        private DbContextOptions<OrderFlowDbContext> _options;

        [SetUp]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<OrderFlowDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_Database" + Guid.NewGuid())
                .Options;
            _context = new OrderFlowDbContext(_options);
            _truckService = new TruckService(_context);

            _context.Trucks.Add(new Truck { TruckID = Guid.Parse("f1a8c5e6-7b9a-4c2d-8e1f-6a7b8c9d0e1f"), LicensePlate = "ABC-123", Capacity = 100, Status = TruckStatus.Available, IsDeleted = false });
            _context.Trucks.Add(new Truck { TruckID = Guid.Parse("b1a8c5e6-7b9a-4c2d-8e1f-6a7b8c9d0e1f"), LicensePlate = "XYZ-789", Capacity = 200, Status = TruckStatus.Unavailable, IsDeleted = false });
            _context.Trucks.Add(new Truck { TruckID = Guid.Parse("c1a8c5e6-7b9a-4c2d-8e1f-6a7b8c9d0e1f"), LicensePlate = "DEF-456", Capacity = 150, Status = TruckStatus.UnderMaintenance, IsDeleted = true });
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void GetAll_ReturnsAllTrucks()
        {
            var trucks = _truckService.GetAll().IgnoreQueryFilters().ToList();
            Assert.AreEqual(3, trucks.Count);
        }

        [Test]
        public async Task CreateTruckAsync_WithValidData_ReturnsTrue()
        {
            var viewModel = new CreateTruckViewModel { LicensePlate = "GHI-321", Capacity = 50, DriverID = Guid.NewGuid() };
            var result = await _truckService.CreateTruckAsync(viewModel);
            Assert.IsTrue(result);
            Assert.AreEqual(4, _context.Trucks.IgnoreQueryFilters().Count());
        }

        [Test]
        public async Task CreateTruckAsync_WithNullViewModel_ReturnsFalse()
        {
            var result = await _truckService.CreateTruckAsync(null);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task CreateTruckAsync_WithInvalidLicensePlate_ReturnsFalse()
        {
            var viewModel = new CreateTruckViewModel { LicensePlate = " ", Capacity = 50, DriverID = Guid.NewGuid() };
            var result = await _truckService.CreateTruckAsync(viewModel);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task CreateTruckAsync_WithInvalidCapacity_ReturnsFalse()
        {
            var viewModel = new CreateTruckViewModel { LicensePlate = "GHI-321", Capacity = 0, DriverID = Guid.NewGuid() };
            var result = await _truckService.CreateTruckAsync(viewModel);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task SoftDeleteTruckAsync_ExistingTruck_ReturnsTrue()
        {
            var truckId = Guid.Parse("f1a8c5e6-7b9a-4c2d-8e1f-6a7b8c9d0e1f");
            var result = await _truckService.SoftDeleteTruckAsync(truckId);
            Assert.IsTrue(result);
            Assert.IsTrue(_context.Trucks.IgnoreQueryFilters().First(t => t.TruckID == truckId).IsDeleted);
        }

        [Test]
        public async Task SoftDeleteTruckAsync_AlreadyDeletedTruck_ReturnsFalse()
        {
            var truckId = Guid.Parse("c1a8c5e6-7b9a-4c2d-8e1f-6a7b8c9d0e1f");
            var result = await _truckService.SoftDeleteTruckAsync(truckId);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task SoftDeleteTruckAsync_NonExistingTruck_ReturnsFalse()
        {
            var truckId = Guid.NewGuid();
            var result = await _truckService.SoftDeleteTruckAsync(truckId);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task UpdateTruckAsync_ExistingTruck_ReturnsTrue()
        {
            var truckId = Guid.Parse("f1a8c5e6-7b9a-4c2d-8e1f-6a7b8c9d0e1f");
            var viewModel = new CreateTruckViewModel { LicensePlate = "NEW-111", Capacity = 999, DriverID = Guid.NewGuid() };
            var result = await _truckService.UpdateTruckAsync(viewModel, truckId);
            Assert.IsTrue(result);
            var updatedTruck = _context.Trucks.First(t => t.TruckID == truckId);
            Assert.AreEqual("NEW-111", updatedTruck.LicensePlate);
            Assert.AreEqual(999, updatedTruck.Capacity);
        }

        [Test]
        public async Task UpdateTruckAsync_NonExistingTruck_ReturnsFalse()
        {
            var viewModel = new CreateTruckViewModel { LicensePlate = "NEW-111", Capacity = 999 };
            var result = await _truckService.UpdateTruckAsync(viewModel, Guid.NewGuid());
            Assert.IsFalse(result);
        }

        [Test]
        public async Task UpdateTruckAsync_WithNullViewModel_ReturnsFalse()
        {
            var truckId = Guid.Parse("f1a8c5e6-7b9a-4c2d-8e1f-6a7b8c9d0e1f");
            var result = await _truckService.UpdateTruckAsync(null, truckId);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task ChangeTruckStatusAsync_ValidStatus_StatusIsUpdated()
        {
            var truckId = Guid.Parse("f1a8c5e6-7b9a-4c2d-8e1f-6a7b8c9d0e1f");
            await _truckService.ChangeTruckStatusAsync(truckId, "Unavailable");
            var truck = await _context.Trucks.FindAsync(truckId);
            Assert.AreEqual(TruckStatus.Unavailable, truck.Status);
        }

        [Test]
        public async Task ChangeTruckStatusAsync_SameStatus_NoChange()
        {
            var truckId = Guid.Parse("f1a8c5e6-7b9a-4c2d-8e1f-6a7b8c9d0e1f");
            await _truckService.ChangeTruckStatusAsync(truckId, "Available");
            var truck = await _context.Trucks.FindAsync(truckId);
            Assert.AreEqual(TruckStatus.Available, truck.Status);
        }

        [Test]
        public void ChangeTruckStatusAsync_InvalidStatus_ThrowsException()
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await _truckService.ChangeTruckStatusAsync(Guid.NewGuid(), "InvalidStatus"));
        }

        [Test]
        public void ChangeTruckStatusAsync_NullId_ThrowsException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _truckService.ChangeTruckStatusAsync(Guid.Empty, "Available"));
        }

        [Test]
        public async Task GetTruckStatusAsync_ExistingTruck_ReturnsStatus()
        {
            var truckId = Guid.Parse("f1a8c5e6-7b9a-4c2d-8e1f-6a7b8c9d0e1f");
            var status = await _truckService.GetTruckStatusAsync(truckId);
            Assert.AreEqual(TruckStatus.Available.ToString(), status);
        }

        [Test]
        public async Task GetTruckStatusAsync_NonExistingTruck_ReturnsEmptyString()
        {
            var status = await _truckService.GetTruckStatusAsync(Guid.NewGuid());
            Assert.AreEqual(string.Empty, status);
        }
    }
}