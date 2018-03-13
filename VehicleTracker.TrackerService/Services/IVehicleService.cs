using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleTracker.ServiceBus.Messages;

namespace VehicleTracker.TrackerService
{
    public interface IVehicleService
    {
        Task<VehicleStatusMessage> Ping(VehicleTrackRequest req);
        Task<VehicleStatusMessage> Ping(string id);
    }
}
