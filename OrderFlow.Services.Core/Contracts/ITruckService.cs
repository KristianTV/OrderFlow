﻿using OrderFlow.Data.Models;
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
    }
}
