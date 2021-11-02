using System;
using AutoMapper;
using Moq;
using YourCarBud.Common;
using YourCarBud.WebApi.Modules.OrderModule.Entities;
using YourCarBud.WebApi.Modules.OrderModule.Services.Abstractions;
using YourCarBud.WebApi.Modules.OrderStepModule.Entities;
using YourCarBud.WebApi.Modules.OrderStepModule.Services.Abstractions;
using YourCarBud.WebApi.Modules.OrderStepModule.Services.OrderStepBehaviours;

namespace YourCarBud.WebApi.Tests.Unit
{
    public static class Faker
    {
        public static Guid FakeGuid => Guid.Empty;

        public static Order CreateOrder()
        {
            var order = new Order
            {
                Id = FakeGuid,
                CreatedAt = DateTime.Now,
                TotalAmount = 100,
                Status = Statuses.Pending,
            };
            order.OrderSteps = new[]
            {
                new OrderStep
                {
                    Order = order,
                    Status = Statuses.Pending,
                    StepName = ContactDetailWorkflowBehaviour._OrderStepName,
                },
            };

            return order;
        }

        public static ContactDetailWorkflowBehaviour CreateContactDetailWorkflowBehaviour()
        {
            return new ContactDetailWorkflowBehaviour(
                It.IsAny<IMapper>(),
                It.IsAny<IOrderService>(),
                It.IsAny<IOrderStepService>(),
                It.IsAny<IRepository<ContactDetail>>()
            );
        }

        public static Mock<IOrderStepWorkflowBehaviourFactory> CreateOrderStepWorkflowBehaviourFactoryMock(
            IOrderStepWorkflowBehaviour orderStepWorkflowBehaviourFake)
        {
            var orderStepWorkflowBehaviourFactoryFake = new Mock<IOrderStepWorkflowBehaviourFactory>();

            orderStepWorkflowBehaviourFactoryFake
                .Setup(item => item.GetAllWorkflowBehaviours())
                .Returns(new[] { orderStepWorkflowBehaviourFake });

            orderStepWorkflowBehaviourFactoryFake
                .Setup(item => item.ResolveBehaviourViaStepName(orderStepWorkflowBehaviourFake.OrderStepName))
                .Returns(orderStepWorkflowBehaviourFake);

            return orderStepWorkflowBehaviourFactoryFake;
        }
    }
}