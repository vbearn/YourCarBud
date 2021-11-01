using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YourCarBud.Common;
using YourCarBud.Common.BootstrapModule;
using YourCarBud.WebApi.Modules.DbContextModule;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;

namespace YourCarBud.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SelfLog.Enable(Console.Error);
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            try
            {
                Log.Information("Starting web host");

                Log.Information("Configuring web host...");
                var host = CreateHostBuilder(args).Build();

                Log.Information("Applying migrations...");
                host.MigrateDbContext<DbContext>((context, services) =>
                {
                    var env = services.GetService<IHostEnvironment>();
                    var logger = services.GetService<ILogger<YourCarBudDbContextSeed>>();

                    new YourCarBudDbContextSeed()
                        .SeedAsync(context, env, services, logger)
                        .Wait();
                });


                Log.Information("Starting web host...");
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new DryIocServiceProviderFactory<DryIoCBootstrap>())
                .UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                )
                .ConfigureWebHostDefaults(webHostBuilder =>
                {
                    webHostBuilder
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .UseIISIntegration()
                        .ConfigureKestrel((context, options) => { })
                        .UseStartup<Startup>();
                });
        }
    }
}