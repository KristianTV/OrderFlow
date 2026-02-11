using OrderFlow.Data.Models;
using OrderFlow.Data.Repository.Contracts;

namespace OrderFlow.Services.Core.Contracts
{
    public interface ICourseOrderService : IRepository
    {
        IQueryable<CourseOrder> GetAll();
        Task<CourseOrder?> GetByIdAsync(Guid orderID, Guid courseID);
        Task<IEnumerable<CourseOrder>> GetByCourseIdAsync(Guid courseID);
        Task<bool> AddOrderToCourseAsync(Guid orderID, Guid courseID, bool save = true);
        Task<bool> RemoveOrderFromCourseAsync(Guid orderID, Guid courseID, bool save = true);

        //Task<bool> CreateOrderAsync(CreateOrderViewModel createOrderViewModel, Guid courseID, Guid? userId, bool save = true);
    }
}
