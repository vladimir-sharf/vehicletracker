using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VehicleTracker.StorageService.Model;
using VehicleTracker.StorageService.Storage;
using System;

namespace VehicleTracker.StorageService.Controllers
{
    [Route("customers")]
    public class CustomersController : RepositoryControllerBase<Guid, Customer, CustomerFilter>
    {
        public CustomersController(IRepository<Guid, Customer, CustomerFilter> customerRepository)
            : base(customerRepository)
        {
        }

        [HttpGet]
        public Task<IEnumerable<Customer>> Get()
            => List(null);
    }
}
