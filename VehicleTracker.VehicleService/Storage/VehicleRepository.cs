using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleTracker.VehicleService.Context;
using VehicleTracker.VehicleService.Model;

namespace VehicleTracker.VehicleService.Storage
{
    public class VehicleRepository : IRepository<string, Vehicle, VehicleFilter>
    {
        private readonly VehicleContext _context;

        public VehicleRepository(VehicleContext context)
        {
            _context = context;
        }

        public async Task<Vehicle> Create(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }

        public async Task Delete(string id)
        {
            var vehicle = await Get(id);
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Vehicle>> List(VehicleFilter filter)
        {
            var task = filter?.CustomerId == null ?
                _context.Vehicles.ToListAsync() : 
                _context.Vehicles
                    .Where(x => x.CustomerId == filter.CustomerId)
                    .ToListAsync();
            var result = await task;
            return result;
        }

        public async Task<Vehicle> Get(string id)
            => await _context.Vehicles.FirstOrDefaultAsync(x => x.Id == id) ?? throw new KeyNotFoundException();

        public async Task<Vehicle> Update(string id, Vehicle vehicle)
        {
            var existing = await Get(id);
            existing.RegNr = vehicle.RegNr;
            existing.CustomerId = vehicle.CustomerId;
            await _context.SaveChangesAsync();
            return existing;
        }
    }
}
