using OrderFlow.Data.Models;
using OrderFlow.Data.Repository.Contracts;
using OrderFlow.ViewModels.TruckSpending;

namespace OrderFlow.Services.Core.Contracts
{
    public interface ITruckSpendingService : IRepository
    {
        IQueryable<TruckSpending> GetAll();

        Task<IEnumerable<IndexTruckSpendingViewModel>> GetTruckSpendingsAsync(Guid? driverId = null, TruckSpendingQueryModel? query = null);

        Task<DetailsTruckSpendingViewModel?> GetTruckSpendingDetailsAsync(Guid spendingId, Guid? driverId = null);

        Task<CreateTruckSpendingViewModel?> GetTruckSpendingForEditAsync(Guid spendingId, Guid? driverId = null);

        Task<bool> CreateTruckSpendingAsync(CreateTruckSpendingViewModel model, Guid? driverId = null);

        Task<bool> UpdateTruckSpendingAsync(Guid spendingId, CreateTruckSpendingViewModel model, Guid? driverId = null);

        Task<bool> DeleteTruckSpendingAsync(Guid spendingId);

        Task<Dictionary<Guid, string>> GetTruckOptionsAsync(Guid? driverId = null);

        Task<Dictionary<Guid, string>> GetCourseOptionsAsync(Guid? truckId = null, Guid? driverId = null);
    }
}
