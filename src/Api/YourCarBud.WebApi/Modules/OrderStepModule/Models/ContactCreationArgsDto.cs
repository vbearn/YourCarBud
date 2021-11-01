using System.ComponentModel.DataAnnotations;

namespace YourCarBud.WebApi.Modules.OrderStepModule.Models
{
    public class ContactCreationArgsDto : IOrderDetailCreationArgs
    {
        [Required] public string Email { get; set; }
    }
}