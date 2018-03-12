using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VehicleTracker.VehicleService.Model;
using VehicleTracker.VehicleService.Storage;
using System;

namespace VehicleTracker.VehicleService.Controllers
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
