using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YourCarBud.Common;
using YourCarBud.WebApi.Modules.OrderModule.Entities;
using YourCarBud.WebApi.Modules.OrderModule.Services.Abstractions;
using YourCarBud.WebApi.Modules.OrderStepModule.Models;
using YourCarBud.WebApi.Modules.OrderStepModule.Services.Abstractions;

namespace YourCarBud.WebApi.Modules.OrderStepModule.Services
{
    public class OrderStepService : IOrderStepService
    {
        private readonly IOrderService _orderService;
        private readonly IRepository<OrderStep> _orderStepRepository;
        private readonly IOrderWorkflowBehaviour _orderWorkflowBehaviour;

        public OrderStepService(
            IOrderService orderService,
            IOrderWorkflowBehaviour orderWorkflowBehaviour,
            IRepository<OrderStep> orderStepRepository
        )
        {
            _orderService = orderService;
            _orderWorkflowBehaviour = orderWorkflowBehaviour;
            _orderStepRepository = orderStepRepository;
        }

        public OrderStep GetOrderStep(Order order, string stepName)
        {
            if (order.OrderSteps == null)
            {
                throw new ArgumentNullException(nameof(order.OrderSteps));
            }

            var orderStep = order.OrderSteps.ToList().SingleOrDefault(x => x.StepName == stepName);
            return orderStep;
        }

        public async Task Update(OrderStep orderStep, CancellationToken cancellationToken)
        {
            await _orderStepRepository.UpdateAsync(orderStep, cancellationToken);
        }

        public void ValidateStatusUpdate(OrderStep orderStep, Statuses statusToBeUpdatedInto)
        {
            if ((int)statusToBeUpdatedInto < (int)orderStep.Status)
            {
                throw new ConflictingOperationException("OrderStep can not be demoted.");
            }

            if (orderStep.Status is Statuses.Success or Statuses.Fail)
            {
                throw new ConflictingOperationException(
                    "OrderStep is already Succeeded or Failed and can not be updated anymore.");
            }

            if (orderStep.Status == Statuses.Pending &&
                statusToBeUpdatedInto is Statuses.Success or Statuses.Fail)
            {
                throw new ConflictingOperationException(
                    "OrderStep can not jump from Pending to Success or Fail. It needs to be Started first.");
            }
        }

        public async Task UpdateStatus(IOrderStepWorkflowBehaviour orderStepWorkflowBehaviour, Guid orderId,
            OrderStepStatusUpdateModel updateModel, CancellationToken cancellationToken)
        {
            var order = await _orderService.GetOrderWithChildren(orderId);
            if (order == null)
            {
                throw new NotFoundException("Order not found.");
            }

            // validate that both Order's and OrderStep's status can be updated
            _orderWorkflowBehaviour.ValidateStatusUpdate(order, updateModel.Status);
            orderStepWorkflowBehaviour.ValidateStatusUpdate(order, updateModel.Status);


            // updating order step status
            var orderStep = GetOrderStep(order, orderStepWorkflowBehaviour.OrderStepName);
            orderStep.Status = updateModel.Status;

            // a pipeline for Updating the OrderStep Status -> PostProcessing OrderStep -> PostProcessing Order
            // taking care of all the steps of workflow
            await ExecuteWorkflowPipelineForStatusUpdate(orderStepWorkflowBehaviour, orderId, updateModel, cancellationToken, orderStep);
        }

        private async Task ExecuteWorkflowPipelineForStatusUpdate(IOrderStepWorkflowBehaviour orderStepWorkflowBehaviour,
            Guid orderId, OrderStepStatusUpdateModel updateModel, CancellationToken cancellationToken, OrderStep orderStep)
        {
            using (var context = await _orderStepRepository.BeginTransactionAsync())
            {
                await Update(orderStep, cancellationToken);

                // do actions after status update, such as marking the whole order as success or fail, marking completedAt,
                // creating Payment or Delivery records, and ...
                await orderStepWorkflowBehaviour.AfterStatusUpdate(orderId, updateModel);
                await _orderWorkflowBehaviour.AfterStatusUpdate(orderId, updateModel);

                await context.CommitAsync(cancellationToken);
            }
        }
    }
}