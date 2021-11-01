using System;

namespace YourCarBud.WebApi.Modules.OrderModule.Dtos
{
    public class OrderStepViewDto
    {
        public Guid Id { get; set; }
        public string StepName { get; set; }
        public string Status { get; set; }
    }
}