using Microsoft.AspNetCore.Builder;

namespace VehicleTracker.StorageService.ErrorHandling
{
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
