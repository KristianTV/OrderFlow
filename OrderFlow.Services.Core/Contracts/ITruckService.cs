using OrderFlow.Data.Repository.Contracts;
using OrderFlow.ViewModels.Truck;

namespace OrderFlow.Services.Core.Contracts
{
    public interface ITruckService : IRepository
    {
        Task<bool> CreateTruckAsync(CreateTruckViewModel createTruckViewModel);
        Task<bool> UpdateTruckAsync(CreateTruckViewModel createTruckViewModel, Guid truckID);
    }
}
