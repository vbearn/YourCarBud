using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YourCarBud.WebApi.Modules.DbContextModule;

namespace YourCarBud.WebApi.Modules.StartupExtensions
{
    internal static class DatabaseExtensionsMethods
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DbContext, YourCarBudDbContext>(options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                        x =>
                        {
                            var dbAssemblyName = typeof(YourCarBudDbContext).Assembly.GetName().Name;
                            x.MigrationsAssembly(dbAssemblyName);
                            x.CommandTimeout((int)TimeSpan.FromMinutes(3).TotalSeconds);
                        });
                }
            );

            return services;
        }
    }
}