using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Data.Repository;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.Services.Core.Extensions;
using OrderFlow.ViewModels.Course;
using OrderFlow.ViewModels.Truck;

namespace OrderFlow.Services.Core
{
    public class TruckCourseService : BaseRepository, ITruckCourseService
    {
        private readonly IOrderService _orderService;
        private readonly INotificationService _notificationService;
        private readonly ITruckService _truckService;
        private readonly ICourseOrderService _courseOrderService;
        public TruckCourseService(OrderFlowDbContext _context,
            IOrderService orderService,
            INotificationService notificationService,
            ITruckService truckService,
            ICourseOrderService _courseOrderService
            ) : base(_context)
        {
            this._orderService = orderService;
            this._notificationService = notificationService;
            this._truckService = truckService;
            this._courseOrderService = _courseOrderService;
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
                                                 .ThenInclude(t => t.Driver)
                                                 .Include(tc => tc.CourseOrders)
                                                 .ThenInclude(co => co.Order)
                                                 .SingleOrDefaultAsync();

            if (truckCourse == null)
            {
                throw new InvalidOperationException("This course does not exist.");
            }

            double capacity = truckCourse.Truck.Capacity;

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

        public async Task<bool> RemoveOrderFromCourseAsync(Guid courseID, Guid orderID)
        {
            throw new NotImplementedException();
            //    if (truckID == Guid.Empty || orderID == Guid.Empty)
            //    {
            //        throw new ArgumentException("Truck ID and Order ID cannot be empty.");
            //    }

            //    var truckOrder = await GetAll()
            //                               .Where(to => to.OrderID.Equals(orderID) &&
            //                                            to.TruckID.Equals(truckID) &&
            //                                            to.Status.Equals(CourseStatus.Assigned)).SingleOrDefaultAsync();

            //    if (truckOrder != null)
            //    {
            //        await _orderService.ChangeOrderStatusAsync(orderID, OrderStatus.Pending.ToString());

            //        await _notificationService.AddAsync(new Notification
            //        {
            //            Title = "Order Remove",
            //            Message = $"Order {orderID} have been remove from truck {truckID}.",
            //            OrderID = orderID,
            //            CreatedAt = DateTime.UtcNow,
            //            ReceiverID = GetAll()
            //                             .AsNoTracking()
            //                             .Include(to => to.Truck)
            //                             .ThenInclude(t => t.Driver)
            //                             .Where(to => to.TruckID.Equals(truckID) &&
            //                                          to.Status.Equals(CourseStatus.Assigned))
            //                             .OrderByDescending(to => to.AssignedDate)
            //                             .Select(to => to.Truck.Driver.Id)
            //                             .FirstOrDefault(),
            //            TruckID = truckID,
            //        });

            //        Delete(truckOrder);

            //        await SaveChangesAsync();

            //        if (!GetAll().Any(t => t.TruckID.Equals(truckID) && t.Status.Equals(CourseStatus.Assigned)))
            //        {
            //            if (!_truckService.GetTruckStatus(truckID).Equals(TruckStatus.Available.ToString()))
            //            {
            //                _truckService.ChangeTruckStatus(truckID, TruckStatus.Available.ToString());
            //                await SaveChangesAsync();
            //            }
            //        }

            //        return true;
            //    }
            //    return false;
        }

        public async Task CompleteCourseAsync(Guid courseID)
        {
            throw new NotImplementedException();
            //    CourseOrder? order = await GetAll()
            //                                  .Include(to => to.Truck)
            //                                  .ThenInclude(t => t.Driver)
            //                                  .Where(x => x.OrderID.Equals(orderID) &&
            //                                              x.Status.Equals(CourseStatus.Assigned))
            //                                  .SingleOrDefaultAsync();

            //    if (order != null)
            //    {
            //        if (order.Status == CourseStatus.Delivered)
            //            return;

            //        order.Status = CourseStatus.Delivered;
            //        order.DeliveryDate = DateTime.UtcNow;

            //        if (order.Truck.DriverID == Guid.Empty)
            //            return;

            //        await _notificationService.AddAsync(new Notification
            //        {
            //            Title = $"Order {orderID} has been delivered",
            //            OrderID = order.OrderID,
            //            Message = $"Order {orderID} has been delivered",
            //            CreatedAt = DateTime.UtcNow,
            //            ReceiverID = order.Truck.DriverID,
            //        });

            //        await SaveChangesAsync();
            //    }
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
    }
}
