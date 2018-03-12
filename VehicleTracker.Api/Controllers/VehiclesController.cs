using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VehicleTracker.Api.Services;
using VehicleTracker.Api.Model;
using VehicleTracker.ServiceBus.Messages;
using Microsoft.AspNetCore.Authorization;

namespace VehicleTracker.Api.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class VehiclesController : Controller
    {
        private readonly VehicleService _service;

        public VehiclesController(VehicleService service)
        {
            _service = service;
        }

        [HttpGet]
        public Task<IEnumerable<Vehicle>> Get(Guid? customerId, VehicleStatus? status)
            => _service.Get(customerId, status);
    }
}
