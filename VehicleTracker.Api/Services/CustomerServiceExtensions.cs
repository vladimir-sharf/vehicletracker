using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracker.Api.Model;
using VehicleData = VehicleTracker.Api.Storage.Rest.Model.Vehicle;

namespace VehicleTracker.Api.Services
{
    public static class CustomerServiceExtensions
    {
        public static async Task<Dictionary<Guid, Customer>> ExtractCustomers(this ICustomerService service, IEnumerable<VehicleData> fromStorage)
        {
            var customers = new Dictionary<Guid, Customer>();
            foreach (var vehicle in fromStorage)
            {
                if (!customers.ContainsKey(vehicle.CustomerId))
                {
                    customers[vehicle.CustomerId] = await service.Get(vehicle.CustomerId);
                }
            }

            return customers;
        }
    }
}
