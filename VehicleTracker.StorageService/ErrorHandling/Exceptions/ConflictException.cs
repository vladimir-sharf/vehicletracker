using System;
using System.Net;

namespace VehicleTracker.StorageService.ErrorHandling
{
    public class ConflictException : HttpStatusCodeException
    {
        public ConflictException(string message = null, Exception innerException = null)
            : base(HttpStatusCode.Conflict, message ?? "Entity with specified id already exists", innerException)
        {
        }
    }
}
