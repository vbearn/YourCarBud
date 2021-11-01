using DryIoc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace YourCarBud.Common
{
    public interface IDependencyInjectionModule
    {
        void ConfigureServices(IRegistrator service, IResolver resolver, IConfiguration configuration);
        void OnModelCreate(ModelBuilder modelBuilder);

    }
}