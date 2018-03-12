using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VehicleTracker.Api.Services;
using VehicleTracker.Api.Model;
using Microsoft.AspNetCore.Authorization;

namespace VehicleTracker.Api.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly CustomerService _service;

        public CustomersController(CustomerService service)
        {
            _service = service;
        }

        [HttpGet]
        public Task<IEnumerable<Customer>> Get()
            => _service.Get();
    }
}
