using System;
using System.Threading.Tasks;
using YourCarBud.WebApi.Modules.OrderModule.Entities;
using YourCarBud.WebApi.Modules.OrderStepModule.Models;

namespace YourCarBud.WebApi.Modules.OrderStepModule.Services.Abstractions
{
    public interface IWorkflowBehaviour
    {
        public void ValidateStatusUpdate(Order order, Statuses statusToBeUpdated);
      
        /// <summary>
        /// synchronizes the state of all dependent entities after a status update
        /// </summary>
        public Task AfterStatusUpdate(Guid orderId, OrderStepStatusUpdateModel updateModel);
    }
}