using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using VehicleTracker.StorageService.Storage;
using VehicleTracker.StorageService.Context;
using VehicleTracker.StorageService.Model;

namespace VehicleTracker.UnitTests
{
    public class CustomerStorageTests : IDisposable
    {
        private readonly VehicleContext _context;

        public CustomerStorageTests()
        {
            _context = StorageUtils.CreateContext("CustomerTests");
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task CustomerRepository_AddGetRemove_Test()
        {
            var repository = new CustomerRepository(_context);
            var customer = "1".IsNameOfNewCustomer("123");
            await repository.Create(customer);
            var result = await repository.Get(customer.Id);

            result.ShouldNotBeNull();
            result.ShouldBe(customer);

            await repository.Delete(customer.Id);

            repository.Get(customer.Id).ShouldThrow(typeof(KeyNotFoundException));
        }

        [Fact]
        public async Task CustomerRepository_Update_Test()
        {
            var repository = new CustomerRepository(_context);
            var customer = "1".IsNameOfNewCustomer("123");

            await repository.Create(customer);
            var result = await repository.Get(customer.Id);
            result.ShouldNotBeNull();

            var updCustomer = new Customer
            {
                Id = customer.Id,
                Name = "2",
                Address = "321"
            };

            await repository.Update(customer.Id, updCustomer);
            result = await repository.Get(customer.Id);

            result.ShouldNotBeNull();
            result.ShouldBe(updCustomer);
        }

        [Fact]
        public async Task CustomerRepository_NonExistent_Test()
        {
            var repository = new CustomerRepository(_context);

            repository.Get(Guid.NewGuid()).ShouldThrow(typeof(KeyNotFoundException));
            repository.Delete(Guid.NewGuid()).ShouldThrow(typeof(KeyNotFoundException));
            repository.Update(Guid.NewGuid(), new Customer()).ShouldThrow(typeof(KeyNotFoundException));
        }


        public static Customer[] Customers = StorageUtils.CreateCustomers(5);

        public static IEnumerable<object[]> CustomerRepository_List_Params => new[]
        {
            new object[]
            {
                Customers,
                new CustomerFilter(),
                Customers.Select(x => x.Id).ToArray(),
            },
        };

        [Theory]
        [MemberData(nameof(CustomerRepository_List_Params))]
        public async Task CustomerRepository_List_Test(IEnumerable<Customer> customers, CustomerFilter filter, IEnumerable<Guid> expected)
        {
            foreach (var v in customers)
            {
                _context.Customers.Add(v);
            }
            await _context.SaveChangesAsync();
            var repository = new CustomerRepository(_context);

            var result = await repository.List(filter);

            result.Select(x => x.Id).ShouldBe(expected, true);
        }
    }
}
