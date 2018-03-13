using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using VehicleTracker.Api.Model;
using VehicleTracker.ServiceBus;
using VehicleTracker.ServiceBus.Messages;
using Microsoft.Extensions.Logging;

namespace VehicleTracker.Api.RealTime
{
    public class VehicleSubscription
    {
        private readonly IHubContext<VehicleHub> _hub;
        private readonly ILogger _logger;

        public VehicleSubscription(IHubContext<VehicleHub> hub, IServiceBus bus, ILogger<VehicleSubscription> logger) 
        {
            _hub = hub;
            _logger = logger;
        }

        public Task Send(VehicleStatusMessage vehicleInfo)
        {
            _logger.LogTrace($"Sending to clients: {vehicleInfo.Id} was {vehicleInfo.Status} at {vehicleInfo.TimeUtc}");
            return _hub.Clients.All.SendAsync("Send", vehicleInfo);
        }
    }
}