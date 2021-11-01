using System;
using System.Threading.Tasks;
using YourCarBud.WebApi.Modules.OrderModule.Entities;
using YourCarBud.WebApi.Modules.OrderStepModule.Models;

namespace YourCarBud.WebApi.Modules.OrderStepModule.Services.Abstractions
{
    public interface IWorkflowBehaviour
    {
        public void ValidateBeforeStatusUpdate(Order order, Statuses statusToBeUpdated);
        public Task DoActionsAfterStatusUpdate(Guid orderId, OrderStepStatusUpdateModel updateModel);
    }
}