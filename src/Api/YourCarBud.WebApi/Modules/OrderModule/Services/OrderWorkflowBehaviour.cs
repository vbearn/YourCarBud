using System;
using System.Linq;
using System.Threading.Tasks;
using YourCarBud.Common;
using YourCarBud.WebApi.Modules.OrderModule.Entities;
using YourCarBud.WebApi.Modules.OrderModule.Services.Abstractions;
using YourCarBud.WebApi.Modules.OrderStepModule.Models;

namespace YourCarBud.WebApi.Modules.OrderModule.Services
{
    public class OrderWorkflowBehaviour : IOrderWorkflowBehaviour
    {
        private readonly IOrderService _orderService;

        public OrderWorkflowBehaviour(
            IOrderService orderService
        )
        {
            _orderService = orderService;
        }


        public void ValidateBeforeStatusUpdate(Order order, Statuses statusToBeUpdatedInto)
        {
            if (order.Status is Statuses.Success or Statuses.Fail)
            {
                throw new ConflictingOperationException("Order is already Succeeded or Failed and can not be updated");
            }

        }

        public async Task DoActionsAfterStatusUpdate(Guid orderId, OrderStepStatusUpdateModel updateModel)
        {
            var order = await _orderService.GetOrderWithChildren(orderId);
            var orderSteps = order.OrderSteps.ToList();


            // if step is started, mark the whole order as started
            if (updateModel.Status == Statuses.Start && order.Status == Statuses.Pending)
            {
                order.Status = Statuses.Start;
                await _orderService.UpdateOrder(order);
            }

            // if any steps are failed, mark the whole order as fail and fill error message
            var isAnyStepFailed = orderSteps.Any(x => x.Status == Statuses.Fail);
            if (isAnyStepFailed)
            {
                order.ErrorMessage = updateModel.Message;
                order.Status = Statuses.Fail;
                await _orderService.UpdateOrder(order);
            }

            // if all steps are succeeded, mark the whole order as success and fill the date
            var allAllStepsSucceeded = orderSteps.All(x => x.Status == Statuses.Success);
            if (allAllStepsSucceeded)
            {
                order.Status = Statuses.Success;
                order.CompletedAt = DateTimeOffset.Now;
                await _orderService.UpdateOrder(order);
            }
        }
    }
}