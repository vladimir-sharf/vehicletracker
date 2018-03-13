using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using VehicleTracker.StorageService.Context;
using VehicleTracker.StorageService.Model;

namespace VehicleTracker.UnitTests
{
    public static class StorageUtils
    {
        public static Customer IsNameOfNewCustomer(this string name, string address = null)
            => new Customer
            {
                Id = Guid.NewGuid(),
                Name = name,
                Address = address
            };

        public static Customer[] CreateCustomers(int count) =>
            Enumerable.Range(1, count).Select(x => $"Customer {x}".IsNameOfNewCustomer()).ToArray();

        public static Vehicle CreateVehicle(this Customer customer, string id, string regNr) =>
            new Vehicle
            {
                Id = id,
                RegNr = regNr,
                CustomerId = customer.Id,
            };

        public static Vehicle IsVinOfNewVehicle(this string id, string regNr, Guid? guid = null) =>
            new Vehicle
            {
                Id = id,
                RegNr = regNr,
                CustomerId = guid ?? Guid.NewGuid(),
            };

        public static VehicleContext CreateContext(string name)
        {
            var connectionString = ConfigurationUtils.Configuration.GetSection("ConnectionStrings")["DefaultConnection"];
            connectionString = connectionString.AppendDbName($"_{name}");
            var optionsBuilder = new DbContextOptionsBuilder<VehicleContext>()
                .UseSqlServer(connectionString);

            var context = new VehicleContext(optionsBuilder.Options);
            context.Database.EnsureCreated();
            return context;
        }

        public static string AppendDbName(this string connectionString, string suffix)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            connectionStringBuilder.InitialCatalog = connectionStringBuilder.InitialCatalog + suffix;
            return connectionStringBuilder.ConnectionString;
        }

        public static void ShouldBe(this Vehicle actual, Vehicle expected)
        {
            actual.Id.ShouldBe(expected.Id);
            actual.CustomerId.ShouldBe(expected.CustomerId);
            actual.RegNr.ShouldBe(expected.RegNr);
        }

        public static void ShouldBe(this Customer actual, Customer expected)
        {
            actual.Id.ShouldBe(expected.Id);
            actual.Name.ShouldBe(expected.Name);
            actual.Address.ShouldBe(expected.Address);
        }
    }
}
