using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
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

        public IQueryable<Truck> GetAll()
        {
            return this.All<Truck>().AsQueryable();
        }

        public async Task<bool> CreateTruckAsync(CreateTruckViewModel createTruckViewModel)
        {
            if (createTruckViewModel == null || string.IsNullOrWhiteSpace(createTruckViewModel.LicensePlate) || createTruckViewModel.Capacity <= 0)
                return false;

            await this.AddAsync(new Truck
            {
                LicensePlate = createTruckViewModel.LicensePlate,
                Capacity = createTruckViewModel.Capacity,
                Status = TruckStatus.Available,
                DriverID = createTruckViewModel.DriverID
            });

            return await this.SaveChangesAsync() > 0;
        }

        public async Task<bool> SoftDeleteTruckAsync(Guid truckID)
        {
            Truck? truck = await this.GetAll().Where(t => t.TruckID.Equals(truckID)).SingleOrDefaultAsync();

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

            Truck? truck = await this.GetAll().Where(t => t.TruckID.Equals(truckID))
                                                   .SingleOrDefaultAsync();

            if (truck == null)
                return false;

            truck.LicensePlate = createTruckViewModel.LicensePlate;
            truck.Capacity = createTruckViewModel.Capacity;
            truck.DriverID = createTruckViewModel.DriverID;

            return await this.SaveChangesAsync() > 0;
        }

        public async Task ChangeTruckStatusAsync(Guid truckID, string status)
        {
            if (truckID == null || string.IsNullOrEmpty(status) || truckID == Guid.Empty)
                throw new ArgumentNullException(nameof(truckID), "Truck ID cannot be null or empty.");

            if (!Enum.TryParse<TruckStatus>(status, true, out var result))
            {
                throw new ArgumentException("Invalid truck status.", nameof(status));
            }

            Truck? truck = await this.GetAll()
                                     .Where(t => t.TruckID.Equals(truckID))
                                     .SingleOrDefaultAsync();

            if (truck != null)
            {
                if (truck.Status.Equals(result))
                    return;

                truck.Status = result;

                await this.SaveChangesAsync();
            }
        }

        public async Task<string> GetTruckStatusAsync(Guid truckID)
        {
            if (truckID == null || truckID == Guid.Empty)
                throw new ArgumentNullException(nameof(truckID), "Truck ID cannot be null or empty.");

            Truck? truck = await this.GetAll()
                                     .Where(t => t.TruckID.Equals(truckID))
                                     .SingleOrDefaultAsync();

            if (truck != null)
            {
                return truck.Status.ToString();
            }

            return string.Empty;
        }

        public void ChangeTruckStatus(Guid truckID, string status)
        {
            if (truckID == null || string.IsNullOrEmpty(status) || truckID == Guid.Empty)
                throw new ArgumentNullException(nameof(truckID), "Truck ID cannot be null or empty.");

            if (!Enum.TryParse<TruckStatus>(status, true, out var result))
            {
                throw new ArgumentException("Invalid truck status.", nameof(status));
            }

            Truck? truck = this.GetAll()
                               .Where(t => t.TruckID.Equals(truckID))
                               .SingleOrDefault();

            if (truck != null)
            {
                if (truck.Status.Equals(result))
                    return;

                truck.Status = result;

            }
        }

        public string GetTruckStatus(Guid truckID)
        {
            if (truckID == null || truckID == Guid.Empty)
                throw new ArgumentNullException(nameof(truckID), "Truck ID cannot be null or empty.");

            Truck? truck =  this.GetAll()
                                .Where(t => t.TruckID.Equals(truckID))
                                .SingleOrDefault();

            if (truck != null)
            {
                return truck.Status.ToString();
            }

            return string.Empty;
        }
    }
}