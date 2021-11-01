using System;
using System.ComponentModel.DataAnnotations;

namespace YourCarBud.WebApi.Modules.OrderStepModule.Models
{
    public class DeliveryAppointmentCreationArgsDto : IOrderDetailCreationArgs
    {
        [Required] public DateTimeOffset AppointmentDateTime { get; set; }
    }
}