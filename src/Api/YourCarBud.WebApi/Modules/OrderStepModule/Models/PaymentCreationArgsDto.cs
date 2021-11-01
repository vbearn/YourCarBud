using System.ComponentModel.DataAnnotations;

namespace YourCarBud.WebApi.Modules.OrderStepModule.Models
{
    public class PaymentCreationArgsDto : IOrderDetailCreationArgs
    {
        [Required] public decimal Amount { get; set; }
    }
}