﻿using VehicleTracker.Api.Model;
using VehicleTracker.ServiceBus.Messages;
using VehicleData = VehicleTracker.Api.Storage.Rest.Model.Vehicle;
using CustomerData = VehicleTracker.Api.Storage.Rest.Model.Customer;
using System;

namespace VehicleTracker.Api.Services
{
    public static class ConvertionUtils
    {
        public static Vehicle ToVehicle(this VehicleData vdata, VehicleStatusMessage info, Customer customer)
            => new Vehicle
            {
                Id = vdata.Id,
                RegNr = vdata.RegNr,
                CustomerId = vdata.CustomerId,
                CustomerName = customer?.Name,
                Status = info?.Status ?? VehicleStatus.Unknown,
                TimeUtc = info?.TimeUtc ?? DateTime.UtcNow,
            };

        public static Customer ToCustomer(this CustomerData data)
            => new Customer
            {
                Id = data.Id,
                Name = data.Name,
                Address = data.Address
            };
    }
}
