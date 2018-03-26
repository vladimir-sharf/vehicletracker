using System;
using System.Net;

namespace VehicleTracker.StorageService.ErrorHandling
{
    public class BadRequestException : HttpStatusCodeException
    {
        public BadRequestException(string message = null, Exception innerException = null)
            : base(HttpStatusCode.BadRequest, message ?? "Bad Request", innerException)
        {
        }
    }
}
