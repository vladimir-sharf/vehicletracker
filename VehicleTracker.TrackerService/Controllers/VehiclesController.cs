﻿using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracker.TrackerService.Model;
using VehicleTracker.TrackerService.Services;

namespace VehicleTracker.TrackerService.Controllers
{
    [Route("[controller]")]
    public class VehiclesController : Controller
    {
        private readonly VehicleService _vehicleService;

        public VehiclesController(VehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpGet("")]
        public Task<IEnumerable<VehicleSubscription>> Get()
            => _vehicleService.Get();

        [HttpPut("{id}")]
        public Task Put(string id)
            => _vehicleService.Add(id);

        [HttpDelete("{id}")]
        public Task Delete(string id)
            => _vehicleService.Remove(id);
    }
}
