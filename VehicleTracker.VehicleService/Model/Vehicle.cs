using System;

namespace VehicleTracker.VehicleService.Model
{
    public class Vehicle
    {
        public string Id { get; set; }
        public string RegNr { get; set; }
        public Guid CustomerId { get; set; }
    }
}
