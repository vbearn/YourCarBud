
using System;

namespace YourCarBud.WebApi.Modules.OrderModule.Entities
{
    public class OrderStep
    {
        public Guid Id { get; set; }
        public string StepName { get; set; }
        public Statuses Status { get; set; }
     
        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; }

    }
}