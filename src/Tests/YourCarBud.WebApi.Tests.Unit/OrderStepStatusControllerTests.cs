using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using YourCarBud.WebApi.Modules.OrderModule.Entities;
using YourCarBud.WebApi.Modules.OrderStepModule.Controllers;
using YourCarBud.WebApi.Modules.OrderStepModule.Dtos;
using YourCarBud.WebApi.Modules.OrderStepModule.Models;
using YourCarBud.WebApi.Modules.OrderStepModule.Services.Abstractions;
using YourCarBud.WebApi.Modules.OrderStepModule.Services.OrderStepBehaviours;

namespace YourCarBud.WebApi.Tests.Unit
{
    public class OrderStepStatusControllerTests
    {
        [Theory]
        [InlineData(ContactDetailWorkflowBehaviour._OrderStepName, "Start", Statuses.Start)]
        [InlineData(ContactDetailWorkflowBehaviour._OrderStepName, "Success", Statuses.Success)]
        [InlineData(ContactDetailWorkflowBehaviour._OrderStepName, "Fail", Statuses.Fail)]
        public async Task OrderStepStatusController_ContactDetailStep_ShouldBeUpdatedCorrectly(string stepName,
            string statusString,
            Statuses status)
        {
            // arrange

            var updateDtoMock = new OrderStepStatusUpdateDto
            {
                Status = statusString
            };
            var updateModelMock = new OrderStepStatusUpdateModel
            {
                Status = status
            };

            var contactDetailWorkflowBehaviourMock = Faker.CreateContactDetailWorkflowBehaviour();

            var orderStepWorkflowBehaviourFactoryStub =
                Faker.CreateOrderStepWorkflowBehaviourFactoryMock(contactDetailWorkflowBehaviourMock);

            var orderStepServiceStub = new Mock<IOrderStepService>();

            var iMapperStub = new Mock<IMapper>();
            iMapperStub
                .Setup(item => item.Map(updateDtoMock,
                    It.IsAny<Action<IMappingOperationOptions<object, OrderStepStatusUpdateModel>>>()))
                .Returns(updateModelMock);


            var orderStepStatusController = new OrderStepStatusController(iMapperStub.Object,
                orderStepServiceStub.Object,
                orderStepWorkflowBehaviourFactoryStub.Object);

            // act

            var updateStepStatusResult = await orderStepStatusController.UpdateStepStatus(It.IsAny<Guid>(), stepName,
                updateDtoMock,
                It.IsAny<CancellationToken>());

            // assert

            Assert.IsType<OkResult>(updateStepStatusResult);

            orderStepServiceStub.Verify(
                x => x.UpdateStatus(contactDetailWorkflowBehaviourMock, It.IsAny<Guid>(), updateModelMock,
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }
    }
}