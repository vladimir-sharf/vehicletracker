using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VehicleTracker.Api.ViewModel;
using Microsoft.Extensions.Options;
using VehicleTracker.Api.Configuration;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VehicleTracker.Api.Controllers
{
    [Route("{*url}")]
    public class HomeController : Controller
    {
        private readonly IOptions<AuthConfiguration> _options;

        public HomeController(IOptions<AuthConfiguration> options)
        {
            _options = options;
        }

        public IActionResult Index()
        {
            var model = new HomeVm
            {
                AuthConfigurationJson = JsonConvert.SerializeObject(_options.Value)
            };
            return View(model);
        }
    }
}
