using System;
using VehicleTracker.ServiceBus.Messages;

namespace VehicleTracker.Api.Model
{
    public class VehicleStatusVm 
    {
        public string Id { get; set; }
        public VehicleStatus Status { get; set; }
        public string StatusName { get; set; }
        public DateTime TimeUtc { get; set; }
    }
}