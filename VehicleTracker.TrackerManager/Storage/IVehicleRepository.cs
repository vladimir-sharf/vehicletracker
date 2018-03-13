using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracker.TrackerManager.Model;

namespace VehicleTracker.TrackerManager.Storage
{
    public interface IVehicleRepository
    {
        Task Add(VehicleSubscription value);
        Task<IEnumerable<VehicleSubscription>> Get();
        Task Remove(string id);
    }
}