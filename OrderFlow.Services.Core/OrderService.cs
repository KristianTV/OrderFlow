using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Data.Repository;
using OrderFlow.Services.Contracts;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.Services.Core.Extensions;
using OrderFlow.ViewModels.Order;
using Order = OrderFlow.Data.Models.Order;

namespace OrderFlow.Services.Core
{
    public class OrderService : BaseRepository, IOrderService
    {
        private readonly INotificationService _notificationService;

        public OrderService(OrderFlowDbContext _context, IMailService mailService, INotificationService notificationService) : base(_context)
        {
            _notificationService = notificationService;
        }

        public async Task<bool> CancelOrderAsync(Guid? orderId, Guid? userId, bool save = true)
        {
            if (orderId == null || userId == null || orderId == Guid.Empty || userId == Guid.Empty)
                return false;

            Order? order = await this.GetAll()
                                     .Include(o => o.CourseOrders)
                                     .Where(x => x.OrderID.Equals(orderId) &&
                                                 x.UserID.Equals(userId))
                                     .SingleOrDefaultAsync();

            if (order != null)
            {
                if (order.IsCanceled)
                    return true;

                order.IsCanceled = true;
                order.Status = OrderStatus.Cancelled;

                await _notificationService.SendSystemNotificationAsync(order.ToNotification($"Order {orderId} has been Cancelled",
                                                                                            $"Order {orderId} has been Cancelled"),
                                                                                            false);

                await this.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> ChangeOrderStatusAsync(Guid? orderId, string? status, bool save = true)
        {
            if (orderId == null || string.IsNullOrEmpty(status) || orderId == Guid.Empty)
                return false;

            if (!Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
            {
                return false;
            }

            Order? order = await this.GetTrackingOrderByIdAsync(orderId);

            if (order != null)
            {
                if (order.Status.Equals(orderStatus))
                    return true;

                order.Status = orderStatus;

                await _notificationService.SendSystemNotificationAsync(order.ToNotification($"Order status changed to {orderStatus}",
                                                                                            $"Order status changed to {orderStatus}"),
                                                                                           false);

                if (save)
                {
                    await this.SaveChangesAsync();
                }

                return true;
            }

            return false;
        }

        public async Task<bool> ChangeStatusToCompletedAsync(Guid? orderID, bool save = true)
        {
            if (orderID == null || orderID == Guid.Empty)

                return false;

            Order? order = await this.GetTrackingOrderByIdAsync(orderID);

            if (order != null)
            {
                if (order.Status == OrderStatus.Completed)
                    return true;

                order.Status = OrderStatus.Completed;
                order.DeliveryDate = DateTime.UtcNow;

                await _notificationService.SendSystemNotificationAsync(order.ToNotification($"Order {orderID} has beed Completed",
                                                                                            $"Order status changed to {OrderStatus.Completed}"),
                                                                                           false);
                if (save)
                {
                    await this.SaveChangesAsync();
                }

                return true;
            }

            return false;
        }

        public IQueryable<Order> GetAll()
        {
            return this.All<Order>().AsQueryable();
        }

        public async Task<IEnumerable<IndexOrderViewModel>> GetUserOrdersAsync(Guid userId, OrderQueryModel query)
        {
            IQueryable<Order> orders = ApplyOrderQuery(this.GetAll()
                                                           .AsNoTracking()
                                                           .Where(o => o.UserID.Equals(userId)),
                                                            query);

            return await ProjectToIndexOrderViewModel(orders).ToListAsync();
        }

        public async Task<IEnumerable<IndexOrderViewModel>> GetAdminOrdersAsync(OrderQueryModel query)
        {
            IQueryable<Order> orders = ApplyOrderQuery(this.GetAll().AsNoTracking(), query);

            return await ProjectToIndexOrderViewModel(orders).ToListAsync();
        }

        public async Task<CreateOrderViewModel?> GetOrderForEditAsync(Guid orderId, Guid userId)
        {
            return await this.GetAll()
                             .AsNoTracking()
                             .Where(o => o.OrderID.Equals(orderId) && o.UserID.Equals(userId))
                             .Select(o => new CreateOrderViewModel
                             {
                                 DeliveryAddress = o.DeliveryAddress,
                                 PickupAddress = o.PickupAddress,
                                 DeliveryInstructions = o.DeliveryInstructions,
                                 LoadCapacity = o.LoadCapacity,
                             })
                             .SingleOrDefaultAsync();
        }

        public async Task<AdminCreateOrderViewModel?> GetAdminOrderForEditAsync(Guid orderId)
        {
            return await this.GetAll()
                             .AsNoTracking()
                             .Where(o => o.OrderID.Equals(orderId))
                             .Select(o => new AdminCreateOrderViewModel
                             {
                                 UsersId = o.UserID,
                                 DeliveryAddress = o.DeliveryAddress,
                                 PickupAddress = o.PickupAddress,
                                 LoadCapacity = o.LoadCapacity,
                                 DeliveryInstructions = o.DeliveryInstructions
                             })
                             .SingleOrDefaultAsync();
        }

        public async Task<DetailsOrderViewModel?> GetOrderDetailsAsync(Guid orderId, Guid? userId = null)
        {
            IQueryable<Order> orders = this.GetAll()
                                           .AsNoTracking()
                                           .Include(o => o.User)
                                           .Include(o => o.Payments)
                                           .Include(o => o.CourseOrders)
                                           .ThenInclude(co => co.TruckCourse)
                                           .ThenInclude(tc => tc.Truck)
                                           .Where(o => o.OrderID.Equals(orderId));

            if (userId.HasValue)
            {
                orders = orders.Where(o => o.UserID.Equals(userId.Value));
            }

            return await orders.Select(o => new DetailsOrderViewModel
            {
                OrderID = o.OrderID,
                UserName = o.User!.UserName!,
                OrderDate = o.OrderDate,
                DeliveryDate = o.DeliveryDate,
                DeliveryAddress = o.DeliveryAddress,
                PickupAddress = o.PickupAddress,
                LoadCapacity = o.LoadCapacity,
                DeliveryInstructions = o.DeliveryInstructions,
                Status = o.Status.ToString(),
                isCanceled = o.IsCanceled,
                TrucksLicensePlates = o.CourseOrders.Select(co => co.TruckCourse.Truck!.LicensePlate).ToList(),
                Payments = o.Payments.Select(payment => new PaymentViewModel
                {
                    Id = payment.PaymentID,
                    CreatedOn = payment.CreatedOn,
                    Amount = payment.Amount,
                    PaymentDescription = payment.PaymentDescription,
                    PaymentMethod = payment.PaymentMethod.HasValue ? payment.PaymentMethod.Value.ToString() : null,
                    PaymentDate = payment.PaymentDate,
                }).OrderBy(p => p.CreatedOn).ToList(),
                TotalPrice = o.Payments.Sum(p => p.Amount)
            })
                               .SingleOrDefaultAsync();
        }
        public async Task<InvoiceViewModel?> GetOrderInvoiceDetailsAsync(Guid orderId, Guid? userId = null)
        {
            IQueryable<Order> orders = this.GetAll()
                                           .AsNoTracking()
                                           .Include(o => o.User)
                                           .Include(o => o.Payments)
                                           .Include(o => o.CourseOrders)
                                           .ThenInclude(co => co.TruckCourse)
                                           .ThenInclude(tc => tc.Truck)
                                           .Where(o => o.OrderID.Equals(orderId));

            if (userId.HasValue)
            {
                orders = orders.Where(o => o.UserID.Equals(userId.Value));
            }



            return await orders.Select(o => new InvoiceViewModel
            {
                InvoiceId = "OF" + o.OrderID.ToString(),
                Issued = o.DeliveryDate,
                Order = new InvoiceOrderViewModel
                {
                    OrderID = o.OrderID,
                    DeliveryAddress = o.DeliveryAddress,
                    PickupAddress = o.PickupAddress,
                    LoadCapacity = o.LoadCapacity,
                    OrderDate = o.OrderDate,
                    DeliveryDate = o.DeliveryDate
                },
                UserName = o.User!.UserName!,
                Payments = o.Payments.Select(payment => new PaymentViewModel
                {
                    Id = payment.PaymentID,
                    CreatedOn = payment.CreatedOn,
                    Amount = payment.Amount,
                    PaymentDescription = payment.PaymentDescription,
                    PaymentMethod = payment.PaymentMethod.HasValue ? payment.PaymentMethod.Value.ToString() : null,
                    PaymentDate = payment.PaymentDate,
                }).ToList(),
                TotalPrice = o.Payments.Sum(p => p.Amount)
            })
                               .SingleOrDefaultAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(Guid? orderId)
        {
            return await this.GetAll()
                             .AsNoTracking()
                             .Where(x => x.OrderID.Equals(orderId))
                             .SingleOrDefaultAsync();
        }

        private static IQueryable<Order> ApplyOrderQuery(IQueryable<Order> orders, OrderQueryModel? query)
        {
            query ??= new OrderQueryModel();

            if (!string.IsNullOrWhiteSpace(query.SearchId))
            {
                orders = orders.Where(o => o.OrderID.ToString().Contains(query.SearchId));
            }

            if (!string.IsNullOrWhiteSpace(query.StatusFilter))
            {
                if (!Enum.TryParse(query.StatusFilter, true, out OrderStatus orderStatus))
                {
                    throw new ArgumentException("Invalid order status.", nameof(query.StatusFilter));
                }

                orders = orders.Where(o => o.Status.Equals(orderStatus));
            }

            if (query.HideCompleted)
            {
                orders = orders.Where(o => o.Status != OrderStatus.Completed);
            }

            return (query.SortOrder switch
            {
                "date_desc" => orders.OrderByDescending(o => o.OrderDate),
                "date_asc" => orders.OrderBy(o => o.OrderDate),
                _ => orders.OrderByDescending(o => o.OrderDate)
            })
            .Skip((Math.Max(query.Page, 1) - 1) * Math.Max(query.PageSize - 1, 1))
            .Take(Math.Max(query.PageSize, 1));
        }

        private static IQueryable<IndexOrderViewModel> ProjectToIndexOrderViewModel(IQueryable<Order> orders)
        {
            return orders.Select(order => new IndexOrderViewModel
            {
                OrderID = order.OrderID,
                OrderDate = order.OrderDate,
                DeliveryAddress = order.DeliveryAddress,
                PickupAddress = order.PickupAddress,
                Status = order.Status.ToString(),
                isCanceled = order.IsCanceled
            });
        }

        private async Task<Order?> GetTrackingOrderByIdAsync(Guid? orderId)
        {
            return await this.GetAll()
                             .Where(x => x.OrderID.Equals(orderId))
                             .SingleOrDefaultAsync();
        }
        private async Task<Order?> GetTrackingOrderByIdAndUserIdAsync(Guid? orderId, Guid? userId)
        {
            return await this.GetAll()
                             .Where(x => x.OrderID.Equals(orderId) &&
                                         x.UserID.Equals(userId))
                             .SingleOrDefaultAsync();
        }
        public async Task<IEnumerable<Order>> GetAllByUserIdAsync(Guid? userId)
        {
            return await this.GetAll()
                             .AsNoTracking()
                             .Where(x => x.UserID.Equals(userId))
                             .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetAllByUserIdAndStatusAsync(Guid? userId, OrderStatus status)
        {
            return await this.GetAll()
                             .AsNoTracking()
                             .Where(x => x.UserID.Equals(userId) && x.Status.Equals(status))
                             .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetAllByStatusAsync(OrderStatus status)
        {
            return await this.GetAll()
                             .AsNoTracking()
                             .Where(x => x.Status.Equals(status))
                             .ToListAsync();
        }

        public async Task<bool> ReactivateOrderAsync(Guid orderId, bool save = true)
        {
            if (orderId == Guid.Empty)
                return false;

            Order? order = await this.GetTrackingOrderByIdAsync(orderId);

            if (order != null)
            {
                if (!order.IsCanceled)
                    return true;

                order.IsCanceled = false;
                order.Status = OrderStatus.Pending;

                await _notificationService.SendSystemNotificationAsync(order.ToNotification($"Order {orderId} has been Reactivated",
                                                                                            $"Order {orderId} has been Reactivated"),
                                                                                            false);
                if (save)
                {
                    await this.SaveChangesAsync();
                }
                return true;
            }

            return false;
        }

        public async Task<bool> CreateOrderAsync(CreateOrderViewModel createOrderViewModel, Guid? userId, bool save = true)
        {
            if (createOrderViewModel == null || !userId.HasValue)
                return false;

            Order newOrder = new Order
            {
                UserID = (Guid)userId,
                OrderDate = DateTime.UtcNow,
                DeliveryAddress = createOrderViewModel.DeliveryAddress,
                PickupAddress = createOrderViewModel.PickupAddress,
                DeliveryInstructions = createOrderViewModel.DeliveryInstructions,
                Status = OrderStatus.Pending,
                LoadCapacity = createOrderViewModel.LoadCapacity,
            };

            await this.AddAsync(newOrder);

            if (save)
            {
                int changes = await this.SaveChangesAsync();
                return changes > 0;
            }

            return true;
        }

        public async Task<bool> UpdateOrderAsync(CreateOrderViewModel createOrder, Guid? orderId, Guid? userId, bool save = true)
        {
            if (orderId == null || userId == null || createOrder == null || orderId == Guid.Empty || userId == Guid.Empty)
                return false;

            Order? order = await this.GetTrackingOrderByIdAndUserIdAsync(orderId, userId);

            if (order != null)
            {
                if (order.Status.Equals(OrderStatus.Completed))
                    return false;

                order.DeliveryAddress = createOrder.DeliveryAddress;
                order.PickupAddress = createOrder.PickupAddress;
                order.DeliveryInstructions = createOrder.DeliveryInstructions;
                order.LoadCapacity = createOrder.LoadCapacity;

                if (save)
                {
                    int changes = await this.SaveChangesAsync();
                    return changes > 0;
                }

                return true;
            }
            return false;
        }

        public async Task CompleteOrderAsync(Guid orderID, Guid courseID, bool save = true)
        {
            if (orderID == Guid.Empty)
                throw new ArgumentException("Order ID cannot be empty.", nameof(orderID));

            if (courseID == Guid.Empty)
                throw new ArgumentException("Course ID cannot be empty.", nameof(courseID));

            Order? order = await this.GetAll()
                                     .AsNoTracking()
                                     .Include(o => o.CourseOrders)
                                     .ThenInclude(co => co.TruckCourse)
                                     .Where(x => x.OrderID.Equals(orderID))
                                     .SingleOrDefaultAsync();

            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {orderID} was not found.");
            }

            await ProcessOrderCompletionAsync(order, courseID);

            if (save)
            {
                await this.SaveChangesAsync();
            }

        }

        public async Task CompleteMultipleOrdersAsync(IEnumerable<Guid> orderIDs, Guid courseID, bool save = true)
        {
            if (orderIDs == null || !orderIDs.Any())
                return;

            if (courseID == Guid.Empty)
                throw new ArgumentException("Course ID cannot be empty.", nameof(courseID));

            List<Order> orders = await this.GetAll()
                                           .AsNoTracking()
                                           .Include(o => o.CourseOrders)
                                           .ThenInclude(co => co.TruckCourse)
                                           .Where(o => orderIDs.Contains(o.OrderID))
                                           .ToListAsync();

            foreach (var order in orders)
            {
                if (order == null || order.Equals(Guid.Empty))
                {
                    continue;
                }
                await ProcessOrderCompletionAsync(order, courseID);
            }

            if (save)
            {
                await this.SaveChangesAsync();
            }
        }

        private async Task ProcessOrderCompletionAsync(Order order, Guid courseID)
        {

            if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Failed)
            {
                return;
            }

            var currentCourse = order.CourseOrders?
                                     .Select(co => co.TruckCourse)
                                     .FirstOrDefault(tc => tc != null && tc.TruckCourseID == courseID);

            if (currentCourse == null)
            {
                throw new InvalidOperationException($"The course {courseID} is not associated with order {order.OrderID}.");
            }

            if (order.DeliveryAddress != null && order.DeliveryAddress.Equals(currentCourse.DeliverAddress))
            {
                await this.ChangeStatusToCompletedAsync(order.OrderID, false);
            }
            else
            {
                await this.ChangeOrderStatusAsync(order.OrderID, OrderStatus.OnHold.ToString(), false);
            }
        }

    }
}
