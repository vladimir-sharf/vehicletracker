using System;
using System.Linq;
using VehicleTracker.StorageService.Context;
using VehicleTracker.StorageService.Model;

namespace VehicleTracker.StorageService
{
    public static class DbInitializer
    {
        public static void Initialize(VehicleContext context)
        {
            context.Database.EnsureCreated();

            if (context.Customers.Any())
            {
                return;
            }

            context
                .CreateKalles()
                .CreateJohans()
                .CreateHaralds()
                .SaveChanges();
        }

        private static VehicleContext CreateKalles(this VehicleContext context)
        {
            var customerId = Guid.NewGuid();
            context.Customers.Add(new Customer { Id = customerId, Name = "Kalles Grustransporter AB", Address = "Cementvägen 8, 111 11 Södertälje" });
            context.Vehicles.Add(customerId.NewVehicle("YS2R4X20005399401", "ABC123"));
            context.Vehicles.Add(customerId.NewVehicle("VLUR4X20009093588", "DEF456"));
            context.Vehicles.Add(customerId.NewVehicle("VLUR4X20009048066", "GHI789"));
            return context;
        }

        private static VehicleContext CreateJohans(this VehicleContext context)
        {
            var customerId = Guid.NewGuid();
            context.Customers.Add(new Customer { Id = customerId, Name = "Johans Bulk AB", Address = "Balkvägen 12, 222 22 Stockholm" });
            context.Vehicles.Add(customerId.NewVehicle("YS2R4X20005388011", "JKL012"));
            context.Vehicles.Add(customerId.NewVehicle("YS2R4X20005387949", "MNO345"));
            return context;
        }

        private static VehicleContext CreateHaralds(this VehicleContext context)
        {
            var customerId = Guid.NewGuid();
            context.Customers.Add(new Customer { Id = customerId, Name = "Haralds Värdetransporter AB", Address = "BalkvägenBudgetvägen 1, 333 33 Uppsala" });
            context.Vehicles.Add(customerId.NewVehicle("YS2R4X20005387765", "PQR678"));
            context.Vehicles.Add(customerId.NewVehicle("YS2R4X20005387055", "STU901"));
            return context;
        }

        private static Vehicle NewVehicle(this Guid customerId, string id, string regNr)
            => new Vehicle { Id = id, RegNr = regNr, CustomerId = customerId };
    }
}
