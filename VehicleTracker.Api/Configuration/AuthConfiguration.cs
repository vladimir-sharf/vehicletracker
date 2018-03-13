using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VehicleTracker.Api.Configuration
{
    public class AuthConfiguration
    {
        public string Authority { get; set; }
        public string JsAuthority { get; set; }
        public string RedirectUri { get; set; }
        public string PostLogoutRedirectUri { get; set; }
        public string Scope { get; set; }
        public string ClientId { get; set; }
    }
}
