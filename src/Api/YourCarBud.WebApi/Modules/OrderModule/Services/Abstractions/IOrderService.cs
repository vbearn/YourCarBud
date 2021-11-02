using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YourCarBud.WebApi.Modules.OrderModule.Entities;

namespace YourCarBud.WebApi.Modules.OrderModule.Services.Abstractions
{
    public interface IOrderService
    {
        IQueryable<Order> GetAllOrdersWithChildren();
        Task<Order> GetOrderWithChildren(Guid id);
        IQueryable<Order> GetOrderWithChildrenQuery(Guid id);
        Task CreateOrder(Order order, CancellationToken cancellationToken);
        Task UpdateOrder(Order order);
    }
}