using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleTracker.StorageService.Context;
using VehicleTracker.StorageService.Model;
using VehicleTracker.StorageService.Storage;
using Xunit;

namespace VehicleTracker.UnitTests
{
    public class VehicleStorageTests : IDisposable
    {
        private readonly VehicleContext _context;

        public VehicleStorageTests()
        {
            _context = StorageUtils.CreateContext("VehicleTests");
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task VehicleRepository_AddGetRemove_Test()
        {
            var repository = new VehicleRepository(_context);
            var vehicle = "1".IsVinOfNewVehicle("123456");

            await repository.Create(vehicle);
            var result = await repository.Get(vehicle.Id);

            result.ShouldNotBeNull();
            result.ShouldBe(vehicle);

            await repository.Delete(vehicle.Id);

            repository.Get(vehicle.Id).ShouldThrow(typeof(KeyNotFoundException));
        }

        [Fact]
        public async Task VehicleRepository_Update_Test()
        {
            var repository = new VehicleRepository(_context);
            var vehicle = "1".IsVinOfNewVehicle("123456");

            await repository.Create(vehicle);
            var result = await repository.Get(vehicle.Id);
            result.ShouldNotBeNull();

            var updVehicle = "1".IsVinOfNewVehicle("654321");
            await repository.Update(vehicle.Id, updVehicle);
            result = await repository.Get(vehicle.Id);

            result.ShouldNotBeNull();
            result.ShouldBe(updVehicle);
        }

        [Fact]
        public async Task VehicleRepository_NonExistent_Test()
        {
            var repository = new VehicleRepository(_context);

            repository.Get("1").ShouldThrow(typeof(KeyNotFoundException));
            repository.Delete("1").ShouldThrow(typeof(KeyNotFoundException));
            repository.Update("1", new Vehicle()).ShouldThrow(typeof(KeyNotFoundException));
        }


        public static Customer[] Customers = StorageUtils.CreateCustomers(2);

        public static IEnumerable<object[]> VehicleRepository_List_Params => new[]
        {
            new object[]
            {
                new[]
                {
                    Customers[0].CreateVehicle("1", "123456"),
                    Customers[0].CreateVehicle("2", "223456"),
                    Customers[0].CreateVehicle("3", "323456"),
                    Customers[1].CreateVehicle("4", "423456"),
                    Customers[1].CreateVehicle("5", "523456"),
                },
                new VehicleFilter(Customers[0].Id),
                new [] { "1", "2", "3" }
            },
            new object[]
            {
                new[]
                {
                    Customers[0].CreateVehicle("1", "123456"),
                    Customers[0].CreateVehicle("2", "223456"),
                    Customers[0].CreateVehicle("3", "323456"),
                    Customers[1].CreateVehicle("4", "423456"),
                    Customers[1].CreateVehicle("5", "523456"),
                },
                new VehicleFilter(null),
                new [] { "1", "2", "3", "4", "5" }
            }
        };

        [Theory]
        [MemberData(nameof(VehicleRepository_List_Params))]
        public async Task VehicleRepository_List_Test(IEnumerable<Vehicle> vehicles, VehicleFilter filter, IEnumerable<string> expected)
        {
            foreach (var v in vehicles)
            {
                _context.Vehicles.Add(v);
            }
            await _context.SaveChangesAsync();
            var repository = new VehicleRepository(_context);

            var result = await repository.List(filter);

            result.Select(x => x.Id).ShouldBe(expected, true);
        }
    }
}
