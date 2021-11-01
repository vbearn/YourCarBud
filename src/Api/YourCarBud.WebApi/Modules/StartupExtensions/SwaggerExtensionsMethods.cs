using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace YourCarBud.WebApi.Modules.StartupExtensions
{
    internal static class SwaggerExtensionsMethods
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "YourCarBud", Version = "v 1" });
            });
            return services;
        }

        public static void UseSwaggerSettings(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("./swagger/v1/swagger.json", "YourCarBud.WebApi v1");
                c.DocumentTitle = "YourCarBud.WebApi";
                c.RoutePrefix = string.Empty;
            });
        }
    }
}