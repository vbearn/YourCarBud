using DryIoc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using YourCarBud.Common;
using YourCarBud.WebApi.Modules.OrderModule.Entities;
using YourCarBud.WebApi.Modules.OrderModule.Services;
using YourCarBud.WebApi.Modules.OrderModule.Services.Abstractions;

namespace YourCarBud.WebApi.Modules.OrderModule
{
    public class OrderModuleConfiguration : IDependencyInjectionModule
    {
        public void OnModelCreate(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(e => { });
            modelBuilder.Entity<OrderStep>(e => { });
        }

        public void ConfigureServices(IRegistrator service, IResolver resolver, IConfiguration configuration)
        {
            service.Register<IOrderService, OrderService>(Reuse.Scoped);
            service.Register<IOrderWorkflowBehaviour, OrderWorkflowBehaviour>(Reuse.Scoped);
        }
    }
}