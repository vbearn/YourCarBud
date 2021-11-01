using YourCarBud.WebApi.Modules.OrderModule.Entities;
using YourCarBud.WebApi.Modules.OrderStepModule.Dtos;

namespace YourCarBud.WebApi.Modules.OrderStepModule.Models
{
    // model is used in services and data layer
    public class OrderStepStatusUpdateModel
    {
        public Statuses Status { get; set; }
        public string Message { get; set; }
        public IOrderDetailCreationArgs DetailCreationArgs { get; set; }
    }
}