using DryIoc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using YourCarBud.Common;
using YourCarBud.WebApi.Modules.OrderModule.Entities;
using YourCarBud.WebApi.Modules.OrderStepModule.Entities;
using YourCarBud.WebApi.Modules.OrderStepModule.Services;
using YourCarBud.WebApi.Modules.OrderStepModule.Services.Abstractions;
using YourCarBud.WebApi.Modules.OrderStepModule.Services.OrderStepBehaviours;

namespace YourCarBud.WebApi.Modules.OrderStepModule
{
    public class OrderStepModuleConfiguration : IDependencyInjectionModule
    {
        public void OnModelCreate(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(e =>
            {
                e.HasOne(x => x.ContactDetail)
                    .WithOne(x => x.Order)
                    .HasForeignKey<ContactDetail>(x => x.OrderId);

                e.HasOne(x => x.DeliveryAppointment)
                    .WithOne(x => x.Order)
                    .HasForeignKey<DeliveryAppointment>(x => x.OrderId);

                e.HasOne(x => x.Payment)
                    .WithOne(x => x.Order)
                    .HasForeignKey<Payment>(x => x.OrderId);
            });
        }

        public void ConfigureServices(IRegistrator service, IResolver resolver, IConfiguration configuration)
        {
            service.Register<IOrderStepWorkflowBehaviour, ContactDetailWorkflowBehaviour>(Reuse.Scoped);
            service.Register<IOrderStepWorkflowBehaviour, ProcessPaymentWorkflowBehaviour>(Reuse.Scoped);
            service.Register<IOrderStepWorkflowBehaviour, ProcessDeliveryAppointmentWorkflowBehaviour>(Reuse.Scoped);


            service.Register<IOrderStepWorkflowBehaviourFactory, OrderStepBehaviourFactory>(Reuse.Scoped);
            service.Register<IOrderStepService, OrderStepService>(Reuse.Scoped);
        }
    }
}