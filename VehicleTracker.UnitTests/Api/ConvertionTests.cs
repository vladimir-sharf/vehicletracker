using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using VehicleData = VehicleTracker.Api.Storage.Rest.Model.Vehicle;
using CustomerData = VehicleTracker.Api.Storage.Rest.Model.Customer;
using VehicleTracker.ServiceBus.Messages;
using VehicleTracker.Api.Model;
using VehicleTracker.Api.Services;
using Shouldly;

namespace VehicleTracker.UnitTests
{
    public class ConvertionTests
    {
        private static Guid CustomerId = Guid.NewGuid();
        public static IEnumerable<object[]> ToVehicle_Test_Data = new[] {
                new object[]
                {
                    new VehicleData
                    {
                        CustomerId = CustomerId,
                        Id = "1",
                        RegNr = "123"
                    },
                    new VehicleStatusMessage("2", VehicleStatus.Connected),
                    new Customer
                    {
                        Id = CustomerId,
                        Name = "Customer 1"
                    },
                    new Vehicle
                    {
                        CustomerId = CustomerId,
                        Id = "1",
                        RegNr = "123",
                        CustomerName = "Customer 1",
                        Status = VehicleStatus.Connected,
                    }
                },
                new object[]
                {
                    new VehicleData
                    {
                        CustomerId = CustomerId,
                        Id = "1",
                        RegNr = "123"
                    },
                    null,
                    null,
                    new Vehicle
                    {
                        CustomerId = CustomerId,
                        Id = "1",
                        RegNr = "123",
                        CustomerName = null,
                        Status = VehicleStatus.Unknown,
                    }
                },
            };

        [Theory]
        [MemberData(nameof(ToVehicle_Test_Data))]
        public void ToVehicle_Test(VehicleData vehicleData, VehicleStatusMessage info, Customer customer, Vehicle expected) 
        {
            var result = vehicleData.ToVehicle(info, customer);
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
        }

        public static IEnumerable<object[]> ToCustomer_Test_Data = new[] {
                new object[]
                {
                    new CustomerData
                    {
                        Id = CustomerId,
                        Name = "Customer 1",
                        Address = "123"
                    },
                    new Customer
                    {
                        Id = CustomerId,
                        Name = "Customer 1",
                        Address = "123"
                    }
                },
            };

        [Theory]
        [MemberData(nameof(ToCustomer_Test_Data))]
        public void ToCustomer_Test(CustomerData customerData, Customer expected)
        {
            var result = customerData.ToCustomer();
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
        }
    }
}
