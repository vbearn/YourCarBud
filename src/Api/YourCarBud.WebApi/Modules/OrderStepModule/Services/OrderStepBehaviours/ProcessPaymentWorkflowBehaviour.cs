using System;
using System.Threading.Tasks;
using AutoMapper;
using YourCarBud.Common;
using YourCarBud.WebApi.Modules.OrderModule.Entities;
using YourCarBud.WebApi.Modules.OrderModule.Services.Abstractions;
using YourCarBud.WebApi.Modules.OrderStepModule.Entities;
using YourCarBud.WebApi.Modules.OrderStepModule.Models;
using YourCarBud.WebApi.Modules.OrderStepModule.Services.Abstractions;

namespace YourCarBud.WebApi.Modules.OrderStepModule.Services.OrderStepBehaviours
{
    public class ProcessPaymentWorkflowBehaviour : IOrderStepWorkflowBehaviour
    {
        public const string _OrderStepName = "ProcessPayment";
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;
        private readonly IOrderStepService _orderStepService;
        private readonly IRepository<Payment> _paymentRepository;

        public ProcessPaymentWorkflowBehaviour(
            IMapper mapper,
            IOrderService orderService,
            IOrderStepService orderStepService,
            IRepository<Payment> paymentRepository
        )
        {
            _mapper = mapper;
            _orderService = orderService;
            _orderStepService = orderStepService;
            _paymentRepository = paymentRepository;
        }

        public string OrderStepName => _OrderStepName;
        public Type DetailCreationArgsDtoType => typeof(PaymentCreationArgsDto);

        public void ValidateBeforeStatusUpdate(Order order, Statuses statusToBeUpdatedInto)
        {
            var orderStep = _orderStepService.GetOrderStep(order, OrderStepName);

            _orderStepService.ValidateOrderStepStatusUpdate(orderStep, statusToBeUpdatedInto);

            var contactDetailStep =
                _orderStepService.GetOrderStep(order, ContactDetailWorkflowBehaviour._OrderStepName);
            var checkIfPrerequisiteStepIsDone = contactDetailStep.Status == Statuses.Success;
            if (!checkIfPrerequisiteStepIsDone)
            {
                throw new ConflictingOperationException(
                    $"{_OrderStepName} step can not be started before {ContactDetailWorkflowBehaviour._OrderStepName} succeeded.");
            }
        }


        public async Task DoActionsAfterStatusUpdate(Guid orderId, OrderStepStatusUpdateModel updateModel)
        {
            var order = await _orderService.GetOrderWithChildren(orderId);


            // if step is success, create the Payment Entity and attach it to the order
            if (updateModel.Status == Statuses.Success)
            {
                if (updateModel.DetailCreationArgs.GetType() != DetailCreationArgsDtoType)
                {
                    throw new ArgumentException(
                        "Payment data is not provided correctly.");
                }

                var paymentEntity = _mapper.Map<Payment>(updateModel.DetailCreationArgs);
                paymentEntity.Order = order;
                await _paymentRepository.AddAsync(paymentEntity);


                order.TotalAmount = paymentEntity.Amount;
                order.Payment = paymentEntity;
                await _orderService.UpdateOrder(order);
            }
        }
    }
}