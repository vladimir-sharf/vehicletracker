using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VehicleTracker.ServiceBus.Messages;

namespace VehicleTracker.TrackerService.Controllers
{
    [Route("[controller]")]
    public class VehiclesController : Controller
    {
        private readonly IVehicleService _vehicleService;

        public VehiclesController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpGet("ping/{id}")]
        public Task<VehicleStatusMessage> Ping(string id)
            => _vehicleService.Ping(id);
    }
}
