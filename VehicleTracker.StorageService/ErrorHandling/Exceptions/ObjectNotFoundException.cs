using System;
using System.Net;

namespace VehicleTracker.StorageService.ErrorHandling
{
    public class ObjectNotFoundException : HttpStatusCodeException
    {
        public ObjectNotFoundException(string message = null, Exception innerException = null)
            : base(HttpStatusCode.NotFound, message ?? "Not Found", innerException)
        {
        }
    }
}
