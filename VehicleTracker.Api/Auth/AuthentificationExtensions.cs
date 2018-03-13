using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Threading.Tasks;
using VehicleTracker.Api.Configuration;

namespace VehicleTracker.Api.Auth
{
    public static class AuthentificationExtensions
    {
        public static IServiceCollection AddVehicleTrackerAuth(this IServiceCollection services, IConfiguration configuration)
        {
            var authSection = configuration.GetSection("Auth");
            services.Configure<AuthConfiguration>(authSection);
            AuthConfiguration config = new AuthConfiguration();
            authSection.Bind(config);

            services.AddAuthorization(options =>
            {
                options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireClaim(ClaimTypes.NameIdentifier);
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.Authority = config.Authority;
                    options.Audience = config.Scope;

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            if (!string.IsNullOrEmpty(accessToken))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            return services;
        }
    }
}
