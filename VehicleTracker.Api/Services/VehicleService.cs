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
    public class VehicleService : IVehicleService
    {
        private readonly IRepository<string, VehicleData, VehicleFilter> _repository;
        private readonly ICustomerService _customerService; 
        private readonly IStatusCache _statusCache;
        private readonly IServiceBus _bus;
        private readonly ILogger _logger;

        public VehicleService(IRepository<string, VehicleData, VehicleFilter> repository,
            ICustomerService customerService, 
            IServiceBus bus,
            IStatusCache statusCache, 
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
            var storageData = await _repository.List(filter);
            _logger.LogTrace($"Found {storageData.Count()} vehicles");

            await _bus.SendSubscribeRequest(new VehicleTrackSubscribeRequest(storageData.Select(x => x.Id)));
            _logger.LogInformation($"Subscribed on {storageData.Count()} vehicles");

            var statuses = await _statusCache.ExtractStatuses(storageData);
            _logger.LogTrace($"Statuses extracted");

            var customers = await _customerService.ExtractCustomers(storageData);
            _logger.LogTrace($"Customers extracted");

            var result = storageData.Select(v => v.ToVehicle(statuses[v.Id], customers[v.CustomerId]));
            if (status != null)
                result = result.Where(v => v.Status == status);

            return result;
        }
    }
}
