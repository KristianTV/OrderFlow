using OrderFlow.Data.Models;
using OrderFlow.Data.Repository.Contracts;
using OrderFlow.ViewModels.Truck;

namespace OrderFlow.Services.Core.Contracts
{
    public interface ITruckService : IRepository
    {
        IQueryable<Truck> GetAll();
        Task<IEnumerable<IndexTruckViewModel>> GetTrucksAsync(Guid? driverId = null, TruckQueryModel? query = null);
        Task<CreateTruckViewModel?> GetTruckForEditAsync(Guid truckID);
        Task<DetailsTruckViewModel?> GetTruckDetailsAsync(Guid truckID, Guid? driverId = null);
        Task<Dictionary<Guid, string>> GetAvailableTruckOptionsAsync();
        Task<bool> CreateTruckAsync(CreateTruckViewModel createTruckViewModel);
        Task<bool> SoftDeleteTruckAsync(Guid truckID);
        Task<bool> UpdateTruckAsync(CreateTruckViewModel createTruckViewModel, Guid truckID);
        Task ChangeTruckStatusAsync(Guid? truckID, string status);
        Task<string> GetTruckStatusAsync(Guid? truckID);
    }
}
