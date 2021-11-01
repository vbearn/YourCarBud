
using System;
using YourCarBud.WebApi.Modules.OrderModule.Entities;

namespace YourCarBud.WebApi.Modules.OrderStepModule.Entities
{
    public class Payment : IOrderStepDetail
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; }
        public decimal Amount { get; set; }

    }
}