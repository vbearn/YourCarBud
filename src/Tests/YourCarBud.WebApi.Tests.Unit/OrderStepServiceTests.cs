using System.Linq;
using Moq;
using Xunit;
using YourCarBud.Common;
using YourCarBud.WebApi.Modules.OrderModule.Entities;
using YourCarBud.WebApi.Modules.OrderModule.Services;
using YourCarBud.WebApi.Modules.OrderModule.Services.Abstractions;
using YourCarBud.WebApi.Modules.OrderStepModule.Services;

namespace YourCarBud.WebApi.Tests.Unit
{
    public class OrderStepServiceTests
    {
        [Fact]
        public void ValidateStatusUpdate_JumpFromPendingToSuccess_ShouldThrowError()
        {
            // arrange

            var orderMock = Faker.CreateOrder();

            var orderStepService = new OrderStepService(
                It.IsAny<OrderService>(),
                It.IsAny<IOrderWorkflowBehaviour>(),
                It.IsAny<IRepository<OrderStep>>()
            );

            // act, assert

            Assert.Throws<ConflictingOperationException>(
                () => orderStepService.ValidateStatusUpdate(orderMock.OrderSteps.First(),
                    Statuses.Success));
        }

        [Theory]
        [InlineData(Statuses.Success)]
        [InlineData(Statuses.Fail)]
        public void ValidateStatusUpdate_UpdateStepWhichIsAlreadySuccessOrFail_ShouldThrowError(
            Statuses currentStepStatus)
        {
            // arrange

            var orderMock = Faker.CreateOrder();
            orderMock.OrderSteps.First().Status = currentStepStatus;

            var orderStepService = new OrderStepService(
                It.IsAny<OrderService>(),
                It.IsAny<IOrderWorkflowBehaviour>(),
                It.IsAny<IRepository<OrderStep>>()
            );

            // act, assert

            Assert.Throws<ConflictingOperationException>(
                () => orderStepService.ValidateStatusUpdate(orderMock.OrderSteps.First(),
                    It.IsAny<Statuses>()));
        }
    }
}