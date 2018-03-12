using System;

namespace VehicleTracker.Api.Storage.Rest.Model
{
    public class Vehicle
    {
        public string Id { get; set; }
        public string RegNr { get; set; }
        public Guid CustomerId { get; set; }
    }
}
