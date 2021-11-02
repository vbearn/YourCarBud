using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YourCarBud.Common;
using YourCarBud.WebApi.Modules.OrderModule.Entities;
using YourCarBud.WebApi.Modules.OrderModule.Services.Abstractions;
using YourCarBud.WebApi.Modules.OrderStepModule.Services.Abstractions;

namespace YourCarBud.WebApi.Modules.OrderModule.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly Lazy<IOrderStepWorkflowBehaviourFactory> _orderStepBehaviourFactory;

        public OrderService(
            IRepository<Order> orderRepository,
            Lazy<IOrderStepWorkflowBehaviourFactory> orderStepBehaviourFactory
        )
        {
            _orderRepository = orderRepository;
            _orderStepBehaviourFactory = orderStepBehaviourFactory;
        }

        public IQueryable<Order> GetAllOrdersWithChildren()
        {
            return _orderRepository.Table.Include(x => x.OrderSteps);
        }

        public Task<Order> GetOrderWithChildren(Guid id)
        {
            return GetOrderWithChildrenQuery(id).SingleOrDefaultAsync();
        }

        public IQueryable<Order> GetOrderWithChildrenQuery(Guid id)
        {
            return GetAllOrdersWithChildren().Where(x => x.Id == id);
        }

        private List<OrderStep> GetDefaultOrderSteps(Order order)
        {
            return _orderStepBehaviourFactory.Value.GetAllWorkflowBehaviours().Select(behaviour => new OrderStep
            {
                Order = order,
                Status = Statuses.Pending,
                StepName = behaviour.OrderStepName
            }).ToList();
        }

        public async Task CreateOrder(Order order, CancellationToken cancellationToken)
        {
            order.OrderSteps = GetDefaultOrderSteps(order);
            order.CreatedAt = DateTimeOffset.Now;
            order.Status = Statuses.Pending;

            await _orderRepository.AddAsync(order, cancellationToken);
        }

        public async Task UpdateOrder(Order order)
        {
            await _orderRepository.UpdateAsync(order);
        }
    }
}