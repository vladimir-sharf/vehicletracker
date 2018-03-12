using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace VehicleTracker.Api.Hubs
{
    [Authorize(JwtBearerDefaults.AuthenticationScheme)]
    public class VehicleHub : Hub
    {
    }
}