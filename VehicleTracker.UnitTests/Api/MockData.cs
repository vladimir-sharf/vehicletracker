using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleTracker.Api.Model;
using VehicleTracker.Api.Services;
using VehicleTracker.Api.Storage;
using VehicleTracker.Api.Storage.StatusCache;
using VehicleTracker.ServiceBus.Messages;
using VehicleData = VehicleTracker.Api.Storage.Rest.Model.Vehicle;

namespace VehicleTracker.UnitTests
{
    public static class MockData
    {
        public static Guid NonExistentCustomerId = Guid.NewGuid();

        public static Customer[] MockCustomers = new[]
        {
            new Customer {
                Id = Guid.NewGuid(),
                Name = "Customer 1"
            },
            new Customer {
                Id = Guid.NewGuid(),
                Name = "Customer 2"
            },
        };

        public static VehicleData[] MockVehicleData = new[]
        {
            new VehicleData
            {
                CustomerId = MockCustomers[0].Id,
                Id = "1",
                RegNr = "123"
            },
            new VehicleData
            {
                CustomerId = MockCustomers[0].Id,
                Id = "2",
                RegNr = "456"
            },
            new VehicleData
            {
                CustomerId = MockCustomers[0].Id,
                Id = "3",
                RegNr = "789"
            },
            new VehicleData
            {
                CustomerId = MockCustomers[1].Id,
                Id = "4",
                RegNr = "000"
            },
            new VehicleData
            {
                CustomerId = NonExistentCustomerId,
                Id = "5",
                RegNr = "111"
            },
        };

        public static Dictionary<string, VehicleStatusMessage> MockVehicleCache = new Dictionary<string, VehicleStatusMessage>()
        {
            {"1", new VehicleStatusMessage("1", VehicleStatus.Connected) },
            {"2", new VehicleStatusMessage("2", VehicleStatus.Disconnected) },
            {"3", new VehicleStatusMessage("3", VehicleStatus.Connected) },
            {"5", new VehicleStatusMessage("5", VehicleStatus.Connected) },
        };

        public static IRepository<string, VehicleData, VehicleFilter> CreateVehicleRepository()
        {
            var repository = new Mock<IRepository<string, VehicleData, VehicleFilter>>();
            repository.Setup(x => x.List(It.IsAny<VehicleFilter>()))
                .Returns(async (VehicleFilter filter) => filter?.CustomerId != null ?
                    MockVehicleData.Where(x => x.CustomerId == filter.CustomerId).ToArray() :
                    MockVehicleData);
            return repository.Object;
        }

        public static ICustomerService CreateCustomerService()
        {
            var customerService = new Mock<ICustomerService>();
            customerService.Setup(x => x.Get())
                .Returns(Task.FromResult((IEnumerable<Customer>)MockCustomers));
            customerService.Setup(x => x.Get(It.IsAny<Guid>()))
                .Returns((Guid id) => Task.FromResult(MockCustomers.FirstOrDefault(x => x.Id == id)));
            return customerService.Object;
        }

        public static IStatusCache CreateStatusCache()
        {
            var statusCache = new Mock<IStatusCache>();
            statusCache.Setup(x => x.Get(It.IsAny<string>()))
                .Returns((string id) => Task.FromResult(MockVehicleCache.ContainsKey(id) ? MockVehicleCache[id] : null));
            return statusCache.Object;
        }
    }
}
