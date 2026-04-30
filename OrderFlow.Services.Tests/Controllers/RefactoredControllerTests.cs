using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using OrderFlow.Data.Models;
using OrderFlow.Hubs;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Course;
using OrderFlow.ViewModels.Order;
using OrderFlow.ViewModels.Truck;
using System.Security.Claims;
using AdminCourseController = OrderFlow.Areas.Admin.Controllers.CourseController;
using AdminOrderController = OrderFlow.Areas.Admin.Controllers.OrderController;
using AdminTruckController = OrderFlow.Areas.Admin.Controllers.TruckController;
using DriverCourseController = OrderFlow.Areas.Driver.Controllers.CourseController;
using DriverTruckController = OrderFlow.Areas.Driver.Controllers.TruckController;
using UserOrderController = OrderFlow.Controllers.OrderController;

namespace OrderFlow.Tests.Controllers
{
    [TestFixture]
    public class RefactoredControllerTests
    {
        [Test]
        public async Task UserOrderIndex_CallsServiceWithCurrentUserAndReturnsView()
        {
            var userId = Guid.NewGuid();
            var orderService = new Mock<IOrderService>();
            orderService
                .Setup(s => s.GetUserOrdersAsync(userId, It.Is<OrderQueryModel>(q =>
                    q.HideCompleted &&
                    q.SearchId == "abc" &&
                    q.StatusFilter == "Pending" &&
                    q.SortOrder == "date_desc")))
                .ReturnsAsync(new List<IndexOrderViewModel>
                {
                    new IndexOrderViewModel { OrderID = Guid.NewGuid(), PickupAddress = "P", DeliveryAddress = "D" }
                });

            var controller = new UserOrderController(
                Mock.Of<ILogger<UserOrderController>>(),
                orderService.Object,
                CreateUserManagerMock().Object,
                Mock.Of<IHubContext<OrderHub>>());
            SetAuthenticatedUser(controller, userId);

            var result = await controller.Index(true, "abc", "Pending", "date_desc");

            var view = result as ViewResult;
            Assert.That(view, Is.Not.Null);
            Assert.That(view!.Model, Is.InstanceOf<IEnumerable<IndexOrderViewModel>>());
        }

        [Test]
        public async Task AdminOrderDetail_CallsServiceAndReturnsDetailView()
        {
            var orderId = Guid.NewGuid();
            var orderService = new Mock<IOrderService>();
            orderService
                .Setup(s => s.GetOrderDetailsAsync(orderId, null))
                .ReturnsAsync(new DetailsOrderViewModel { OrderID = orderId, UserName = "Customer" });

            var controller = new AdminOrderController(
                Mock.Of<ILogger<AdminOrderController>>(),
                orderService.Object,
                CreateUserManagerMock().Object);

            var result = await controller.Detail(orderId.ToString());

            var view = result as ViewResult;
            Assert.That(view, Is.Not.Null);
            Assert.That(((DetailsOrderViewModel)view!.Model!).OrderID, Is.EqualTo(orderId));
        }

        [Test]
        public async Task AdminTruckIndex_CallsServiceWithStatusAndReturnsView()
        {
            var truckService = new Mock<ITruckService>();
            truckService
                .Setup(s => s.GetTrucksAsync(null, It.Is<TruckQueryModel>(q => q.Status == "Available")))
                .ReturnsAsync(new List<IndexTruckViewModel>
                {
                    new IndexTruckViewModel { TruckID = Guid.NewGuid(), LicensePlate = "ABC-123", DriverName = "Driver" }
                });

            var controller = new AdminTruckController(
                Mock.Of<ILogger<AdminTruckController>>(),
                truckService.Object,
                CreateUserManagerMock().Object,
                Mock.Of<IOrderService>(),
                Mock.Of<ICourseOrderService>());

            var result = await controller.Index("Available");

            var view = result as ViewResult;
            Assert.That(view, Is.Not.Null);
            Assert.That(controller.ViewData["CurrentStatus"], Is.EqualTo("Available"));
        }

        [Test]
        public async Task DriverTruckDetail_RestrictsLookupToCurrentDriver()
        {
            var driverId = Guid.NewGuid();
            var truckId = Guid.NewGuid();
            var truckService = new Mock<ITruckService>();
            truckService
                .Setup(s => s.GetTruckDetailsAsync(truckId, driverId))
                .ReturnsAsync(new DetailsTruckViewModel { TruckID = truckId, DriverName = "Driver" });

            var controller = new DriverTruckController(
                Mock.Of<ILogger<DriverTruckController>>(),
                truckService.Object,
                Mock.Of<IOrderService>(),
                Mock.Of<ICourseOrderService>());
            SetAuthenticatedUser(controller, driverId);

            var result = await controller.Detail(truckId.ToString());

            var view = result as ViewResult;
            Assert.That(view, Is.Not.Null);
            Assert.That(((DetailsTruckViewModel)view!.Model!).TruckID, Is.EqualTo(truckId));
        }

        [Test]
        public async Task AdminCourseCreate_LoadsAvailableTruckOptionsFromService()
        {
            var truckId = Guid.NewGuid();
            var truckService = new Mock<ITruckService>();
            truckService
                .Setup(s => s.GetAvailableTruckOptionsAsync())
                .ReturnsAsync(new Dictionary<Guid, string> { [truckId] = "TRK-1" });

            var controller = new AdminCourseController(
                Mock.Of<ILogger<AdminCourseController>>(),
                Mock.Of<ITruckCourseService>(),
                truckService.Object);

            var result = await controller.Create();

            var view = result as ViewResult;
            Assert.That(view, Is.Not.Null);
            var model = (CreateCourseViewModel)view!.Model!;
            Assert.That(model.AvailableTruckIDs, Contains.Key(truckId));
        }

        [Test]
        public async Task DriverCourseIndex_CallsServiceWithCurrentDriverAndQuery()
        {
            var driverId = Guid.NewGuid();
            var courseService = new Mock<ITruckCourseService>();
            courseService
                .Setup(s => s.GetCoursesAsync(driverId, It.Is<CourseQueryModel>(q =>
                    q.HideCompleted &&
                    q.SearchId == "course" &&
                    q.StatusFilter == "Assigned" &&
                    q.SortOrder == "date_asc")))
                .ReturnsAsync(new List<IndexCourseViewModel>
                {
                    new IndexCourseViewModel { TruckCourseID = Guid.NewGuid(), PickupAddress = "P" }
                });

            var controller = new DriverCourseController(
                Mock.Of<ILogger<DriverCourseController>>(),
                courseService.Object,
                Mock.Of<ITruckService>());
            SetAuthenticatedUser(controller, driverId);

            var result = await controller.Index(true, "course", "Assigned", "date_asc");

            var view = result as ViewResult;
            Assert.That(view, Is.Not.Null);
            Assert.That(view!.Model, Is.InstanceOf<IEnumerable<IndexCourseViewModel>>());
        }

        private static void SetAuthenticatedUser(Controller controller, Guid userId)
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(
                        new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) },
                        "TestAuth"))
                }
            };
        }

        private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                store.Object,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!);
        }
    }
}
