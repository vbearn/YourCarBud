using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YourCarBud.WebApi.Modules.OrderModule.Entities;
using YourCarBud.WebApi.Modules.OrderStepModule.Services.OrderStepBehaviours;

namespace YourCarBud.WebApi.Modules.DbContextModule
{
    public class YourCarBudDbContextSeed
    {
        public async Task SeedAsync(DbContext context, IHostEnvironment env, IServiceProvider services,
            ILogger<YourCarBudDbContextSeed> logger, int retry = 0)
        {
            int retryForAvailability = retry;

            try
            {
                var hasAlreadyBeenSeeded = await context.Set<Order>().AnyAsync();
                if (!hasAlreadyBeenSeeded)
                {
                    var seedOrder = new Order
                    {
                        CreatedAt = DateTime.Now,
                        TotalAmount = 100,
                        Status = Statuses.Pending,
                    };
                    seedOrder.OrderSteps = new[]
                    {
                        new OrderStep
                        {
                            Order = seedOrder,
                            Status = Statuses.Pending,
                            StepName = ContactDetailWorkflowBehaviour._OrderStepName,
                        },
                        new OrderStep
                        {
                            Order = seedOrder,
                            Status = Statuses.Pending,
                            StepName = ProcessPaymentWorkflowBehaviour._OrderStepName,
                        },
                        new OrderStep
                        {
                            Order = seedOrder,
                            Status = Statuses.Pending,
                            StepName = ProcessDeliveryAppointmentWorkflowBehaviour._OrderStepName,
                        }
                    };

                    await context.AddAsync(seedOrder);

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                if (retryForAvailability < 10)
                {
                    retryForAvailability++;

                    logger.LogError(ex, "EXCEPTION ERROR while migrating {DbContextName}",
                        nameof(YourCarBudDbContextSeed));

                    await SeedAsync(context, env, services, logger, retryForAvailability);
                }
            }
        }
    }
}