using System;
using System.Collections.Generic;

namespace YourCarBud.WebApi.Modules.OrderModule.Dtos
{
    public class OrderViewDto
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset CompletedAt { get; set; }

        public decimal TotalAmount { get; set; }

        public string ErrorMessage { get; set; }
        public string Status { get; set; }

        public virtual IEnumerable<OrderStepViewDto> Steps { get; set; }
    }
}