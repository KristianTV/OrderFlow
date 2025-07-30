using OrderFlow.Data.Repository.Contracts;
using OrderFlow.ViewModels.Truck;

namespace OrderFlow.Services.Core.Contracts
{
    public interface ITruckOrderService : IRepository
    {
        Task<int> AssignOrdersToTruckAsync(IEnumerable<OrderViewModel> assignOrders, Guid truckID);
        Task RemoveOrderFromTruckAsync(Guid truckID, Guid orderID);
    }
}
