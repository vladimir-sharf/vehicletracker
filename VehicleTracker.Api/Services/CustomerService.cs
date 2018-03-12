using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleTracker.Api.Model;
using VehicleTracker.Api.Storage;
using CustomerData = VehicleTracker.Api.Storage.Rest.Model.Customer;

namespace VehicleTracker.Api.Services
{
    public class CustomerService
    {
        private readonly IRepository<Guid, CustomerData, CustomerFilter> _repository;

        public CustomerService(IRepository<Guid, CustomerData, CustomerFilter> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Customer>> Get()
        {
            var cdata = await _repository.List(null);
            return cdata.Select(ConvertionUtils.ToCustomer);
        }

        public async Task<Customer> Get(Guid id) 
        {
            var cdata = await _repository.Get(id);
            return cdata.ToCustomer();
        }
    }
}
