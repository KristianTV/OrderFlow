using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Data.Repository;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Truck;
using System.Threading.Tasks;

namespace OrderFlow.Services.Core
{
    public class TruckOrderService : BaseRepository, ITruckOrderService
    {
        public TruckOrderService(OrderFlowDbContext _context) : base(_context)
        {
        }

        public async Task<int> AssignOrdersToTruckAsync(IEnumerable<OrderViewModel> assignOrders, Guid truckID)
        {
            //Todo kg check 
            if (assignOrders == null || assignOrders == null || !assignOrders.Any())
            {
                return 0;
            }

            foreach (var order in assignOrders)
            {
                if (this.DbSet<TruckOrder>().Any(o => o.OrderID.Equals(order.OrderID)))
                {
                    continue;
                }

               await this.AddAsync(new TruckOrder
               {
                    OrderID = order.OrderID,
                    TruckID = truckID
               });

                var orderForEdit = await this.All<Order>().Where(o => o.OrderID.Equals(order.OrderID)).SingleOrDefaultAsync();

                orderForEdit.Status = OrderStatus.InProgress;
            }

            return await this.SaveChangesAsync();
        }
    }
}
