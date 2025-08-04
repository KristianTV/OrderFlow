using OrderFlow.Data.Models;
using OrderFlow.Data.Repository.Contracts;
using OrderFlow.ViewModels.Truck;

namespace OrderFlow.Services.Core.Contracts
{
    public interface ITruckOrderService : IRepository
    {
        IQueryable<TruckOrder> GetAll();
        Task<int> AssignOrdersToTruckAsync(IEnumerable<OrderViewModel> assignOrders, Guid truckID);
        Task RemoveOrderFromTruckAsync(Guid truckID, Guid orderID);
        Task CompleteTruckOrderAsync(Guid orderID);
    }
}
