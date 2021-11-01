using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace YourCarBud.WebApi.Modules.StartupExtensions
{
    internal static class CorsExtensionsMethods
    {

        public static IServiceCollection AddCorsSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var allowedHosts = configuration.GetValue<string>("AllowedHosts");
            services.AddCors(options =>
            {
                options.AddPolicy("AllowedHosts",
                    builder =>
                    {
                        builder
                            .WithOrigins(allowedHosts)
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
            return services;
        }

        public static void UseCorsSettings(this IApplicationBuilder app)
        {
            app.UseCors("AllowedHosts");
        }
    }
}