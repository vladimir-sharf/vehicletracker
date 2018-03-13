using System;

namespace VehicleTracker.ServiceBus.Messages
{
    public class VehicleTrackRequest
    {
        public VehicleTrackRequest(string id)
        {
            Id = id;
            TimeUtc = DateTime.UtcNow;
        }

        public string Id { get; }
        public DateTime TimeUtc { get; set; }
    }
}
