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
    public class ContactDetailWorkflowBehaviour : IOrderStepWorkflowBehaviour
    {
        public const string _OrderStepName = "ContactDetails";
        private readonly IRepository<ContactDetail> _contactRepository;
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;
        private readonly IOrderStepService _orderStepService;

        public ContactDetailWorkflowBehaviour(
            IMapper mapper,
            IOrderService orderService,
            IOrderStepService orderStepService,
            IRepository<ContactDetail> contactRepository
        )
        {
            _mapper = mapper;
            _orderService = orderService;
            _orderStepService = orderStepService;
            _contactRepository = contactRepository;
        }

        public Type DetailCreationArgsDtoType => typeof(ContactCreationArgsDto);
        public string OrderStepName => _OrderStepName;

        public void ValidateStatusUpdate(Order order, Statuses statusToBeUpdatedInto)
        {
            var orderStep = _orderStepService.GetOrderStep(order, OrderStepName);

            _orderStepService.ValidateStatusUpdate(orderStep, statusToBeUpdatedInto);
        }

        public async Task AfterStatusUpdate(Guid orderId, OrderStepStatusUpdateModel updateModel)
        {
            var order = await _orderService.GetOrderWithChildren(orderId);


            // if step is success, create the ContactDetail Entity and attach it to the order
            if (updateModel.Status == Statuses.Success)
            {
                if (updateModel.DetailCreationArgs.GetType() != DetailCreationArgsDtoType)
                {
                    throw new ArgumentException(
                        "DeliveryAppointment data is not provided correctly.");
                }

                // TODO: validate Email field
                var contactEntity = _mapper.Map<ContactDetail>(updateModel.DetailCreationArgs);
                contactEntity.Order = order;
                await _contactRepository.AddAsync(contactEntity);


                order.ContactDetail = contactEntity;
                await _orderService.UpdateOrder(order);
            }
        }
    }
}