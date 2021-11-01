using DryIoc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using YourCarBud.Common;

namespace YourCarBud.WebApi.Modules.DbContextModule
{
    public class DbContextModuleConfiguration : IDependencyInjectionModule
    {
        public void OnModelCreate(ModelBuilder modelBuilder)
        {
        }

        public void ConfigureServices(IRegistrator service, IResolver resolver, IConfiguration configuration)
        {
            service.Register(typeof(IRepository<>), typeof(Repository<>), Reuse.Scoped);
            service.Register<DbContext, YourCarBudDbContext>(Reuse.Scoped);
        }
    }
}