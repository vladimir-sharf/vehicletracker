using System;
using VehicleTracker.ServiceBus.Messages;

namespace VehicleTracker.Api.Model
{
    public class Vehicle
    {
        public string Id { get; set; }
        public string RegNr { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public VehicleStatus Status { get; set; }
        public DateTime TimeUtc { get; set; }
    }
}
