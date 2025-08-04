using OrderFlow.Data.Models;
using OrderFlow.Data.Repository.Contracts;
using OrderFlow.ViewModels.Truck;

namespace OrderFlow.Services.Core.Contracts
{
    public interface ITruckService : IRepository
    {
        IQueryable<Truck> GetAll();
        Task<bool> CreateTruckAsync(CreateTruckViewModel createTruckViewModel);
        Task<bool> SoftDeleteTruckAsync(Guid truckID);
        Task<bool> UpdateTruckAsync(CreateTruckViewModel createTruckViewModel, Guid truckID);
        Task ChangeTruckStatusAsync(Guid truckID, string status);
        Task<string> GetTruckStatusAsync(Guid truckID);
        void ChangeTruckStatus(Guid truckID, string status);
        string GetTruckStatus(Guid truckID);
    }
}
