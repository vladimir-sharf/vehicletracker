using Shouldly;
using System.Collections.Generic;
using System.Linq;
using VehicleTracker.Api.Model;

namespace VehicleTracker.UnitTests
{
    public static class ApiUtils
    {
        public static void ShouldBe(this Vehicle vehicle, Vehicle expected)
        {
            vehicle.Id.ShouldBe(expected.Id);
            vehicle.CustomerId.ShouldBe(expected.CustomerId);
            vehicle.CustomerName.ShouldBe(expected.CustomerName);
            vehicle.RegNr.ShouldBe(expected.RegNr);
            vehicle.Status.ShouldBe(expected.Status);
        }

        public static void ShouldBe(this Customer customer, Customer expected)
        {
            customer.Id.ShouldBe(expected.Id);
            customer.Name.ShouldBe(expected.Name);
            customer.Address.ShouldBe(expected.Address);
        }

        public static void ShouldBe(this IEnumerable<Vehicle> vehicles, IEnumerable<Vehicle> expected)
        {
            vehicles.Count().ShouldBe(expected.Count());
            foreach (var vehicle in vehicles)
            {
                var exp = expected.FirstOrDefault(x => x.Id == vehicle.Id);
                exp.ShouldNotBeNull();
                vehicle.ShouldBe(exp);
            }
        }
    }
}
