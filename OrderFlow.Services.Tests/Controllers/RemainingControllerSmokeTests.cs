using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OrderFlow.Areas.Admin.Controllers;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Services.Core;
using OrderFlow.Services.Core.Contracts;
using AdminDashboardController = OrderFlow.Areas.Admin.Controllers.DashboardController;
using AdminNotificationController = OrderFlow.Areas.Admin.Controllers.NotificationController;
using DriverDashboardController = OrderFlow.Areas.Driver.Controllers.DashboardController;
using DriverNotificationController = OrderFlow.Areas.Driver.Controllers.NotificationController;
using HomeController = OrderFlow.Controllers.HomeController;
using UserController = OrderFlow.Controllers.UserController;

namespace OrderFlow.Tests.Controllers
{
    [TestFixture]
    public class RemainingControllerSmokeTests
    {
        private OrderFlowDbContext _context = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<OrderFlowDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new OrderFlowDbContext(options);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public void HomeError_ReturnsNotFoundView_For404()
        {
            var controller = new HomeController(
                Mock.Of<ILogger<HomeController>>(),
                CreateUserManagerMock().Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

            var result = controller.Error(404);

            var view = result as ViewResult;
            Assert.That(view, Is.Not.Null);
            Assert.That(view!.ViewName, Is.EqualTo("NotFound"));
        }

        [Test]
        public void UserRegisterGet_ReturnsViewForAnonymousUser()
        {
            var controller = new UserController(
                Mock.Of<ILogger<UserController>>(),
                CreateUserManagerMock().Object,
                CreateSignInManagerMock().Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

            var result = controller.Register();

            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task AdminDashboardIndex_ReturnsDashboardViewModel()
        {
            var notificationService = new NotificationService(_context);
            var orderService = new OrderService(_context, notificationService);
            var courseOrderService = new CourseOrderService(_context, orderService);
            var truckService = new TruckService(_context);
            var controller = new AdminDashboardController(
                Mock.Of<ILogger<AdminDashboardController>>(),
                orderService,
                courseOrderService,
                truckService,
                notificationService);

            var result = await controller.Index();

            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task DriverDashboardIndex_RedirectsWhenUserIdIsInvalid()
        {
            var notificationService = new NotificationService(_context);
            var orderService = new OrderService(_context, notificationService);
            var truckService = new TruckService(_context);
            var courseOrderService = new CourseOrderService(_context, orderService);
            var truckCourseService = new TruckCourseService(
                _context,
                orderService,
                notificationService,
                truckService,
                courseOrderService);
            var controller = new DriverDashboardController(
                Mock.Of<ILogger<DriverDashboardController>>(),
                truckCourseService,
                notificationService);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

            var result = await controller.Index();

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect!.ActionName, Is.EqualTo("Error"));
            Assert.That(redirect.ControllerName, Is.EqualTo("Home"));
        }

        [Test]
        public async Task CourseOrderGetOrdersByCourseId_ReturnsBadRequestForInvalidId()
        {
            var controller = new CourseOrderController(
                Mock.Of<ICourseOrderService>(),
                Mock.Of<ITruckCourseService>(),
                Mock.Of<IOrderService>(),
                Mock.Of<ILogger<CourseOrderController>>());

            var result = await controller.GetOrdersByCourseId("bad-id");

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void PaymentCreateGet_RedirectsForInvalidOrderId()
        {
            var controller = new PaymentController(
                Mock.Of<ILogger<PaymentController>>(),
                Mock.Of<IPaymentService>());
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            var result = controller.Create("bad-id");

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect!.ActionName, Is.EqualTo("Index"));
            Assert.That(redirect.ControllerName, Is.EqualTo("Order"));
        }

        [Test]
        public async Task RoleChangeRow_ReturnsBadRequestWhenInputIsMissing()
        {
            var controller = new RoleController(
                Mock.Of<ILogger<RoleController>>(),
                CreateUserManagerMock().Object);

            var result = await controller.ChangeRow(null, "Admin");

            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task AdminNotificationDetail_ReturnsBadRequestForInvalidNotificationId()
        {
            var controller = new AdminNotificationController(
                Mock.Of<ILogger<AdminNotificationController>>(),
                Mock.Of<INotificationService>(),
                Mock.Of<IOrderService>(),
                Mock.Of<ITruckService>(),
                CreateUserManagerMock().Object);

            var result = await controller.Detail("bad-id");

            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task DriverNotificationDetail_ReturnsBadRequestForInvalidNotificationId()
        {
            var controller = new DriverNotificationController(
                Mock.Of<ILogger<DriverNotificationController>>(),
                Mock.Of<INotificationService>());
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

            var result = await controller.Detail("bad-id");

            Assert.That(result, Is.InstanceOf<BadRequestResult>());
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

        private static Mock<SignInManager<ApplicationUser>> CreateSignInManagerMock()
        {
            var userManager = CreateUserManagerMock();
            return new Mock<SignInManager<ApplicationUser>>(
                userManager.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null!,
                null!,
                null!,
                null!);
        }
    }
}
