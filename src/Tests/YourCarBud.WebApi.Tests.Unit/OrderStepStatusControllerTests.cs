using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Xunit;
using YourCarBud.Common;
using YourCarBud.WebApi.Modules.OrderModule.Entities;
using YourCarBud.WebApi.Modules.OrderModule.Services.Abstractions;
using YourCarBud.WebApi.Modules.OrderStepModule.Controllers;
using YourCarBud.WebApi.Modules.OrderStepModule.Dtos;
using YourCarBud.WebApi.Modules.OrderStepModule.Entities;
using YourCarBud.WebApi.Modules.OrderStepModule.Models;
using YourCarBud.WebApi.Modules.OrderStepModule.Services.Abstractions;
using YourCarBud.WebApi.Modules.OrderStepModule.Services.OrderStepBehaviours;

namespace YourCarBud.WebApi.Tests.Unit
{
    public class OrderStepStatusControllerTests
    {
        [Theory]
        [InlineData("ContactDetails", "Start", Statuses.Start)]
        [InlineData("ContactDetails", "Success", Statuses.Success)]
        [InlineData("ContactDetails", "Fail", Statuses.Fail)]
        public async Task OrderService_NormalOrder_ShouldBeCreatedCorrectly(string stepName, string statusString,
            Statuses status)
        {
            var orderStepWorkflowBehaviourFactoryStub = new Mock<IOrderStepWorkflowBehaviourFactory>();
            var orderStepServiceStub = new Mock<IOrderStepService>();
            var iMapperStub = new Mock<IMapper>();

            var contactDetailWorkflowBehaviourMock = new ContactDetailWorkflowBehaviour(
                It.IsAny<IMapper>(),
                It.IsAny<IOrderService>(),
                It.IsAny<IOrderStepService>(),
                It.IsAny<IRepository<ContactDetail>>()
            );

            orderStepWorkflowBehaviourFactoryStub
                .Setup(item => item.ResolveBehaviourViaStepName(stepName))
                .Returns(contactDetailWorkflowBehaviourMock);


            var updateDtoMock = new OrderStepStatusUpdateDto
            {
                Status = statusString
            };
            var updateModelMock = new OrderStepStatusUpdateModel
            {
                Status = status
            };

            iMapperStub
                .Setup(item => item.Map<OrderStepStatusUpdateModel>(updateDtoMock, default))
                .Returns(updateModelMock);


            var orderStepStatusController = new OrderStepStatusController(iMapperStub.Object,
                orderStepServiceStub.Object,
                orderStepWorkflowBehaviourFactoryStub.Object);

            await orderStepStatusController.UpdateStepStatus(It.IsAny<Guid>(), stepName, updateDtoMock,
                It.IsAny<CancellationToken>());

            orderStepServiceStub.Verify(
                x => x.UpdateStatus(contactDetailWorkflowBehaviourMock, It.IsAny<Guid>(), updateModelMock,
                    It.IsAny<CancellationToken>()),
                Times.Once());

        }

    }
}