using System;
using VehicleTracker.ServiceBus.Messages;

namespace VehicleTracker.StorageService.Model
{
    public class VehicleFilter
    {
        public VehicleFilter(Guid? customerId, string id)
        {
            CustomerId = customerId;
            Id = id;
        }

        public Guid? CustomerId { get; set; }

        public string Id { get; set;}        
    }
}
