using System;
using System.Net;

namespace VehicleTracker.StorageService.ErrorHandling
{
    public class HttpStatusCodeException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public HttpStatusCodeException(HttpStatusCode statusCode, string message = null, Exception innerException = null)
            : base (message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
