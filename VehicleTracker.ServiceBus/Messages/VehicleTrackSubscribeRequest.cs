using System.Collections.Generic;

namespace VehicleTracker.ServiceBus.Messages
{
    public class VehicleTrackSubscribeRequest
    {
        public VehicleTrackSubscribeRequest(IEnumerable<string> ids)
        {
            Ids = ids;
        }

        public IEnumerable<string> Ids { get; }
    }
}
