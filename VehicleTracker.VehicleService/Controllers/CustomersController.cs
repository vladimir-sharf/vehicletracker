using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VehicleTracker.VehicleService.Model;
using VehicleTracker.VehicleService.Storage;
using System;

namespace VehicleTracker.VehicleService.Controllers
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
