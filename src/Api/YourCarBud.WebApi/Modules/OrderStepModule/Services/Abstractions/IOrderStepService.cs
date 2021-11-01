using System;
using System.Threading;
using System.Threading.Tasks;
using YourCarBud.WebApi.Modules.OrderModule.Entities;
using YourCarBud.WebApi.Modules.OrderStepModule.Models;

namespace YourCarBud.WebApi.Modules.OrderStepModule.Services.Abstractions
{
    public interface IOrderStepService
    {
        OrderStep GetOrderStep(Order order, string stepName);
        Task Update(OrderStep orderStep, CancellationToken cancellationToken);
        void ValidateOrderStepStatusUpdate(OrderStep orderStep, Statuses statusToBeUpdatedInto);

        Task UpdateStepStatus(IOrderStepWorkflowBehaviour orderStepWorkflowBehaviour, Guid orderId,
            OrderStepStatusUpdateModel updateModel, CancellationToken cancellationToken);
    }
}