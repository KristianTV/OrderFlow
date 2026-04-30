using OrderFlow.Data.Models;
using OrderFlow.Data.Repository.Contracts;
using OrderFlow.ViewModels.Course;
using OrderFlow.ViewModels.CourseOrder;

namespace OrderFlow.Services.Core.Contracts
{
    public interface ITruckCourseService : IRepository
    {
        IQueryable<TruckCourse> GetAll();
        Task<TruckCourse?> GetByIdAsync(Guid courseID);
        IQueryable<TruckCourse> GetQueryableByIdAsync(Guid courseID);
        Task<IEnumerable<IndexCourseViewModel>> GetCoursesAsync(Guid? driverId, CourseQueryModel query);
        Task<CreateCourseViewModel?> GetCourseForEditAsync(Guid courseId);
        Task<DetailsCourseViewModel?> GetCourseDetailsAsync(Guid courseId, Guid? driverId = null);
        Task<int> AssignOrdersToCourseAsync(IEnumerable<OrderViewModel> ordersToAssign, Guid courseID);
        Task<bool> RemoveOrderFromCourseAsync(Guid courseID, Guid orderID);
        Task<bool> CompleteCourseAsync(Guid courseID, bool save = true);
        Task<bool> CreateCourseAsync(CreateCourseViewModel createCourseViewModel, bool save = true);
        Task<bool> UpdateCourseAsync(CreateCourseViewModel createCourseViewModel, Guid courseId, bool save = true);
        Task<bool> DeleteCourseAsync(Guid courseID);
    }
}
