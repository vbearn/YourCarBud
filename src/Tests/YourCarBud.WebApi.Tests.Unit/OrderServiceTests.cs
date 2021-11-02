using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using YourCarBud.Common;
using YourCarBud.WebApi.Modules.OrderModule.Entities;
using YourCarBud.WebApi.Modules.OrderModule.Services;
using YourCarBud.WebApi.Modules.OrderStepModule.Services.Abstractions;

namespace YourCarBud.WebApi.Tests.Unit
{
    public class OrderServiceTests
    {
        [Fact]
        public async Task OrderService_CreateOrder_ShouldAddToRepositoryWithDefaultValues()
        {
            // arrange

            var orderMock = Faker.CreateOrder();

            var contactDetailWorkflowBehaviourMock = Faker.CreateContactDetailWorkflowBehaviour();

            var orderStepWorkflowBehaviourFactoryStub =
                Faker.CreateOrderStepWorkflowBehaviourFactoryMock(contactDetailWorkflowBehaviourMock);

            var orderRepositoryStub = new Mock<IRepository<Order>>();
            orderRepositoryStub.Setup(item => item.AddAsync(It.IsAny<Order>(),
                    It.IsAny<CancellationToken>(), true))
                .Returns(Task.CompletedTask);


            var orderService = new OrderService(
                orderRepositoryStub.Object,
                new Lazy<IOrderStepWorkflowBehaviourFactory>(() => orderStepWorkflowBehaviourFactoryStub.Object));

            // act

            await orderService.CreateOrder(orderMock,
                It.IsAny<CancellationToken>());

            // assert

            orderMock.Status.Should().Be(Statuses.Pending);
            orderMock.CreatedAt.Should().NotBe(DateTimeOffset.MinValue);
            orderMock.OrderSteps.Should().HaveCountGreaterThan(0);

            orderRepositoryStub.Verify(
                x => x.AddAsync(orderMock, It.IsAny<CancellationToken>(), true),
                Times.Once());
        }


        [Fact]
        public void OrderService_GetOrderWithChildren_ShouldReturnCorrectOrder()
        {
            // arrange

            var orderMock = Faker.CreateOrder();

            var orderRepositoryStub = new Mock<IRepository<Order>>();
            orderRepositoryStub.SetupGet(item => item.Table)
                .Returns(new[] { orderMock }.AsQueryable());

            var orderService = new OrderService(
                orderRepositoryStub.Object,
                It.IsAny<Lazy<IOrderStepWorkflowBehaviourFactory>>());

            // act

            var order = orderService.GetOrderWithChildrenQuery(orderMock.Id).Single();

            // assert

            order.Id.Should().Be(orderMock.Id);
        }
    }
}