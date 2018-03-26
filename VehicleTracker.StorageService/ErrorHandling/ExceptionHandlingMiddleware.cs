using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace VehicleTracker.StorageService.ErrorHandling
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (HttpStatusCodeException ex)
            {
                await SetResponse(context, (int)ex.StatusCode, ex, ex.Message ?? "Not Found");
            }
            catch (Exception ex)
            {
                await SetResponse(context, 500, ex);
            }
        }

        private async Task SetResponse(HttpContext context, int statusCode, Exception ex, string message = null)
        {
            if (context.Response.HasStarted)
            {
                _logger.LogWarning("The response has already started, the http status code middleware will not be executed.");
                throw ex;
            }

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(message ?? ex.Message);
        }


    }
}
