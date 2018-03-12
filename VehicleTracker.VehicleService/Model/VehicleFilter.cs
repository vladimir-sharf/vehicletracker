using System;
using VehicleTracker.ServiceBus.Messages;

namespace VehicleTracker.VehicleService.Model
{
    public class VehicleFilter
    {
        public VehicleFilter(Guid? customerId)
        {
            CustomerId = customerId;
        }

        public Guid? CustomerId { get; set; }
    }
}
