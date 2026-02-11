using OrderFlow.Data.Models;
using OrderFlow.Data.Repository.Contracts;
using OrderFlow.ViewModels.Course;
using OrderFlow.ViewModels.Truck;

namespace OrderFlow.Services.Core.Contracts
{
    public interface ITruckCourseService : IRepository
    {
        IQueryable<TruckCourse> GetAll();
        Task<TruckCourse?> GetByIdAsync(Guid courseID);
        IQueryable<TruckCourse> GetQueryableByIdAsync(Guid courseID);
        Task<int> AssignOrdersToCourseAsync(IEnumerable<OrderViewModel> ordersToAssign, Guid courseID);
        Task<bool> RemoveOrderFromCourseAsync(Guid courseID, Guid orderID);
        Task CompleteCourseAsync(Guid courseID);
        Task<bool> CreateCourseAsync(CreateCourseViewModel createCourseViewModel, bool save = true);
    }
}
