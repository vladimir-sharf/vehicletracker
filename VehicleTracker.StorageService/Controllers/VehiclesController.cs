using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VehicleTracker.StorageService.Model;
using VehicleTracker.StorageService.Storage;
using System;

namespace VehicleTracker.StorageService.Controllers
{
    [Route("vehicles")]
    public class VehiclesController : RepositoryControllerBase<string, Vehicle, VehicleFilter>
    {
        public VehiclesController(IRepository<string, Vehicle, VehicleFilter> vehicleRepository)
            : base(vehicleRepository)
        {
        }

        [HttpGet]
        public Task<IEnumerable<Vehicle>> Get(Guid? customerId)
            => List(new VehicleFilter(customerId));
    }
}
