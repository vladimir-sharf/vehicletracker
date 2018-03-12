using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracker.TrackerService.Model;

namespace VehicleTracker.TrackerService.Storage
{
    public interface IVehicleRepository
    {
        Task Add(VehicleSubscription value);
        Task<IEnumerable<VehicleSubscription>> Get();
        Task Remove(string id);
    }
}