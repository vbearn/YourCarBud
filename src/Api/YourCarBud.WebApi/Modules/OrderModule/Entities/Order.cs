using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using YourCarBud.WebApi.Modules.OrderStepModule.Entities;

namespace YourCarBud.WebApi.Modules.OrderModule.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset CompletedAt { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        public string ErrorMessage { get; set; }
        public Statuses Status { get; set; }

        public virtual ICollection<OrderStep> OrderSteps { get; set; }

        public Guid? PaymentId { get; set; }
        public Payment Payment { get; set; }

        public Guid? ContactDetailId { get; set; }
        public ContactDetail ContactDetail { get; set; }

        public Guid? DeliveryAppointmentId { get; set; }
        public DeliveryAppointment DeliveryAppointment { get; set; }
    }

    public enum Statuses
    {
        Pending = 0,
        Start = 1,
        Success = 2,
        Fail = 3,
    }
}