using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracker.TrackerManager.Model;

namespace VehicleTracker.TrackerManager.Storage
{
    public class VehicleRepositoryInMemory : IVehicleRepository
    {
        private readonly ConcurrentDictionary<string, VehicleSubscription> _storage = new ConcurrentDictionary<string, VehicleSubscription>();

        public Task<IEnumerable<VehicleSubscription>> Get()
            => Task.FromResult<IEnumerable<VehicleSubscription>>(_storage.Values);

        public Task Add(VehicleSubscription value)
            => Task.FromResult(_storage[value.Id] = value);

        public Task Remove(string id)
            => Task.FromResult(_storage.TryRemove(id, out var b));
    }
}
