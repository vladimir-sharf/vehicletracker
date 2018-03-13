using System;

namespace VehicleTracker.TrackerManager.Model
{
    public class VehicleSubscription
    {
        public string Id { get; set; }
        public DateTime TimeUtc { get; set; }

        public VehicleSubscription(string id, DateTime timeUtc)
        {
            Id = id;
            TimeUtc = timeUtc;
        }
    }
}
