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
    public class ProcessDeliveryAppointmentWorkflowBehaviour : IOrderStepWorkflowBehaviour
    {
        public const string _OrderStepName = "ProcessDeliveryAppointment";
        private readonly IRepository<DeliveryAppointment> _deliveryRepository;
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;
        private readonly IOrderStepService _orderStepService;

        public ProcessDeliveryAppointmentWorkflowBehaviour(
            IMapper mapper,
            IOrderService orderService,
            IOrderStepService orderStepService,
            IRepository<DeliveryAppointment> deliveryRepository
        )
        {
            _mapper = mapper;
            _orderService = orderService;
            _orderStepService = orderStepService;
            _deliveryRepository = deliveryRepository;
        }

        public string OrderStepName => _OrderStepName;
        public Type DetailCreationArgsDtoType => typeof(DeliveryAppointmentCreationArgsDto);

        public void ValidateStatusUpdate(Order order, Statuses statusToBeUpdatedInto)
        {
            var orderStep = _orderStepService.GetOrderStep(order, OrderStepName);

            _orderStepService.ValidateStatusUpdate(orderStep, statusToBeUpdatedInto);

            var contactDetailStep =
                _orderStepService.GetOrderStep(order, ContactDetailWorkflowBehaviour._OrderStepName);

            var checkIfPrerequisiteStepIsDone = contactDetailStep.Status == Statuses.Success;
            if (!checkIfPrerequisiteStepIsDone)
            {
                throw new ConflictingOperationException(
                    $"{_OrderStepName} step can not be started before {ContactDetailWorkflowBehaviour._OrderStepName}'s success.");
            }
        }


        public async Task AfterStatusUpdate(Guid orderId, OrderStepStatusUpdateModel updateModel)
        {
            var order = await _orderService.GetOrderWithChildren(orderId);


            // if step is success, create the DeliveryAppointment Entity and attach it to the order
            if (updateModel.Status == Statuses.Success)
            {
                if (updateModel.DetailCreationArgs.GetType() != DetailCreationArgsDtoType)
                {
                    throw new ArgumentException(
                        "DeliveryAppointmentCreationArgsDto is not provided correctly.");
                }

                var paymentEntity = _mapper.Map<DeliveryAppointment>(updateModel.DetailCreationArgs);
                paymentEntity.Order = order;
                await _deliveryRepository.AddAsync(paymentEntity);


                order.DeliveryAppointment = paymentEntity;
                await _orderService.UpdateOrder(order);
            }
        }
    }
}