using System;
using System.Collections.Generic;
using Xunit;
using VehicleTracker.ServiceBus.Messages;
using VehicleTracker.Api.Model;
using VehicleTracker.Api.Services;
using Shouldly;
using System.Threading.Tasks;
using VehicleTracker.ServiceBus;
using System.Linq;

namespace VehicleTracker.UnitTests
{
    public class VehicleServiceTests
    {
        public static IEnumerable<object[]> VehicleService_Get_Test_Data = new[]
        {
            new object[]
            {
                MockData.MockCustomers[0].Id,
                VehicleStatus.Connected,
                new[] {
                    new Vehicle
                    {
                        CustomerId = MockData.MockCustomers[0].Id,
                        Id = "1",
                        RegNr = "123",
                        CustomerName = MockData.MockCustomers[0].Name,
                        Status = VehicleStatus.Connected
                    },
                    new Vehicle
                    {
                        CustomerId = MockData.MockCustomers[0].Id,
                        Id = "3",
                        RegNr = "789",
                        CustomerName = MockData.MockCustomers[0].Name,
                        Status = VehicleStatus.Connected
                    },
                },
                3
            },
            new object[]
            {
                null,
                VehicleStatus.Connected,
                new[] {
                    new Vehicle
                    {
                        CustomerId = MockData.MockCustomers[0].Id,
                        Id = "1",
                        RegNr = "123",
                        CustomerName = MockData.MockCustomers[0].Name,
                        Status = VehicleStatus.Connected
                    },
                    new Vehicle
                    {
                        CustomerId = MockData.MockCustomers[0].Id,
                        Id = "3",
                        RegNr = "789",
                        CustomerName = MockData.MockCustomers[0].Name,
                        Status = VehicleStatus.Connected
                    },
                    new Vehicle
                    {
                        CustomerId = MockData.NonExistentCustomerId,
                        Id = "5",
                        RegNr = "111",
                        CustomerName = null,
                        Status = VehicleStatus.Connected
                    },
                },
                5
            },
            new object[]
            {
                MockData.MockCustomers[0].Id,
                null,
                new[] {
                    new Vehicle
                    {
                        CustomerId = MockData.MockCustomers[0].Id,
                        Id = "1",
                        RegNr = "123",
                        CustomerName = MockData.MockCustomers[0].Name,
                        Status = VehicleStatus.Connected
                    },
                    new Vehicle
                    {
                        CustomerId = MockData.MockCustomers[0].Id,
                        Id = "2",
                        RegNr = "456",
                        CustomerName = MockData.MockCustomers[0].Name,
                        Status = VehicleStatus.Disconnected
                    },
                    new Vehicle
                    {
                        CustomerId = MockData.MockCustomers[0].Id,
                        Id = "3",
                        RegNr = "789",
                        CustomerName = MockData.MockCustomers[0].Name,
                        Status = VehicleStatus.Connected
                    },
                },
                3
            }
        };

        [Theory]
        [MemberData(nameof(VehicleService_Get_Test_Data))]
        public async Task VehicleService_Get_Test(Guid? customerId, VehicleStatus? status, IEnumerable<Vehicle> expected, int subscribeCount)
        {
            var repository = MockData.CreateVehicleRepository();
            var customerService = MockData.CreateCustomerService();
            var statusCache = MockData.CreateStatusCache();
            var logger = CommonUtils.CreateLogger<VehicleService>();

            string address = null;
            var requests = new List<VehicleTrackSubscribeRequest>();
            InteractionType? interactionType = null;
            var serviceBus = CommonUtils.CreateServiceBus((string a, VehicleTrackSubscribeRequest r, InteractionType t) =>
            {
                address = a;
                requests.Add(r);
                interactionType = t;
            });

            var vehicleService = new VehicleService(repository, customerService, serviceBus, statusCache, logger);

            var result = await vehicleService.Get(customerId, status);
            result.ShouldBe(expected);

            address.ShouldBe(QueueNames.SubscribeQueueName);
            interactionType.ShouldBe(InteractionType.CompetingConsumers);
            requests.Count.ShouldBe(1);
            requests[0].Ids.Count().ShouldBe(subscribeCount);
        }
    }
}
