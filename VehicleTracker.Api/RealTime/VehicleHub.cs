using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace VehicleTracker.Api.RealTime
{
    [Authorize(JwtBearerDefaults.AuthenticationScheme)]
    public class VehicleHub : Hub
    {
        private readonly ILogger _logger;

        public VehicleHub(ILogger<VehicleHub> logger)
        {
            _logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            _logger.LogInformation($"Connection {Context.Connection.ConnectionId} established to Vehicle hub from {Context.Connection.RemoteIpAddress}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation($"Connection {Context.Connection.ConnectionId} disconnected");
            return base.OnDisconnectedAsync(exception);
        }
    }
}