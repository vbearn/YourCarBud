
using System;
using YourCarBud.WebApi.Modules.OrderModule.Entities;

namespace YourCarBud.WebApi.Modules.OrderStepModule.Entities
{
    public class ContactDetail : IOrderStepDetail
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; }
        public string Email { get; set; }

    }
}