using System;

namespace VehicleTracker.Api.Storage.Rest.Model
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
