using System;

namespace VehicleTracker.ServiceBus.Messages
{
    public class VehicleStatusMessage
    {
        public string Id { get; set; }
        public VehicleStatus Status { get; set; }
        public DateTime TimeUtc { get; set; }

        public VehicleStatusMessage(string id, VehicleStatus status)
        {
            Id = id;
            Status = status;
            TimeUtc = DateTime.UtcNow;
        }
    }
}
