using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Data.Repository;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Truck;

namespace OrderFlow.Services.Core
{
    public class TruckService : BaseRepository, ITruckService
    {
        public TruckService(OrderFlowDbContext _context) : base(_context)
        {
        }

        public async Task<bool> CreateTruckAsync(CreateTruckViewModel createTruckViewModel)
        {
            if (createTruckViewModel == null || string.IsNullOrWhiteSpace(createTruckViewModel.LicensePlate) || createTruckViewModel.Capacity <= 0)
                return false;

            await this.AddAsync(new Truck
            {
                LicensePlate = createTruckViewModel.LicensePlate,
                Capacity = createTruckViewModel.Capacity,
                Status = "Available",
                DriverID = createTruckViewModel.DriverID
            });

            return await this.SaveChangesAsync() > 0;
        }

        public async Task<bool> SoftDeleteTruckAsync(Guid truckID)
        {
            Truck? truck = await this.DbSet<Truck>().Where(t => t.TruckID.Equals(truckID)).SingleOrDefaultAsync();

            if (truck != null)
            {
                if (truck.isDeleted)
                {
                    return true;
                }

                truck.isDeleted = true;

                return await this.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<bool> UpdateTruckAsync(CreateTruckViewModel createTruckViewModel, Guid truckID)
        {
            if (createTruckViewModel == null || truckID == Guid.Empty)
                return false;

            Truck? truck = await this.DbSet<Truck>().Where(t => t.TruckID.Equals(truckID))
                                                   .SingleOrDefaultAsync();

            if (truck == null)
                return false;

            truck.LicensePlate = createTruckViewModel.LicensePlate;
            truck.Capacity = createTruckViewModel.Capacity;
            truck.DriverID = createTruckViewModel.DriverID;

            return await this.SaveChangesAsync() > 0;
        }
    }
}
