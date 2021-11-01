using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace YourCarBud.WebApi.Modules.StartupExtensions
{
    internal static class FluentValidationExtensionsMethods
    {
        public static IServiceCollection AddFluentValidationSettings(this IServiceCollection services)
        {
            services.AddFluentValidation(fv =>
            {
                fv.ImplicitlyValidateChildProperties = true;
                fv.RegisterValidatorsFromAssemblyContaining<Startup>();
            });
            return services;
        }
    }
}