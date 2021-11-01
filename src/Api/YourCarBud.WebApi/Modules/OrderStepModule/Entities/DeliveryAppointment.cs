using System;
using YourCarBud.WebApi.Modules.OrderModule.Entities;

namespace YourCarBud.WebApi.Modules.OrderStepModule.Entities
{
    public class DeliveryAppointment : IOrderStepDetail
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; }
        public DateTimeOffset AppointmentDateTime { get; set; }

    }
}