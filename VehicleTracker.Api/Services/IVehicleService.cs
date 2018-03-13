using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracker.Api.Model;
using VehicleTracker.ServiceBus.Messages;

namespace VehicleTracker.Api.Services
{
    public interface IVehicleService
    {
        Task<IEnumerable<Vehicle>> Get(Guid? customerId, VehicleStatus? status);
    }
}