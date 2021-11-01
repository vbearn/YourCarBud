using System;

namespace YourCarBud.WebApi.Modules.OrderModule.Entities
{
    public interface IOrderStepDetail
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Order Order { get; set; }
    }
}