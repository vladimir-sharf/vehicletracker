using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using VehicleTracker.Api.Model;
using VehicleTracker.ServiceBus;
using VehicleTracker.ServiceBus.Messages;

namespace VehicleTracker.Api.Hubs
{
    public class VehicleSubscription
    {
        private readonly IHubContext<VehicleHub> _hub;

        public VehicleSubscription(IHubContext<VehicleHub> hub, IServiceBus bus) 
        {
            _hub = hub;
        }

        public Task Send(VehicleStatusMessage vehicleInfo)
            => _hub.Clients.All.SendAsync("Send", vehicleInfo);
    }
}