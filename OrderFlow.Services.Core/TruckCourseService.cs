using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Data.Repository;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.Services.Core.Extensions;
using OrderFlow.ViewModels.Course;
using OrderFlow.ViewModels.CourseOrder;
using OrderFlow.ViewModels.Order;

namespace OrderFlow.Services.Core
{
    public class TruckCourseService : BaseRepository, ITruckCourseService
    {
        private readonly IOrderService _orderService;
        private readonly INotificationService _notificationService;
        private readonly ITruckService _truckService;
        private readonly ICourseOrderService _courseOrderService;
        private readonly IPaymentService _paymentService;
        public TruckCourseService(OrderFlowDbContext _context,
            IOrderService orderService,
            INotificationService notificationService,
            ITruckService truckService,
            ICourseOrderService _courseOrderService,
            IPaymentService _paymentService
            ) : base(_context)
        {
            this._orderService = orderService;
            this._notificationService = notificationService;
            this._truckService = truckService;
            this._courseOrderService = _courseOrderService;
            this._paymentService = _paymentService;
        }

        public IQueryable<TruckCourse> GetAll()
        {
            return this.All<TruckCourse>().AsQueryable();
        }

        public async Task<TruckCourse?> GetByIdAsync(Guid courseID)
        {
            return await this.GetAll()
                             .AsNoTracking()
                             .SingleOrDefaultAsync(tc => tc.TruckCourseID.Equals(courseID));
        }

        public IQueryable<TruckCourse> GetQueryableByIdAsync(Guid courseID)
        {
            return this.GetAll()
                       .AsNoTracking()
                       .Where(tc => tc.TruckCourseID.Equals(courseID));
        }

        public async Task<IEnumerable<IndexCourseViewModel>> GetCoursesAsync(Guid? driverId, CourseQueryModel query)
        {
            IQueryable<TruckCourse> courses = this.GetAll().AsNoTracking();

            if (driverId.HasValue)
            {
                courses = courses.Include(tc => tc.Truck)
                                 .Where(tc => tc.Truck != null &&
                                              tc.Truck.DriverID.Equals(driverId.Value));
            }

            courses = ApplyCourseQuery(courses, query);

            return await courses.Select(tc => new IndexCourseViewModel
            {
                TruckCourseID = tc.TruckCourseID,
                TruckID = tc.TruckID,
                PickupAddress = tc.PickupAddress,
                DeliverAddress = tc.DeliverAddress,
                AssignedDate = tc.AssignedDate,
                DeliveryDate = tc.DeliveryDate,
                Status = tc.Status,
                Income = tc.Income
            })
                               .ToListAsync();
        }

        public async Task<CreateCourseViewModel?> GetCourseForEditAsync(Guid courseId)
        {
            return await this.GetAll()
                             .AsNoTracking()
                             .Where(tc => tc.TruckCourseID.Equals(courseId))
                             .Select(tc => new CreateCourseViewModel
                             {
                                 SelectedTruckID = tc.TruckID,
                                 PickupAddress = tc.PickupAddress,
                                 DeliverAddress = tc.DeliverAddress,
                                 Income = tc.Income
                             })
                             .SingleOrDefaultAsync();
        }

        public async Task<DetailsCourseViewModel?> GetCourseDetailsAsync(Guid courseId, Guid? driverId = null)
        {
            IQueryable<TruckCourse> courses = this.GetAll()
                                                  .AsNoTracking()
                                                  .Include(tc => tc.CourseOrders)
                                                  .ThenInclude(co => co.Order)
                                                  .Include(tc => tc.Truck)
                                                  .Where(tc => tc.TruckCourseID.Equals(courseId));

            if (driverId.HasValue)
            {
                courses = courses.Where(tc => tc.Truck != null &&
                                              tc.Truck.DriverID.Equals(driverId.Value));
            }

            return await courses.Select(tc => new DetailsCourseViewModel
            {
                TruckCourseID = tc.TruckCourseID,
                TruckID = tc.TruckID,
                TruckPlates = tc.Truck!.LicensePlate ?? string.Empty,
                PickupAddress = tc.PickupAddress,
                DeliverAddress = tc.DeliverAddress,
                AssignedDate = tc.AssignedDate,
                DeliveryDate = tc.DeliveryDate,
                Status = tc.Status,
                Income = tc.Income,
                AssinedOrders = tc.CourseOrders.Select(co => new IndexOrderViewModel
                {
                    OrderID = co.OrderID,
                    OrderDate = co.Order.OrderDate,
                    DeliveryAddress = co.Order.DeliveryAddress,
                    PickupAddress = co.Order.PickupAddress,
                    Status = co.Order.Status.ToString(),
                    isCanceled = co.Order.IsCanceled,
                    LoadCapacity = co.Order.LoadCapacity
                }).ToList(),
            })
                               .SingleOrDefaultAsync();
        }

        public async Task<int> AssignOrdersToCourseAsync(IEnumerable<OrderViewModel> ordersToAssign, Guid courseID)
        {

            if (ordersToAssign == null || !ordersToAssign.Any())
            {
                throw new ArgumentException("No orders to assign.");
            }

            List<Guid> orderIds = ordersToAssign.Select(o => o.OrderID).ToList();

            List<Guid> existingOrderIds = await _orderService.GetAll()
                                                             .AsNoTracking()
                                                             .Where(o => orderIds.Contains(o.OrderID))
                                                             .Select(o => o.OrderID)
                                                             .ToListAsync();

            List<Guid> missingOrderIds = orderIds.Except(existingOrderIds).ToList();

            if (missingOrderIds.Any())
            {
                throw new InvalidOperationException($"Orders not found: {string.Join(", ", missingOrderIds)}");
            }


            TruckCourse? truckCourse = await this.GetQueryableByIdAsync(courseID)
                                                 .Include(tc => tc.Truck)
                                                 .ThenInclude(t => t!.Driver)
                                                 .Include(tc => tc.CourseOrders)
                                                 .ThenInclude(co => co.Order)
                                                 .SingleOrDefaultAsync();

            if (truckCourse == null)
            {
                throw new InvalidOperationException("This course does not exist.");
            }

            if (truckCourse.Truck == null)
            {
                throw new InvalidOperationException("This course does not have a truck assigned.");
            }

            double capacity = truckCourse!.Truck!.Capacity;

            double loadedCapacity = truckCourse.CourseOrders.Sum(co => co.Order.LoadCapacity);

            capacity -= loadedCapacity;

            if (capacity < 0)
            {
                throw new InvalidOperationException("Truck capacity is full.");
            }

            foreach (var order in ordersToAssign)
            {
                capacity -= order.LoadCapacity;

                if (capacity < 0)
                {
                    throw new InvalidOperationException("Truck capacity is full.");
                }

                bool isAdded = await _courseOrderService.AddOrderToCourseAsync(order.OrderID, courseID, false);

                if (!isAdded)
                {
                    throw new InvalidOperationException($"Order with ID {order.OrderID} could not be assigned to the course.");
                }

                await _orderService.ChangeOrderStatusAsync(order.OrderID, OrderStatus.InProgress.ToString(), false);
            }

            if (existingOrderIds.Any())
            {
                await _notificationService.SendSystemNotificationAsync(truckCourse.ToNotification("Orders Assigned",
                                                                                                  "Orders have been assigned to your course."),
                                                                                                  false);
            }

            return await SaveChangesAsync();
        }

        public async Task<bool> CompleteCourseAsync(Guid courseID, bool save = true)
        {
            TruckCourse? course = await this.GetAll()
                                            .Include(tc => tc.Truck)
                                            .Include(tc => tc.CourseOrders)
                                            .Where(tc => tc.TruckCourseID.Equals(courseID))
                                            .SingleOrDefaultAsync();

            if (course == null)
                return false;

            if (course.Status == CourseStatus.Delivered)
                return false;

            if (course.Truck == null || course.Truck?.DriverID == Guid.Empty)
                return false;

            if (course.CourseOrders == null || !course.CourseOrders.Any())
                return false;

            course.Status = CourseStatus.Delivered;
            course.DeliveryDate = DateTime.UtcNow;

            await _orderService.CompleteMultipleOrdersAsync(course.CourseOrders.Select(co => co.OrderID), course.TruckCourseID, false);

            await _paymentService.CreateCoursePayoutAsync(course.CourseOrders.Select(co => co.OrderID), course, false);

            await _notificationService.SendSystemNotificationAsync(course.ToNotification($"Course {courseID} has been completed!", $"Course {courseID} has been completed!"), false);

            if (save)
            {
                return await SaveChangesAsync() > 0;
            }

            return true;
        }

        public async Task<bool> CreateCourseAsync(CreateCourseViewModel createCourseViewModel, bool save = true)
        {
            if (createCourseViewModel == null)
            {
                throw new NullReferenceException("Create Course ViewModel can\'t be null.");
            }

            TruckCourse truckCourse = new TruckCourse
            {
                TruckID = createCourseViewModel.SelectedTruckID,
                PickupAddress = createCourseViewModel.PickupAddress,
                DeliverAddress = createCourseViewModel.DeliverAddress,
                Income = createCourseViewModel.Income ?? 0,
                AssignedDate = DateTime.UtcNow,
                Status = (createCourseViewModel.SelectedTruckID != null) ? CourseStatus.Assigned : CourseStatus.Pending,
            };

            await this.AddAsync(truckCourse);

            if (save)
            {
                int changes = await this.SaveChangesAsync();
                return changes > 0;
            }

            return true;
        }

        public async Task<bool> UpdateCourseAsync(CreateCourseViewModel createCourseViewModel, Guid courseId, bool save = true)
        {
            if (createCourseViewModel == null || courseId == Guid.Empty)
                return false;

            TruckCourse? course = await this.GetAll()
                                            .Where(tc => tc.TruckCourseID.Equals(courseId))
                                            .SingleOrDefaultAsync();

            if (course == null)
                return false;

            course.TruckID = createCourseViewModel.SelectedTruckID;
            course.PickupAddress = createCourseViewModel.PickupAddress;
            course.DeliverAddress = createCourseViewModel.DeliverAddress;
            course.Income = createCourseViewModel.Income ?? 0;
            course.Status = (createCourseViewModel.SelectedTruckID != null) ? CourseStatus.Assigned : CourseStatus.Pending;

            if (save)
            {
                return await this.SaveChangesAsync() > 0;
            }

            return true;
        }

        public async Task<bool> DeleteCourseAsync(Guid courseID)
        {
            if (courseID == Guid.Empty)
            {
                throw new ArgumentException("Course ID cannot be empty.", nameof(courseID));
            }

            var course = this.GetAll()
                             .SingleOrDefault(tc => tc.TruckCourseID == courseID);

            if (course == null)
            {
                throw new KeyNotFoundException($"Course with ID {courseID} not found.");
            }

            this.Delete(course);

            return await this.SaveChangesAsync() > 0;
        }

        private static IQueryable<TruckCourse> ApplyCourseQuery(IQueryable<TruckCourse> courses, CourseQueryModel? query)
        {
            query ??= new CourseQueryModel();

            if (!string.IsNullOrWhiteSpace(query.SearchId))
            {
                string normalizedSearchId = NormalizeDisplayIdSearch(query.SearchId, "CRS");

                courses = courses.Where(tc => tc.TruckCourseID.ToString().Contains(normalizedSearchId));
            }

            if (!string.IsNullOrWhiteSpace(query.StatusFilter))
            {
                if (!Enum.TryParse(query.StatusFilter, true, out CourseStatus courseStatus))
                {
                    throw new ArgumentException("Invalid course status.", nameof(query.StatusFilter));
                }

                courses = courses.Where(tc => tc.Status.Equals(courseStatus));
            }

            if (query.HideCompleted)
            {
                courses = courses.Where(tc => tc.Status != CourseStatus.Delivered);
            }

            return (query.SortOrder switch
            {
                "date_desc" => courses.OrderByDescending(tc => tc.AssignedDate),
                "date_asc" => courses.OrderBy(tc => tc.AssignedDate),
                _ => courses.OrderByDescending(tc => tc.AssignedDate)
            })
            .Skip((Math.Max(query.Page, 1) - 1) * Math.Max(query.PageSize - 1, 1))
            .Take(Math.Max(query.PageSize, 1));
        }

        private static string NormalizeDisplayIdSearch(string searchId, string prefix)
        {
            string normalizedSearchId = searchId.Trim();
            string prefixedSearchId = $"{prefix}-";

            if (normalizedSearchId.StartsWith(prefixedSearchId, StringComparison.OrdinalIgnoreCase))
            {
                normalizedSearchId = normalizedSearchId[prefixedSearchId.Length..];
            }

            return normalizedSearchId.ToLowerInvariant();
        }
    }
}
