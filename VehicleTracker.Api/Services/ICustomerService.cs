using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracker.Api.Model;

namespace VehicleTracker.Api.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> Get();
        Task<Customer> Get(Guid id);
    }
}