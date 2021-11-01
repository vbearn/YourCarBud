using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YourCarBud.Common;
using YourCarBud.WebApi.Modules.StartupExtensions;

namespace YourCarBud.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddOptions()
                .AddDatabase(Configuration)
                .AddSwagger()
                .AddAutoMapper(typeof(Startup))
                .AddCorsSettings(Configuration)
                .AddFluentValidationSettings()
                .AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwaggerSettings();

            app.UseHttpsRedirection();

            app.UseCorsSettings();
            app.UseMiddleware<HttpStatusExceptionMiddleware>();

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}