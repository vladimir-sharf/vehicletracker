using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracker.StorageService.Context;
using VehicleTracker.StorageService.ErrorHandling;
using VehicleTracker.StorageService.Model;

namespace VehicleTracker.StorageService.Storage
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
            try
            {
                _context.Vehicles.Add(vehicle);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (e.IsDuplicateError())
                    throw new ConflictException();
            }
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
            var task = _context.Vehicles
                .Filter(filter)
                .ToListAsync();
            var result = await task;
            return result;
        }

        public async Task<Vehicle> Get(string id)
            => await _context.Vehicles.FirstOrDefaultAsync(x => x.Id == id) ?? throw new ObjectNotFoundException();

        public async Task<Vehicle> Update(string id, Vehicle vehicle)
        {
            if (id != vehicle.Id)
                throw new BadRequestException("Ids in url and in payload are different");

            var existing = await Get(id);
            existing.RegNr = vehicle.RegNr;
            existing.CustomerId = vehicle.CustomerId;
            await _context.SaveChangesAsync();
            return existing;
        }
    }
}
