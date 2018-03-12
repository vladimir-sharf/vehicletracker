using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleTracker.Api.Model;
using VehicleTracker.Api.Storage;
using VehicleTracker.Api.Storage.StatusCache;
using VehicleTracker.ServiceBus;
using VehicleTracker.ServiceBus.Messages;
using VehicleData = VehicleTracker.Api.Storage.Rest.Model.Vehicle;

namespace VehicleTracker.Api.Services
{
    public class VehicleService
    {
        private readonly IRepository<string, VehicleData, VehicleFilter> _repository;
        private readonly CustomerService _customerService; 
        private readonly StatusCache _statusCache;
        private readonly IServiceBus _bus;
        private readonly ILogger _logger;

        public VehicleService(IRepository<string, VehicleData, VehicleFilter> repository, 
            CustomerService customerService, 
            IServiceBus bus, 
            StatusCache statusCache, 
            ILogger<VehicleService> logger)
        {
            _repository = repository;
            _statusCache = statusCache;
            _customerService = customerService;
            _bus = bus;
            _logger = logger;
        }

        public async Task<IEnumerable<Vehicle>> Get(Guid? customerId, VehicleStatus? status)
        {
            _logger.LogInformation($"Request vehicles CustomerId={customerId?.ToString() ?? "All"}, Status={status?.ToString() ?? "All"}");
            var filter = new VehicleFilter
            {
                CustomerId = customerId
            };

            var fromStorage = await _repository.List(filter);
            _logger.LogTrace($"Found {fromStorage.Count()} vehicles");

            await _bus.SendSubscribeRequest(new VehicleTrackSubscribeRequest(fromStorage.Select(x => x.Id)));
            _logger.LogInformation($"Subscribed on {fromStorage.Count()} vehicles");

            var statuses = new Dictionary<string, VehicleStatusMessage>();
            var customers = new Dictionary<Guid, Customer>();
            foreach (var vehicle in fromStorage)
            {
                var vehicleInfo = await _statusCache.Get(vehicle.Id);
                statuses.Add(vehicle.Id, vehicleInfo);

                if (!customers.ContainsKey(vehicle.CustomerId)) {
                    customers[vehicle.CustomerId] = await _customerService.Get(vehicle.CustomerId);
                }
            }
            _logger.LogTrace($"Statuses extracted");

            var result = fromStorage.Select(v => v.ToVehicle(statuses[v.Id], customers[v.CustomerId]));
            if (status != null)
                result = result.Where(v => v.Status == status);

            return result;
        }
    }
}
