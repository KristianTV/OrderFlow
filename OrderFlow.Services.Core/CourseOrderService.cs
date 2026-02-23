using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Data.Repository;
using OrderFlow.Services.Core.Contracts;

namespace OrderFlow.Services.Core
{
    public class CourseOrderService : BaseRepository, ICourseOrderService
    {
        private readonly IOrderService _orderService;
        public CourseOrderService(OrderFlowDbContext _context, IOrderService orderService) : base(_context)
        {
            _orderService = orderService;
        }

        public async Task<bool> AddOrderToCourseAsync(Guid orderID, Guid courseID, bool save = true)
        {
            if (await this.GetAll().AsNoTracking().AnyAsync(co => co.OrderID.Equals(orderID) && co.TruckCourseID.Equals(courseID)))
                return false;

            CourseOrder courseOrder = new CourseOrder
            {
                OrderID = orderID,
                TruckCourseID = courseID
            };

            await this.AddAsync(courseOrder);

            await _orderService.ChangeOrderStatusAsync(orderID, OrderStatus.InProgress.ToString(), false);

            if (save)
            {
                return await this.SaveChangesAsync() > 0;
            }

            return true;
        }

        public IQueryable<CourseOrder> GetAll()
        {
            return All<CourseOrder>().AsQueryable();
        }

        public async Task<IEnumerable<CourseOrder>> GetByCourseIdAsync(Guid courseID)
        {
            return await this.GetAll()
                             .AsNoTracking()
                             .Where(co => co.TruckCourseID.Equals(courseID))
                             .ToListAsync();
        }

        public async Task<CourseOrder?> GetByIdAsync(Guid orderID, Guid courseID)
        {
            return await this.GetAll()
                             .AsNoTracking()
                             .FirstOrDefaultAsync(co => co.OrderID.Equals(orderID) &&
                                                        co.TruckCourseID.Equals(courseID));
        }

        public async Task<bool> RemoveOrderFromCourseAsync(Guid orderID, Guid courseID, bool save = true)
        {

            CourseOrder? courseOrder = await this.GetByIdAsync(orderID, courseID);

            if (courseOrder == null)
            {
                return false;
            }

            this.Delete<CourseOrder>(courseOrder);
            await _orderService.ChangeOrderStatusAsync(orderID, OrderStatus.Pending.ToString(), false);

            if (save)
            {
                return await this.SaveChangesAsync() > 0;
            }

            return true;
        }

    }
}
